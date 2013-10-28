using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementService
    {
        AnnouncementDetails CreateAnnouncement(int announcementTypeId, int? classId = null);
        AnnouncementDetails GetAnnouncementDetails(int announcementId);
        void DeleteAnnouncement(int announcementId);
        void DeleteAnnouncements(int classId, int announcementType, AnnouncementState state);
        void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft);

        Announcement EditAnnouncement(AnnouncementInfo announcement, int? classId = null, IList<RecipientInfo> recipients = null);
        void SubmitAnnouncement(int announcementId, int recipientId);
        void SubmitForAdmin(int announcementId);

        Announcement GetAnnouncementById(int id);
        IList<AnnouncementComplex> GetAnnouncements(int count, bool gradedOnly);
        PaginatedList<AnnouncementComplex> GetAnnouncements(int start, int count, bool onlyOwners = false);
        PaginatedList<AnnouncementComplex> GetAnnouncements(bool starredOnly, int start, int count, int? classId, int? markingPeriodId = null, bool ownerOnly = false);
        IList<AnnouncementComplex> GetAnnouncements(DateTime fromDate, DateTime toDate, bool onlyOwners = false, IList<int> gradeLevelsIds = null, int? classId = null);
        IList<AnnouncementComplex> GetAnnouncements(string filter);
        Announcement GetLastDraft();

        void UpdateAnnouncementGradingStyle(int announcementId, GradingStyleEnum gradingStyle);
        Announcement DropUnDropAnnouncement(int announcementId, bool drop);
        IList<Announcement> GetDroppedAnnouncement(int markingPeriodClassId);


        IList<AnnouncementRecipient> GetAnnouncementRecipients(int announcementId);
        IList<Person> GetAnnouncementRecipientPersons(int announcementId); 
        int GetNewAnnouncementItemOrder(AnnouncementDetails announcement);

        Announcement Star(int id, bool starred);

        IList<string> GetLastFieldValues(int personId, int classId, int announcementType);

    }

    public class AnnouncementService : SchoolServiceBase, IAnnouncementService
    {
        public AnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        private AnnouncementDataAccess CreateAnnoucnementDataAccess(UnitOfWork unitOfWork)
        {
            if(BaseSecurity.IsAdminViewer(Context))
                return new AnnouncementForAdminDataAccess(unitOfWork);
            if(Context.Role == CoreRoles.TEACHER_ROLE)
                return new AnnouncementForTeacherDataAccess(unitOfWork);
            if(Context.Role == CoreRoles.STUDENT_ROLE)
                return new AnnouncementForStudentDataAccess(unitOfWork);
            throw new ChalkableException("Unsupported role for announcements");
        }


        public AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            using (var uow = Read())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                query.RoleId = Context.Role.Id;
                query.PersonId = Context.LocalId;
                query.Now = Context.NowSchoolTime.Date;
                return da.GetAnnouncements(query);
            }
        }

        public IList<AnnouncementComplex> GetAnnouncements(int count, bool gradedOnly)
        {
            return GetAnnouncements(new AnnouncementsQuery {Count = count, GradedOnly = gradedOnly}).Announcements;
        }

        public PaginatedList<AnnouncementComplex> GetAnnouncements(int start, int count, bool onlyOwners = false)
        {
            return GetAnnouncements(false, start, count, null, null, onlyOwners);
        }
        public PaginatedList<AnnouncementComplex> GetAnnouncements(bool starredOnly, int start, int count, int? classId, int? markingPeriodId = null, bool ownerOnly = false)
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
            return new PaginatedList<AnnouncementComplex>(res.Announcements, start / count, count, res.SourceCount);
        }

        public IList<AnnouncementComplex> GetAnnouncements(DateTime fromDate, DateTime toDate, bool onlyOwners = false, IList<int> gradeLevelsIds = null, int? classId = null)
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


        public IList<AnnouncementComplex> GetAnnouncements(string filter)
        {
            //TODO : rewrite impl for better performance
            var anns = GetAnnouncements(new AnnouncementsQuery()).Announcements;
            IList<AnnouncementComplex> res = new List<AnnouncementComplex>();
            const int adminAnnType = (int)SystemAnnouncementType.Admin;
            if (!string.IsNullOrEmpty(filter))
            {
                string[] words = filter.Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < words.Count(); i++)
                {
                    string word = words[i];
                    int annOrder;
                    IList<AnnouncementComplex> curretnAnns = new List<AnnouncementComplex>();
                    if (int.TryParse(words[i], out annOrder))
                    {
                        curretnAnns = anns.Where(x => x.Order == annOrder).ToList();
                    }
                    else
                    {
                        curretnAnns = anns.Where(x =>
                                         (!x.ClassAnnouncementTypeRef.HasValue && x.Subject != null && x.Subject.ToLower().Contains(word))
                                         || (x.ClassRef.HasValue && x.ClassName.ToLower().Contains(word))
                                         || (!x.ClassRef.HasValue && "all".Contains(word))
                                         || x.ClassAnnouncementTypeName.ToLower().Contains(word)
                                         || x.Title != null && x.Title.ToLower().Contains(word)
                                         || x.Content != null && x.Content.ToLower().Contains(word)
                                         ).ToList();
                    }
                    res = res.Union(curretnAnns).ToList();
                }
            }
            return res;
        }


        public AnnouncementDetails CreateAnnouncement(int announcementTypeId, int? classId = null)
        {
            if (!AnnouncementSecurity.CanCreateAnnouncement(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var annDa = CreateAnnoucnementDataAccess(uow);
                var nowLocalDate = Context.NowSchoolTime;
                var markingPeriod = ServiceLocator.MarkingPeriodService.GetLastMarkingPeriod(nowLocalDate);
                var res = annDa.Create(announcementTypeId, classId, markingPeriod.Id, nowLocalDate, Context.LocalId ?? 0);
                uow.Commit();
                return res;
            }
        }

        public AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            using (var uow = Read())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                return da.GetDetails(announcementId, Context.LocalId ?? 0, Context.Role.Id);
            }
        }


        public void DeleteAnnouncement(int announcementId)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                var announcement = da.GetById(announcementId);
                if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                    throw new ChalkableSecurityException();
                da.Delete(announcementId, null, null, null, null);
                uow.Commit();
            }
        }

        public void DeleteAnnouncements(int classId, int announcementType, AnnouncementState state)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                da.Delete(null, Context.LocalId, classId, announcementType, state);
                uow.Commit();
            }
        }

        public void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft)
        {
            if(Context.LocalId != schoolpersonid && !BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                da.Delete(null, Context.LocalId, null, null, state);
                uow.Commit();
            }
        }

        public Announcement DropUnDropAnnouncement(int announcementId, bool drop)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
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

        public IList<Announcement> GetDroppedAnnouncement(int markingPeriodClassId)
        {
            throw new NotImplementedException();
        }
        public Announcement EditAnnouncement(AnnouncementInfo announcement, int? classId = null, IList<RecipientInfo> recipients = null)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                var res = da.GetById(announcement.AnnouncementId);
                if (!AnnouncementSecurity.CanModifyAnnouncement(res, Context))
                    throw new ChalkableSecurityException();

                res.Content = announcement.Content;
                res.Subject = announcement.Subject;
                if (Context.Role == CoreRoles.TEACHER_ROLE && announcement.ClassAnnouncementTypeId.HasValue)
                    res.ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeId.Value;
                if (BaseSecurity.IsAdminViewer(Context))
                    res.ClassAnnouncementTypeRef = null;

                if(announcement.ExpiresDate.HasValue)
                   res.Expires = announcement.ExpiresDate.Value;
                res = SetClassToAnnouncement(res, classId, res.Expires);
                res = PreperingReminderData(uow, res);
                res = ReCreateRecipients(uow, res, recipients);
                da.Update(res);
                uow.Commit();
                return res;
            }
        }


        private Announcement Submit(AnnouncementDataAccess dataAccess, UnitOfWork unitOfWork, int announcementId,
            int? classId)
        {

            var res = dataAccess.GetById(announcementId);
            if (!AnnouncementSecurity.CanModifyAnnouncement(res, Context))
                throw new ChalkableSecurityException();
            var dateNow = Context.NowSchoolTime.Date;
            SetClassToAnnouncement(res, classId, res.Expires);
            if (res.State == AnnouncementState.Draft)
            {
                res.State = AnnouncementState.Created;
                res.Created = dateNow;
            }
            res = PreperingReminderData(unitOfWork, res);
            res.GradingStyle = GradingStyleEnum.Numeric100;
            //TODO : add gradingStyle to ClassAnnouncementtype
            //if (res.ClassAnnouncementTypeRef.HasValue)
            //{
            //    var classAnnType = new ClassAnnouncementTypeDataAccess(unitOfWork).GetById(res.ClassAnnouncementTypeRef.Value);
            //    classAnnType. 
            //}
           
            dataAccess.Update(res);
            return res;
        }


        public void SubmitAnnouncement(int announcementId, int recipientId)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                var res = Submit(da, uow, announcementId, recipientId);
                var sy = new SchoolYearDataAccess(uow).GetByDate(res.Expires);
                da.ReorderAnnouncements(sy.Id, res.ClassAnnouncementTypeRef.Value, res.PersonRef, recipientId);
                uow.Commit();
            }
        }
        public void SubmitForAdmin(int announcementId)
        {
            using (var uow = Update())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                Submit(da, uow, announcementId, null);
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
                var annReminders = da.GetList(announcement.Id, Context.LocalId ?? 0);
                foreach (var reminder in annReminders)
                {
                    reminder.RemindDate = reminder.Before.HasValue ? expires.AddDays(-reminder.Before.Value) : dateNow.Date;
                }
                da.Update(annReminders);
            }
            else da.DeleteByAnnouncementId(announcement.Id);
            return announcement;
        }
        private Announcement SetClassToAnnouncement(Announcement announcement, int? classId, DateTime expiresDate)
        {
            if (classId.HasValue)
            {
                var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(expiresDate);
                if (mp == null)
                    throw new NoMarkingPeriodException();
                var mpc = ServiceLocator.MarkingPeriodService.GetMarkingPeriodClass(classId.Value, mp.Id);
                if (mpc == null)
                    throw new ChalkableException(ChlkResources.ERR_CLASS_IS_NOT_SCHEDULED_FOR_MARKING_PERIOD);
                if (announcement.State == AnnouncementState.Created && announcement.ClassRef == classId)
                      throw new ChalkableException("Class can't be changed for submmited announcement");
                announcement.ClassRef = classId;
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
        private AnnouncementRecipient InternalAddAnnouncementRecipient(int announcementId, RecipientInfo recipientInfo)
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
        public IList<AnnouncementRecipient> GetAnnouncementRecipients(int announcementId)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementRecipientDataAccess(uow);
                return da.GetList(announcementId);
            }
        }

        public void UpdateAnnouncementGradingStyle(int announcementId, GradingStyleEnum gradingStyle)
        {
            throw new NotImplementedException();
        }


        public Announcement GetAnnouncementById(int id)
        {
            using (var ouw = Read())
            {
                var da = CreateAnnoucnementDataAccess(ouw);
                var res = da.GetAnnouncement(id, Context.Role.Id, Context.LocalId ?? 0); // security here 
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


        public Announcement Star(int id, bool starred)
        {
            var ann = GetAnnouncementById(id);
            using (var uow = Update())
            {
                new AnnouncementRecipientDataDataAccess(uow)
                    .Update(id, Context.LocalId ?? 0, starred, null, Context.NowSchoolTime.Date);
                uow.Commit();
                return ann;
            }
        }


        public Announcement GetLastDraft()
        {
            using (var uow = Read())
            {
                var da = CreateAnnoucnementDataAccess(uow);
                return da.GetLastDraft(Context.LocalId ?? 0);
            }
        }


        public IList<Person> GetAnnouncementRecipientPersons(int announcementId)
        {
            var ann = GetAnnouncementById(announcementId);
            if (ann.State == AnnouncementState.Draft)
                throw new ChalkableException(ChlkResources.ERR_NO_RECIPIENTS_IN_DRAFT_STATE);
            using (var uow = Read())
            {
                return CreateAnnoucnementDataAccess(uow).GetAnnouncementRecipientPersons(announcementId, Context.LocalId ?? 0);
            }
        }

        public IList<string> GetLastFieldValues(int personId, int classId, int announcementType)
        {
            using (var uow = Read())
            {
                return CreateAnnoucnementDataAccess(uow).GetLastFieldValues(personId, classId, announcementType, 10);
            }
        }

    }
}
