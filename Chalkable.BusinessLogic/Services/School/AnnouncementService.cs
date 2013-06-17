using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        Announcement EditAnnouncement(Announcement announcement, Guid? markingPeriodId = null, Guid? classId = null, IList<RecipientInfo> recipients = null);
        void SubmitAnnouncement(Guid announcementId, Guid recipientId, Guid markingPeriodId, bool isFirst);
        void SubmitForAdmin(Guid announcementId, bool isFirst);

        Announcement GetAnnouncementById(Guid id);

        PaginatedList<AnnouncementComplex> GetAnnouncements(int start, int count, bool onlyOwners = false);
        PaginatedList<AnnouncementComplex> GetAnnouncements(bool starredOnly, int start, int count, Guid? classId, Guid? markingPeriodId = null, bool ownerOnly = false);
        List<AnnouncementComplex> GetAnnouncementsFeedPage(bool starredOnly, int start, int count, ref int sourceCount, Guid? classId, Guid? markingPeriodId = null, bool ownedOnly = false);


        void UpdateAnnouncementGradingStyle(Guid announcementId, GradingStyleEnum gradingStyle);
        Announcement DropUnDropAnnouncement(Guid announcementId, bool drop);
        IList<Announcement> GetDroppedAnnouncement(Guid markingPeriodClassId);


        IList<AnnouncementRecipient> GetAnnouncementRecipients(Guid announcementId);
        int GetNewAnnouncementItemOrder(Announcement announcement);

        IList<AnnouncementType> GetAnnouncementTypes();
        AnnouncementType GetAnnouncementTypeById(int id);
        AnnouncementType GetAnnouncementTypeBySystemType(SystemAnnouncementType type);
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
        public List<AnnouncementComplex> GetAnnouncementsFeedPage(bool starredOnly, int start, int count, ref int sourceCount, Guid? classId,
                                             Guid? markingPeriodId = null, bool ownedOnly = false)
        {
            var q = new AnnouncementsQuery
            {
                StarredOnly = starredOnly,
                Start = start,
                Count = count,
                ClassId = classId,
                MarkingPeriodId = markingPeriodId,
                OwnedOnly = ownedOnly,
            };
            if (Context.Role == CoreRoles.STUDENT_ROLE && !starredOnly)
                q.ToDate = Context.NowSchoolTime.AddDays(16).Date;

            var res = GetAnnouncements(q);
            sourceCount = res.SourceCount;
            return res.Announcements;
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
                if(!AnnouncementSecurity.CanModifyAnnouncement(announcement,Context))
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
            if(Context.UserId != schoolpersonid)
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                da.Delete(null, Context.UserId, null, null, state);
                uow.Commit();
            }
        }


        public IList<AnnouncementType> GetAnnouncementTypes()
        {
            using (var uow = Read())
            {
                var da = new AnnouncementTypeDataAccess(uow);
                var res = da.GetList();
                if (BaseSecurity.IsAdminViewer(Context))
                {
                    res.First(x => x.SystemType == SystemAnnouncementType.Admin).CanCreate = true;
                }
                if (Context.Role == CoreRoles.TEACHER_ROLE)
                {
                    res.First(x => x.SystemType == SystemAnnouncementType.Standard).CanCreate = true;
                    res.First(x => x.SystemType == SystemAnnouncementType.BookReport).CanCreate = true;
                    res.First(x => x.SystemType == SystemAnnouncementType.Test).CanCreate = true;
                    res.First(x => x.SystemType == SystemAnnouncementType.TermPaper).CanCreate = true;
                    res.First(x => x.SystemType == SystemAnnouncementType.Quiz).CanCreate = true;
                    res.First(x => x.SystemType == SystemAnnouncementType.Project).CanCreate = true;
                    res.First(x => x.SystemType == SystemAnnouncementType.Midterm).CanCreate = true;
                    res.First(x => x.SystemType == SystemAnnouncementType.HW).CanCreate = true;
                    res.First(x => x.SystemType == SystemAnnouncementType.Final).CanCreate = true;
                    res.First(x => x.SystemType == SystemAnnouncementType.Essay).CanCreate = true;
                }
                return res;
            }
        }

        public AnnouncementType GetAnnouncementTypeById(int id)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementTypeDataAccess(uow);
                return da.GetById(id);
            }
        }

        public AnnouncementType GetAnnouncementTypeBySystemType(SystemAnnouncementType type)
        {
            return GetAnnouncementTypeById((int) type);
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
        public Announcement EditAnnouncement(Announcement announcement, Guid? markingPeriodId = null, Guid? classId = null, IList<RecipientInfo> recipients = null)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                var res = da.GetById(announcement.Id);
                if (!AnnouncementSecurity.CanModifyAnnouncement(announcement, Context) || !AnnouncementSecurity.CanModifyAnnouncement(res, Context))
                    throw new ChalkableSecurityException();

                var created = res.Created;
                var gradingStyle = res.GradingStyle;
                res = announcement; 
                res.GradingStyle = gradingStyle;
                res.Created = created;
                res = SetMarkingPeriodToAnnouncement(res, classId, markingPeriodId);
                res = PreperingReminderData(uow, res);
                res = ReCreateRecipients(uow, res, recipients);
                da.Update(res);
                uow.Commit();
                return res;
            }
        }


        private Announcement Submit(AnnouncementDataAccess dataAccess, UnitOfWork unitOfWork, Guid announcementId, 
            Guid? classId, Guid? markingPeriodId, bool isFirst)
        {

            var res = dataAccess.GetById(announcementId);
            if (!AnnouncementSecurity.CanModifyAnnouncement(res, Context))
                throw new ChalkableSecurityException();
            var dateNow = Context.NowSchoolTime;
            SetMarkingPeriodToAnnouncement(res, classId, markingPeriodId);
            if (isFirst)
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


        public void SubmitAnnouncement(Guid announcementId, Guid recipientId, Guid markingPeriodId, bool isFirst)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
                var res = Submit(da, uow, announcementId, recipientId, mp.Id, isFirst);
                da.ReorderAnnouncements(mp.SchoolYearRef, res.AnnouncementTypeRef, res.PersonRef, recipientId);
                uow.Commit();
            }
        }
        public void SubmitForAdmin(Guid announcementId, bool isFirst)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementDataAccess(uow);
                Submit(da, uow, announcementId, null, null, isFirst);
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
                //TODO: thing about this ... to many calls to db 
                var annReminders = da.GetList(announcement.Id, Context.UserId);
                foreach (var reminder in annReminders)
                {
                    reminder.RemindDate = reminder.Before.HasValue ? expires.AddDays(-reminder.Before.Value) : dateNow;
                    da.Update(reminder);
                }
            }
            else da.Delete(announcement.Id);
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
                da.Delete(announcement.Id);
                var annRecipients = new List<AnnouncementRecipient>();
                foreach (var recipientInfo in recipientInfos)
                {
                    annRecipients.Add(InternalAddAnnouncementRecipient(announcement.Id, recipientInfo));
                }
                da.Create(annRecipients);
            }
            return announcement;
        }
        private AnnouncementRecipient InternalAddAnnouncementRecipient(Guid announcementId, RecipientInfo recipientInfo)
        {
            var announcementRecipient = new AnnouncementRecipient
            {
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


        public int GetNewAnnouncementItemOrder(Announcement announcement)
        {
            throw new NotImplementedException();
        }
    }
}
