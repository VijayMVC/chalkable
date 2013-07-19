using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementService
    {
        AnnouncementDetails CreateAnnouncement(int announcementTypeId, Guid? classId = null);
        AnnouncementDetails GetAnnouncementDetails(Guid announcementId);
        void DeleteAnnouncement(Guid announcementId);
        void DeleteAnnouncements(Guid classId, int announcementType, AnnouncementState state);
        void DeleteAnnouncements(Guid schoolpersonid, AnnouncementState state = AnnouncementState.Draft);

        Announcement EditAnnouncement(AnnouncementInfo announcement, Guid? markingPeriodId = null, Guid? classId = null, IList<RecipientInfo> recipients = null);
        void SubmitAnnouncement(Guid announcementId, Guid recipientId, Guid markingPeriodId);
        void SubmitForAdmin(Guid announcementId);

        Announcement GetAnnouncementById(Guid id);
        
        PaginatedList<AnnouncementComplex> GetAnnouncements(int start, int count, bool onlyOwners = false);
        PaginatedList<AnnouncementComplex> GetAnnouncements(bool starredOnly, int start, int count, Guid? classId, Guid? markingPeriodId = null, bool ownerOnly = false);
        IList<AnnouncementComplex> GetAnnouncements(DateTime fromDate, DateTime toDate, bool onlyOwners = false, IList<Guid> gradeLevelsIds = null, Guid? classId = null);

        Announcement GetLastDraft();

        void UpdateAnnouncementGradingStyle(Guid announcementId, GradingStyleEnum gradingStyle);
        Announcement DropUnDropAnnouncement(Guid announcementId, bool drop);
        IList<Announcement> GetDroppedAnnouncement(Guid markingPeriodClassId);


        IList<AnnouncementRecipient> GetAnnouncementRecipients(Guid announcementId);
        IList<Person> GetAnnouncementRecipientPersons(Guid announcementId); 
        int GetNewAnnouncementItemOrder(AnnouncementDetails announcement);

        Announcement Star(Guid id, bool starred);



    }

    public class AnnouncementService : SchoolServiceBase, IAnnouncementService
    {
        public AnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        private delegate AnnouncementQueryResult GetAnnouncementDelegate(AnnouncementDataAccess dataAccess, AnnouncementsQuery query);
        private static Dictionary<string, GetAnnouncementDelegate> getAnnouncementByRole = new Dictionary<string, GetAnnouncementDelegate>
                                            {
                                                {CoreRoles.STUDENT_ROLE.Name, (dataAccess, query) => dataAccess.GetStudentAnnouncements(query)},
                                                {CoreRoles.TEACHER_ROLE.Name, (dataAccess, query) => dataAccess.GetTeacherAnnouncements(query)},
                                                {CoreRoles.ADMIN_VIEW_ROLE.Name, (dataAccess, query) => dataAccess.GetAdminAnnouncements(query)},
                                                {CoreRoles.ADMIN_EDIT_ROLE.Name, (dataAccess, query) => dataAccess.GetAdminAnnouncements(query)},
                                                {CoreRoles.ADMIN_GRADE_ROLE.Name, (dataAccess, query) => dataAccess.GetAdminAnnouncements(query)},
                                            };
        
        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            using (var uow = Read())
            {
                if (!getAnnouncementByRole.ContainsKey(Context.Role.Name))
                    throw new ChalkableException(ChlkResources.ERR_GET_ANNOUNCEMENT_IS_UNSUPPORTED);
                
                var da = new AnnouncementDataAccess(uow);
                query.RoleId = Context.Role.Id;
                query.PersonId = Context.UserId;
                query.Now = Context.NowSchoolTime.Date;
                return  getAnnouncementByRole[Context.Role.Name].Invoke(da, query);
            }
        }
        
        public PaginatedList<AnnouncementComplex> GetAnnouncements(int start, int count, bool onlyOwners = false)
        {
            return GetAnnouncements(false, start, count, null, null, onlyOwners);
        }
        public PaginatedList<AnnouncementComplex> GetAnnouncements(bool starredOnly, int start, int count, Guid? classId, Guid? markingPeriodId = null, bool ownerOnly = false)
        {
            var q = new AnnouncementsQuery
            {
                StarredOnly = starredOnly,
                Start = start,
                Count = count,
                ClassId = classId,
                MarkingPeriodId = markingPeriodId,
                OwnedOnly = ownerOnly,
            };
            var res = GetAnnouncements(q);
            return new PaginatedList<AnnouncementComplex>(res.Announcements, start / count, count);
        }

        public IList<AnnouncementComplex> GetAnnouncements(DateTime fromDate, DateTime toDate, bool onlyOwners = false, IList<Guid> gradeLevelsIds = null, Guid? classId = null)
        {
            var q = new AnnouncementsQuery
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    GradeLevelIds = gradeLevelsIds,
                    ClassId = classId
                };
            return GetAnnouncements(q).Announcements; 
        }


        public AnnouncementDetails CreateAnnouncement(int announcementTypeId, Guid? classId = null)
        {
            if (!AnnouncementSecurity.CanCreateAnnouncement(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var annDa = new AnnouncementDataAccess(uow);
                var nowLocalDate = Context.NowSchoolTime;
                var markingPeriod = ServiceLocator.MarkingPeriodService.GetLastMarkingPeriod(nowLocalDate);
                var res = annDa.Create(announcementTypeId, classId, markingPeriod.Id, nowLocalDate, Context.UserId);
                uow.Commit();
                return res;
            }
        }

        public AnnouncementDetails GetAnnouncementDetails(Guid announcementId)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementDataAccess(uow);
                return da.GetDetails(announcementId, Context.UserId);
            }
        }


        public void DeleteAnnouncement(Guid announcementId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                var announcement = da.GetById(announcementId);
                if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                    throw new ChalkableSecurityException();
                da.Delete(announcementId, null, null, null, null);
                uow.Commit();
            }
        }
        
        public void DeleteAnnouncements(Guid classId, int announcementType, AnnouncementState state)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                da.Delete(null, Context.UserId, classId, announcementType, state);
                uow.Commit();
            }
        }

        public void DeleteAnnouncements(Guid schoolpersonid, AnnouncementState state = AnnouncementState.Draft)
        {
            if(Context.UserId != schoolpersonid && !BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                da.Delete(null, Context.UserId, null, null, state);
                uow.Commit();
            }
        }


        

        public Announcement DropUnDropAnnouncement(Guid announcementId, bool drop)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                var ann =  da.GetById(announcementId);
                if(!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                    throw new ChalkableSecurityException();
                
                var stAnnDa = new StudentAnnouncementDataAccess(uow);
                stAnnDa.Update(announcementId, drop);
                ann.Dropped = drop;
                da.Update(ann);
                uow.Commit();
                return ann;
            }
        }

        public IList<Announcement> GetDroppedAnnouncement(Guid markingPeriodClassId)
        {
            throw new NotImplementedException();
        }
        public Announcement EditAnnouncement(AnnouncementInfo announcement, Guid? markingPeriodId = null, Guid? classId = null, IList<RecipientInfo> recipients = null)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                var res = da.GetById(announcement.AnnouncementId);
                if (!AnnouncementSecurity.CanModifyAnnouncement(res, Context))
                    throw new ChalkableSecurityException();

                res.Content = announcement.Content;
                res.Subject = announcement.Subject;
                res.AnnouncementTypeRef = announcement.AnnouncementTypeId;
                if(announcement.ExpiresDate.HasValue)
                   res.Expires = announcement.ExpiresDate.Value;

                res = SetMarkingPeriodToAnnouncement(res, classId, markingPeriodId);
                res = PreperingReminderData(uow, res);
                res = ReCreateRecipients(uow, res, recipients);
                da.Update(res);
                uow.Commit();
                return res;
            }
        }


        private Announcement Submit(AnnouncementDataAccess dataAccess, UnitOfWork unitOfWork, Guid announcementId, 
            Guid? classId, Guid? markingPeriodId)
        {

            var res = dataAccess.GetById(announcementId);
            if (!AnnouncementSecurity.CanModifyAnnouncement(res, Context))
                throw new ChalkableSecurityException();
            var dateNow = Context.NowSchoolTime.Date;
            SetMarkingPeriodToAnnouncement(res, classId, markingPeriodId);
            if (res.State == AnnouncementState.Draft)
            {
                res.State = AnnouncementState.Created;
                res.Created = dateNow;
            }
            res = PreperingReminderData(unitOfWork, res);
            //var fgat = Entities.FinalGradeAnnouncementTypes.FirstOrDefault(x => x.FinalGrade.MarkingPeriodClassRef == res.MarkingPeriodClassRef
            //                                                                 && x.AnnouncementTypeRef == res.AnnouncementTypeRef);
            //if (fgat != null)
            //    res.GradingStyle = fgat.GradingStyle;
            //else
            //    res.GradingStyleEnumTyped = GradingStyleEnum.Numeric100;
            
            res.GradingStyle = GradingStyleEnum.Numeric100;
            dataAccess.Update(res);
            return res;
        }


        public void SubmitAnnouncement(Guid announcementId, Guid recipientId, Guid markingPeriodId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
                var res = Submit(da, uow, announcementId, recipientId, mp.Id);
                da.ReorderAnnouncements(mp.SchoolYearRef, res.AnnouncementTypeRef, res.PersonRef, recipientId);
                uow.Commit();
            }
        }
        public void SubmitForAdmin(Guid announcementId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                Submit(da, uow, announcementId, null, null);
                uow.Commit();
            }
        }
      
        private Announcement PreperingReminderData(UnitOfWork unitOfWork, Announcement announcement)
        {
            var dateNow = Context.NowSchoolTime;
            var expires = announcement.Expires;
            var da = new AnnouncementReminderDataAccess(unitOfWork);
            if (expires.Date >= Context.NowSchoolTime.Date)
            {
                var annReminders = da.GetList(announcement.Id, Context.UserId);
                foreach (var reminder in annReminders)
                {
                    reminder.RemindDate = reminder.Before.HasValue ? expires.AddDays(-reminder.Before.Value) : dateNow.Date;
                }
                da.Update(annReminders);
            }
            else da.DeleteByAnnouncementId(announcement.Id);
            return announcement;
        }
        private Announcement SetMarkingPeriodToAnnouncement(Announcement announcement, Guid? classId, Guid? markingPeriodId)
        {
            if (markingPeriodId.HasValue && classId.HasValue)
            {
                var mpc = ServiceLocator.MarkingPeriodService.GetMarkingPeriodClass(classId.Value, markingPeriodId.Value);
                if (mpc == null)
                    throw new ChalkableException(ChlkResources.ERR_CLASS_IS_NOT_SCHEDULED_FOR_MARKING_PERIOD);
                announcement.MarkingPeriodClassRef = mpc.Id;
            }
            return announcement;
        }
        private Announcement ReCreateRecipients(UnitOfWork unitOfWork, Announcement announcement, IList<RecipientInfo> recipientInfos)
        {
            if (recipientInfos != null && BaseSecurity.IsAdminViewer(Context))
            {
                var da = new AnnouncementRecipientDataAccess(unitOfWork);
                da.DeleteByAnnouncementId(announcement.Id);
                var annRecipients = new List<AnnouncementRecipient>();
                foreach (var recipientInfo in recipientInfos)
                {
                    annRecipients.Add(InternalAddAnnouncementRecipient(announcement.Id, recipientInfo));
                }
                da.Insert(annRecipients);
            }
            return announcement;
        }
        private AnnouncementRecipient InternalAddAnnouncementRecipient(Guid announcementId, RecipientInfo recipientInfo)
        {
            var announcementRecipient = new AnnouncementRecipient
            {
                Id = Guid.NewGuid(),
                AnnouncementRef = announcementId,
                ToAll = recipientInfo.ToAll,
                RoleRef = recipientInfo.ToAll ? null : recipientInfo.RoleId,
                GradeLevelRef = recipientInfo.ToAll ? null : recipientInfo.GradeLevelId,
                PersonRef = recipientInfo.ToAll ? null : recipientInfo.PersonId
            };
            return announcementRecipient;
        }

        //TODO: security check 
        public IList<AnnouncementRecipient> GetAnnouncementRecipients(Guid announcementId)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementRecipientDataAccess(uow);
                return da.GetList(announcementId);
            }
        }

        public void UpdateAnnouncementGradingStyle(Guid announcementId, GradingStyleEnum gradingStyle)
        {
            throw new NotImplementedException();
        }


        public Announcement GetAnnouncementById(Guid id)
        {
            using (var ouw = Read())
            {
                var da = new AnnouncementDataAccess(ouw);
                var res = da.GetAnnouncement(id, Context.Role.Id, Context.UserId); // security here 
                if(res == null)
                    throw new ChalkableSecurityException();
                return res;
            }
        }


        public int GetNewAnnouncementItemOrder(AnnouncementDetails announcement)
        {
            var attOrder = announcement.AnnouncementAttachments.Max(x => (int?)x.Order);
            var appOrder = announcement.AnnouncementApplications.Max(x => (int?)x.Order);
            int order = 0;
            if (attOrder.HasValue)
            {
                if (appOrder.HasValue)
                {
                    order = Math.Max(attOrder.Value, appOrder.Value) + 1;
                }
                else
                {
                    order = attOrder.Value + 1;
                }
            }
            else
            {
                if (appOrder.HasValue)
                {
                    order = appOrder.Value + 1;
                }
            }
            return order;
        }


        public Announcement Star(Guid id, bool starred)
        {
            var ann = GetAnnouncementById(id);
            using (var uow = Update())
            {
                new AnnouncementRecipientDataDataAccess(uow).Update(id, Context.UserId, starred, null);
                uow.Commit();
                return ann;
            }
        }


        public Announcement GetLastDraft()
        {
            using (var uow = Read())
            {
                var da = new AnnouncementDataAccess(uow);
                return da.GetLastDraft(Context.UserId);
            }
        }


        public IList<Person> GetAnnouncementRecipientPersons(Guid announcementId)
        {
            var ann = GetAnnouncementById(announcementId);
            if (ann.State == AnnouncementState.Draft)
                throw new ChalkableException(ChlkResources.ERR_NO_RECIPIENTS_IN_DRAFT_STATE);
            using (var uow = Read())
            {
                return new AnnouncementDataAccess(uow).GetAnnouncementRecipientPersons(announcementId, Context.UserId);
            }
        }


    }
}
