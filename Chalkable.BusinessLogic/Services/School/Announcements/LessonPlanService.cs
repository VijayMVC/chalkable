using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Data.School.Model.Announcements.Sis;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface ILessonPlanService : IBaseAnnouncementService
    {
        AnnouncementDetails Create(int classId, DateTime? startDate, DateTime? endDate);
        AnnouncementDetails CreateFromTemplate(int lessonPlanTemplateId, int classId);
        AnnouncementDetails Edit(int lessonPlanId, int classId, int? galleryCategoryId, string title, string content, DateTime? startDate, DateTime? endDate, bool visibleForStudent);
        IList<LessonPlan> GetLessonPlansTemplates(int? galleryCategoryId, string title, int? classId); 
        IList<string> GetLastFieldValues(int classId);
        bool Exists(string title, int? excludedLessonPlaId);
        bool ExistsInGallery(string title, int? exceludedLessonPlanId);
        void SetVisibleForStudent(int lessonPlanId, bool visible);
        LessonPlan GetLessonPlanById(int lessonPlanId);

        IList<LessonPlan> GetLessonPlans(DateTime? fromDate, DateTime? toDate, int? classId, int? galleryCategoryId);
        IList<LessonPlan> GetLessonPlansbyFilter(string filter); 
        IList<AnnouncementComplex> GetLessonPlansForFeed(DateTime? fromDate, DateTime? toDate, int? galeryCategoryId, int? classId, bool? complete, bool onlyOwners = false, int start = 0, int count = int.MaxValue); 
        LessonPlan GetLastDraft();

        void DuplicateLessonPlan(int lessonPlanId, IList<int> classIds);
    }

    public class LessonPlanService : BaseAnnouncementService<LessonPlan>, ILessonPlanService
    {
        public LessonPlanService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        protected override BaseAnnouncementDataAccess<LessonPlan> CreateDataAccess(UnitOfWork unitOfWork)
        {
            return CreateLessonPlanDataAccess(unitOfWork);
        }

        protected LessonPlanDataAccess CreateLessonPlanDataAccess(UnitOfWork unitOfWork)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            if (BaseSecurity.IsTeacher(Context))
                return new LessonPlanForTeacherDataAccess(unitOfWork, Context.SchoolYearId.Value);
            if (Context.Role == CoreRoles.STUDENT_ROLE)
                return new LessonPlanForStudentDataAccess(unitOfWork, Context.SchoolYearId.Value);

            throw new ChalkableException("Not supported role for lesson plan");
        }

        public AnnouncementDetails Create(int classId, DateTime? startDate, DateTime? endDate)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            BaseSecurity.EnsureTeacher(Context);
            using (var u = Update())
            {
                var res = CreateLessonPlanDataAccess(u).Create(classId, Context.NowSchoolTime, startDate, endDate, Context.PersonId.Value, Context.SchoolYearId.Value);
                u.Commit();
                return PrepareStandardsWithCode(res);
            }
        }

        public AnnouncementDetails CreateFromTemplate(int lessonPlanTemplateId, int classId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            BaseSecurity.EnsureTeacher(Context);
            AnnouncementDetails res;
            var annApps = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(lessonPlanTemplateId, true);
            var appIds = annApps.Select(aa => aa.ApplicationRef).ToList();
            //get only simple apps
            var apps = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(appIds).Where(a=>!a.IsAdvanced).ToList();
            annApps = annApps.Where(aa => apps.Any(a => a.Id == aa.ApplicationRef)).ToList();

            using (var u = Update())
            {
                res = CreateLessonPlanDataAccess(u).CreateFromTemplate(lessonPlanTemplateId, Context.PersonId.Value, classId);
                res.AnnouncementAttachments = AnnouncementAttachmentService.CopyAnnouncementAttachments(lessonPlanTemplateId, new List<int> { res.Id }, u, ServiceLocator, ConnectorLocator);
                res.AnnouncementAttributes = AnnouncementAssignedAttributeService.CopyNonStiAttributes(lessonPlanTemplateId, new List<int>{res.Id}, u, ServiceLocator, ConnectorLocator);
                res.AnnouncementApplications = ApplicationSchoolService.CopyAnnApplications(annApps, new List<int> { res.Id }, u);
                u.Commit();
            }
            return PrepareStandardsWithCode(res); 
        }

        public void DuplicateLessonPlan(int lessonPlanId, IList<int> classIds)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var lessonPlan = GetLessonPlanById(lessonPlanId); // security check
            BaseSecurity.EnsureTeacher(Context);
            if (lessonPlan.IsDraft)
                throw new ChalkableException("Only submited lesson plan can be duplicate");
            
            //get announcementApplications for copying
            var annApps = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(lessonPlanId, true);
            var appIds = annApps.Select(aa => aa.ApplicationRef).ToList();
            //get only simple apps
            var apps = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(appIds).Where(a => !a.IsAdvanced).ToList();
            annApps = annApps.Where(aa => apps.Any(a => a.Id == aa.ApplicationRef)).ToList();
            
            using (var u = Update())
            {
                var resIds = CreateLessonPlanDataAccess(u).DuplicateLessonPlan(lessonPlanId, classIds, Context.NowSchoolYearTime);
                AnnouncementAttachmentService.CopyAnnouncementAttachments(lessonPlanId, resIds, u, ServiceLocator, ConnectorLocator);
                AnnouncementAssignedAttributeService.CopyNonStiAttributes(lessonPlanId, resIds, u, ServiceLocator, ConnectorLocator);
                ApplicationSchoolService.CopyAnnApplications(annApps, resIds, u);
                u.Commit();
            }
        }


        public AnnouncementDetails Edit(int lessonPlanId, int classId, int? galleryCategoryId, string title, string content,
                                        DateTime? startDate, DateTime? endDate, bool visibleForStudent)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var lessonPlan = GetLessonPlanById(lessonPlanId); // security check 
            using (var uow = Update())
            {
                var da = CreateLessonPlanDataAccess(uow);
                AnnouncementSecurity.EnsureInModifyAccess(lessonPlan, Context);
                
                if (lessonPlan.ClassRef != classId)
                {
                    if(!lessonPlan.IsDraft)
                        throw new ChalkableException("Class can't be changed for submited lesson plan");

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
                if(lessonPlan.IsSubmitted)
                    ValidateLessonPlan(lessonPlan, da);
                da.Update(lessonPlan);
                uow.Commit();
                return GetDetails(da, lessonPlanId);
            }                
        }


        public override Announcement EditTitle(int announcementId, string title)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var announcement = GetLessonPlanById(announcementId);
            if (announcement.Title != title)
            {
                using (var uow = Update())
                {
                    if (!announcement.IsOwner)
                        throw new ChalkableSecurityException();
                    var da = CreateLessonPlanDataAccess(uow);
                    if (string.IsNullOrEmpty(title))
                        throw new ChalkableException("Title parameter is empty");
                    if (da.ExistsInGallery(title, announcement.Id) && announcement.GalleryCategoryRef.HasValue)
                        throw new ChalkableException("The item with current title already exists in the gallery");
                    announcement.Title = title;
                    da.Update(announcement);
                    uow.Commit();
                }
            }
            return announcement;
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
                ValidateLessonPlan(ln, da);
                if (ln.IsDraft)
                {
                    ln.State = AnnouncementState.Created;
                    ln.Created = Context.NowSchoolTime.Date;
                    da.Update(ln);
                }
                u.Commit();
            }
        }

        private void ValidateLessonPlan(LessonPlan lessonPlan, LessonPlanDataAccess da)
        {
            if(!lessonPlan.StartDate.HasValue)
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "LessonPlan start date "));
            if(!lessonPlan.EndDate.HasValue)
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "LessonPlan end date "));
            if(lessonPlan.StartDate > lessonPlan.EndDate)
                throw new ChalkableException("Lesson Plan are no valid. Start date is greater the end date");

            if (string.IsNullOrEmpty(lessonPlan.Title))
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "LessonPlan Title "));
            if (da.ExistsInGallery(lessonPlan.Title, lessonPlan.Id) && lessonPlan.GalleryCategoryRef.HasValue)
                throw new ChalkableException("Lesson Plan with current title already exists in the gallery");
                    
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

        public IList<string> GetLastFieldValues(int classId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLastFields(Context.PersonId.Value, classId));
        }
        
        public bool Exists(string title, int? excludedLessonPlaId)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u).Exists(title, excludedLessonPlaId));
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
                        throw new NoAnnouncementException();
                    return res;
                });
        }

        public override Announcement GetAnnouncementById(int id)
        {
            return GetLessonPlanById(id);
        }

        public override AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u =>  GetDetails(CreateDataAccess(u), announcementId));
        }
        
        private AnnouncementDetails GetDetails(BaseAnnouncementDataAccess<LessonPlan> dataAccess, int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var ann = dataAccess.GetDetails(announcementId, Context.PersonId.Value, Context.Role.Id);
            if (ann == null)
                throw new NoAnnouncementException();
            return PrepareStandardsWithCode(ann);
        }

        private AnnouncementDetails PrepareStandardsWithCode(AnnouncementDetails announcement)
        {
            var annStandards = ServiceLocator.StandardService.GetAnnouncementStandards(announcement.Id);
            announcement.AnnouncementStandards = annStandards.Where(x => announcement.AnnouncementStandards.Any(y => y.StandardRef == x.StandardRef
                                                                                                && y.AnnouncementRef == x.AnnouncementRef)).ToList();
            return announcement;
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

        
        public IList<LessonPlan> GetLessonPlansTemplates(int? galleryCategoryId, string title, int? classId)
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

        public LessonPlan GetLastDraft()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            return DoRead(u => CreateLessonPlanDataAccess(u).GetLastDraft(Context.PersonId.Value));
        }

        public bool ExistsInGallery(string title, int? exceludedLessonPlanId)
        {
            return DoRead(u => CreateLessonPlanDataAccess(u).ExistsInGallery(title, exceludedLessonPlanId));
        }
    }
}
