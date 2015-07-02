using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface ILessonPlanService : IBaseAnnouncementService
    {
        AnnouncementDetails Create(int classId, DateTime? startDate, DateTime? endDate);
        AnnouncementDetails CreateFromTemplate(int lessonPlanTemplateId, int classId);
        AnnouncementDetails Edit(int lessonPlanId, int classId, int? galleryCategoryId, string title, string content, DateTime? startDate, DateTime? endDate, bool visibleForStudent);
        IList<LessonPlan> GetLessonPlansTemplates(int galleryCategoryId, string title, int? classId); 
        IList<string> GetLastFieldValues(int classId);
        bool Exists(string title, int? excludedLessonPlaId);
        void SetVisibleForStudent(int lessonPlanId, bool visible);
        LessonPlan GetLessonPlanById(int lessonPlanId);

        IList<LessonPlan> GetLessonPlans(DateTime? fromDate, DateTime? toDate, int? classId, int? galleryCategoryId);
        IList<LessonPlan> GetLessonPlansbyFilter(string filter); 
        IList<AnnouncementComplex> GetLessonPlansForFeed(DateTime? fromDate, DateTime? toDate, int? galeryCategoryId, int? classId, bool? complete, bool onlyOwners = false, int start = 0, int count = int.MaxValue); 
        LessonPlan GetLastDraft();
    }

    public class LessonPlanService : BaseAnnouncementService, ILessonPlanService
    {
        public LessonPlanService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public AnnouncementDetails Create(int classId, DateTime? startDate, DateTime? endDate)
        {
            Trace.Assert(Context.PersonId.HasValue);
            BaseSecurity.EnsureTeacher(Context);
            using (var u = Update())
            {
                var res = CreateLessonPlanDataAccess(u).Create(classId, Context.NowSchoolTime, startDate, endDate, Context.PersonId.Value);
                u.Commit();
                return res;
            }
        }

        public AnnouncementDetails CreateFromTemplate(int lessonPlanTemplateId, int classId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            BaseSecurity.EnsureTeacher(Context);
            AnnouncementDetails res;
            using (var u = Update())
            {
                res = CreateLessonPlanDataAccess(u).CreateFromTemplate(lessonPlanTemplateId, Context.PersonId.Value, classId);
                var templateAttachments = new AnnouncementAttachmentDataAccess(u).GetAll(new AndQueryCondition
                    {
                        {AnnouncementAttachment.ANNOUNCEMENT_REF_FIELD, lessonPlanTemplateId}
                    });
                foreach (var annAtt in templateAttachments)
                {
                    var attachmentInfo = ServiceLocator.AnnouncementAttachmentService.GetAttachmentContent(annAtt);
                    ServiceLocator.AnnouncementAttachmentService.AddAttachment(res.Id, AnnouncementType.LessonPlan, attachmentInfo.Content, annAtt.Name, annAtt.Uuid);
                }
                u.Commit();
            }
            res.AnnouncementAttachments = ServiceLocator.AnnouncementAttachmentService.GetAttachments(res.Id);
            return res;
        }

        public AnnouncementDetails Edit(int lessonPlanId, int classId, int? galleryCategoryId, string title, string content,
                                        DateTime? startDate, DateTime? endDate, bool visibleForStudent)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateLessonPlanDataAccess(uow);
                var lessonPlan = da.GetAnnouncement(lessonPlanId, Context.PersonId.Value);
                AnnouncementSecurity.EnsureInModifyAccess(lessonPlan, Context);
                
                if (lessonPlan.ClassRef != classId)
                {
                    if(!lessonPlan.IsDraft)
                        throw new ChalkableException("Class can't be changed for submmited lesson plan");

                    lessonPlan.ClassRef = classId;
                    //clear old data befor swiching 
                    new AnnouncementApplicationDataAccess(uow).DeleteByAnnouncementId(lessonPlan.Id);
                    new AnnouncementStandardDataAccess(uow).DeleteNotAssignedToClass(lessonPlan.Id, classId);
                }
                
                lessonPlan.Title = title;
                lessonPlan.Content = content;
                lessonPlan.StartDate = startDate;
                lessonPlan.EndDate = endDate;
                lessonPlan.VisibleForStudent = visibleForStudent;
                lessonPlan.GalleryCategoryRef = galleryCategoryId;
                da.Update(lessonPlan);
                uow.Commit();
                return da.GetDetails(lessonPlanId, Context.PersonId.Value, Context.RoleId);
            }                
        }


        public override void Submit(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var u = Update())
            {
                var da = CreateLessonPlanDataAccess(u);
                var res = da.GetDetails(announcementId, Context.PersonId.Value, Context.RoleId);
                var ln = res.LessonPlanData;
                AnnouncementSecurity.EnsureInModifyAccess(res, Context);
                if (ln.IsDraft)
                {
                    ln.State = AnnouncementState.Created;
                    ln.Created = Context.NowSchoolTime.Date;
                    if (string.IsNullOrEmpty(ln.Title))
                        throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Announcement Title "));

                    da.Update(ln);
                }
                u.Commit();
            }
        }

        public override void DeleteAnnouncement(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateLessonPlanDataAccess(uow);
                var announcement = da.GetAnnouncement(announcementId, Context.PersonId.Value);
                if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                    throw new ChalkableSecurityException();
                da.Delete(announcementId);
                uow.Commit();
            }
        }

        public override void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft)
        {
            Trace.Assert(Context.PersonId.HasValue);
            DoUpdate(u =>
            {
                var da = CreateLessonPlanDataAccess(u);
                var drafts = da.GetLessonPlans(new AndQueryCondition { { Announcement.STATE_FIELD, state } }, Context.PersonId.Value);
                da.Delete(drafts.Select(x => x.Id).ToList());
            });
        }

        public IList<string> GetLastFieldValues(int classId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLastFields(Context.PersonId.Value, classId));
        }
        
        public bool Exists(string title, int? excludedLessonPlaId)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u).Exists(title, excludedLessonPlaId));
        }

        protected LessonPlanDataAccess CreateLessonPlanDataAccess(UnitOfWork unitOfWork)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            if(BaseSecurity.IsTeacher(Context))
                return new LessonPlanForTeacherDataAccess(unitOfWork, Context.SchoolLocalId.Value);
            if(Context.Role == CoreRoles.STUDENT_ROLE)
                return new LessonPlanForStudentDataAccess(unitOfWork, Context.SchoolLocalId.Value);
            
            throw new ChalkableException("Not supported role for lesson plan");
        }


        public void SetVisibleForStudent(int lessonPlanId, bool visible)
        {
            var lessonPlan = GetLessonPlanById(lessonPlanId);
            AnnouncementSecurity.EnsureInModifyAccess(lessonPlan, Context);
            if(lessonPlan.VisibleForStudent == visible) return;
            lessonPlan.VisibleForStudent = visible;
            DoUpdate(u => CreateLessonPlanDataAccess(u).Update(lessonPlan));
        }

        public LessonPlan GetLessonPlanById(int lessonPlanId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u =>
                {
                    var res = CreateLessonPlanDataAccess(u).GetAnnouncement(lessonPlanId, Context.PersonId.Value);
                    if(res == null)
                        throw new ChalkableSecurityException();
                    return res;
                });
        }

        public override AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetDetails(announcementId, Context.PersonId.Value, Context.RoleId));
        }


        protected override void SetComplete(Announcement announcement, bool complete)
        {
            DoUpdate(u => new AnnouncementRecipientDataDataAccess(u).UpdateAnnouncementRecipientData(announcement.Id, Context.PersonId.Value, complete));
        }

        public override void SetAnnouncementsAsComplete(DateTime? toDate, bool complete)
        {
            Trace.Assert(Context.PersonId.HasValue);
            CompleteAnnouncement(Context.PersonId.Value, complete, toDate);
        }

        private void CompleteAnnouncement(int personId, bool complete, DateTime? toDate)
        {
            DoUpdate(u =>
            {
                var anns = CreateLessonPlanDataAccess(u)
                    .GetAnnouncements(new AnnouncementsQuery { PersonId = personId, ToDate = toDate })
                    .Announcements;
                var da = new AnnouncementRecipientDataDataAccess(u);
                foreach (var ann in anns)
                    da.UpdateAnnouncementRecipientData(ann.Id, personId, complete);
            });
        }

        public override bool CanAddStandard(int announcementId)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u).CanAddStandard(announcementId));
        }

        public IList<LessonPlan> GetLessonPlansTemplates(int galleryCategoryId, string title, int? classId)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLessonPlanTemplates(galleryCategoryId, title, classId));
        }
        
        public IList<LessonPlan> GetLessonPlans(DateTime? fromDate, DateTime? toDate, int? classId, int? galleryCategoryId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLessonPlans(fromDate, toDate, classId, galleryCategoryId, Context.PersonId.Value));
        }

        public IList<LessonPlan> GetLessonPlansbyFilter(string filter)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLessonPlansByFilter(filter, Context.PersonId.Value));
        }

        public IList<AnnouncementComplex> GetLessonPlansForFeed(DateTime? fromDate, DateTime? toDate, int? galleryCategoryId, int? classId, bool? complete, bool onlyOwners = false, int start = 0, int count = int.MaxValue)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u).GetAnnouncements(new AnnouncementsQuery
                {
                    RoleId = Context.RoleId,
                    PersonId = Context.PersonId,
                    FromDate = fromDate,
                    ToDate = toDate,
                    GalleryCategoryId = galleryCategoryId,
                    ClassId = classId,
                    Complete = complete,
                    OwnedOnly = onlyOwners,
                    Start = start,
                    Count = count
                })).Announcements;
        }

        public new LessonPlan GetLastDraft()
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLastDraft(Context.PersonId.Value));
        }

        public override IList<Person> GetAnnouncementRecipientPersons(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetAnnouncementRecipientPersons(announcementId, Context.PersonId.Value));
        }
    }
}
