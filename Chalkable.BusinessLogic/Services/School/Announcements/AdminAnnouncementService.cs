﻿using System;
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
    public interface IAdminAnnouncementService : IBaseAnnouncementService
    {
        AnnouncementDetails Create(DateTime expiresDate);
        AnnouncementDetails Edit(int adminAnnouncementId, string title, string content, DateTime? expiresDate);
        IList<string> GetLastFieldValues(int personId);
        void SubmitGroupsToAnnouncement(int adminAnnouncementId, IList<int> groupsIds);
        AdminAnnouncement GetAdminAnnouncementById(int adminAnnouncementId);
        bool Exists(string title, int? excludedLessonPlaId);

        IList<AnnouncementComplex> GetAdminAnnouncementsForFeed(bool? complete, IList<int> gradeLevels, DateTime? fromDate, DateTime? toDate, int start = 0, int count = int.MaxValue);
        IList<AdminAnnouncement> GetAdminAnnouncements(IList<int> gradeLevels, DateTime? fromDate, DateTime? toDate, int? studentId);
        IList<AdminAnnouncement> GetAdminAnnouncementsByFilter(string filter); 
        AdminAnnouncement GetLastDraft();
    }

    public class AdminAnnouncementService : BaseAnnouncementService, IAdminAnnouncementService
    {
        public AdminAnnouncementService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        protected AdminAnnouncementDataAccess CreateDataAccess(UnitOfWork unitOfWork)
        {
            if(BaseSecurity.IsDistrictAdmin(Context))
                return new AdminAnnouncementForAdminDataAccess(unitOfWork);
            if(Context.Role == CoreRoles.STUDENT_ROLE)
                return new AdminAnnouncementForStudentDataAccess(unitOfWork);
            throw new ChalkableException("Not supported role for admin announcements");
        }

        public AnnouncementDetails Create(DateTime expiresDate)
        {
            Trace.Assert(Context.PersonId.HasValue);
            BaseSecurity.EnsureDistrictAdmin(Context);
            using (var u = Update())
            {
                var date = Context.NowSchoolTime;
                var res = CreateDataAccess(u).Create(date, date, Context.PersonId.Value);
                u.Commit();
                return res;
            }
        }

        public AnnouncementDetails Edit(int adminAnnouncementId, string title, string content, DateTime? expiresDate)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateDataAccess(uow);
                var lessonPlan = da.GetAnnouncement(adminAnnouncementId, Context.PersonId.Value);
                AnnouncementSecurity.EnsureInModifyAccess(lessonPlan, Context);

                lessonPlan.Title = title;
                lessonPlan.Content = content;
                if (expiresDate.HasValue)
                    lessonPlan.Expires = expiresDate.Value;
                da.Update(lessonPlan);
                uow.Commit();
                return da.GetDetails(adminAnnouncementId, Context.PersonId.Value, Context.RoleId);
            }    
        }

        public override Announcement EditTitle(int announcementId, string title)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var announcement = GetAdminAnnouncementById(announcementId);
            if (announcement.Title != title)
            {
                using (var uow = Update())
                {
                    if (!announcement.IsOwner)
                        throw new ChalkableSecurityException();
                    var da = CreateDataAccess(uow);
                    if (string.IsNullOrEmpty(title))
                        throw new ChalkableException("Title parameter is empty");
                    if (da.Exists(title, announcement.Id))
                        throw new ChalkableException("The item with current title already exists");
                    announcement.Title = title;
                    da.Update(announcement);
                    uow.Commit();
                }
            }
            return announcement;
        }

        public override void Submit(int announcementId)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            using (var uow = Update())
            {
                var da = CreateDataAccess(uow);
                var annRecipients = new DataAccessBase<AnnouncementGroup>(uow)
                    .GetAll(new AndQueryCondition { { AnnouncementGroup.ANNOUNCEMENT_REF_FIELD, announcementId } });
                if (annRecipients.Count == 0)
                    throw new ChalkableException("Admin Announcement has no groups. You can't sumbit admin announcement without selected groups");
                var ann = da.GetDetails(announcementId, Context.PersonId.Value, Context.RoleId);
                AnnouncementSecurity.EnsureInModifyAccess(ann, Context);
                if (string.IsNullOrEmpty(ann.Title))
                    throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Announcement Title "));
                if (ann.AdminAnnouncementData.IsDraft)
                {
                    ann.AdminAnnouncementData.Created = Context.NowSchoolTime;
                    ann.AdminAnnouncementData.State = AnnouncementState.Created;
                }
                da.Update(ann.AdminAnnouncementData);
                uow.Commit();
            }
        }

        public override void DeleteAnnouncement(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateDataAccess(uow);
                var announcement = da.GetAnnouncement(announcementId, Context.PersonId.Value);
                if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                    throw new ChalkableSecurityException();
                da.Delete(announcementId);
                uow.Commit();
            }
        }

        public override void DeleteAnnouncements(int personId, AnnouncementState state = AnnouncementState.Draft)
        {
            Trace.Assert(Context.PersonId.HasValue);
            DoUpdate(u =>
            {
                var da = CreateDataAccess(u);
                var drafts = da.GetAdminAnnouncements(new AndQueryCondition { { Announcement.STATE_FIELD, state } }, Context.PersonId.Value);
                da.Delete(drafts.Select(x => x.Id).ToList());
            });
        }

        public IList<string> GetLastFieldValues(int personId)
        {
            return DoRead(u => new AdminAnnouncementDataAccess(u).GetLastFieldValues(personId, int.MaxValue));
        }

        public void SubmitGroupsToAnnouncement(int adminAnnouncementId, IList<int> groupsIds)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var ann = GetAnnouncementById(adminAnnouncementId); //security check
            DoUpdate(u => SubmitAnnouncementGroups(ann.Id, groupsIds, u));
        }
        
        public override Announcement GetAnnouncementById(int id)
        {
            return GetAdminAnnouncementById(id);
        }
        
        public override AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u =>
                {
                    var res = CreateDataAccess(u).GetDetails(announcementId, Context.PersonId.Value, Context.RoleId);
                    if(res == null) 
                        throw new NoAnnouncementException();
                    return res;
                });
        }

        public AdminAnnouncement GetAdminAnnouncementById(int adminAnnouncementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetAnnouncement(adminAnnouncementId, Context.PersonId.Value));
        }


        private void SubmitAnnouncementGroups(int announcementId, IEnumerable<int> groupsIds, UnitOfWork uow)
        {
            if (groupsIds == null) return;
            var da = new DataAccessBase<AnnouncementGroup, int>(uow);
            var annGroups = da.GetAll(new AndQueryCondition { { AnnouncementGroup.ANNOUNCEMENT_REF_FIELD, announcementId } });
            da.Delete(annGroups);
            groupsIds = groupsIds.Distinct();
            var annRecipients = groupsIds.Select(gId => new AnnouncementGroup
            {
                AnnouncementRef = announcementId,
                GroupRef = gId
            }).ToList();
            da.Insert(annRecipients);
        }


        public bool Exists(string title, int? excludedLessonPlaId)
        {
            return DoRead(u => CreateDataAccess(u).Exists(title, excludedLessonPlaId));
        }

        public override void SetAnnouncementsAsComplete(DateTime? toDate, bool complete)
        {
            Trace.Assert(Context.PersonId.HasValue);
            if (BaseSecurity.IsDistrictAdmin(Context))
                CompleteAnnouncement(Context.PersonId.Value, complete, toDate);
        }

        private void CompleteAnnouncement(int personId, bool complete, DateTime? toDate)
        {
            DoUpdate(u =>
            {
                var anns = new AdminAnnouncementDataAccess(u)
                    .GetAnnouncements(new AnnouncementsQuery { PersonId = personId, ToDate = toDate })
                    .Announcements;
                var da = new AnnouncementRecipientDataDataAccess(u);
                foreach (var ann in anns)
                    da.UpdateAnnouncementRecipientData(ann.Id, personId, complete);
            });
        }

        protected override void SetComplete(Announcement announcement, bool complete)
        {
            DoUpdate(u => new AnnouncementRecipientDataDataAccess(u).UpdateAnnouncementRecipientData(announcement.Id, Context.PersonId.Value, complete)); 
        }

        public override bool CanAddStandard(int announcementId)
        {
            return DoRead(u => new AdminAnnouncementDataAccess(u).CanAddStandard(announcementId));
        }


        public IList<AnnouncementComplex> GetAdminAnnouncementsForFeed(bool? complete, IList<int> gradeLevels, DateTime? fromDate, DateTime? toDate, int start = 0, int count = int.MaxValue)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetAnnouncements(new AnnouncementsQuery
                {
                    Complete = complete,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Start = start,
                    Count = count,
                    PersonId = Context.PersonId,
                    RoleId = Context.RoleId,
                    GradeLevelsIds = gradeLevels
                })).Announcements;
        }

        public IList<AdminAnnouncement> GetAdminAnnouncements(IList<int> gradeLevels, DateTime? fromDate, DateTime? toDate, int? studentId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetAnnouncements(new AnnouncementsQuery
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    PersonId = Context.PersonId,
                    RoleId = Context.RoleId,
                    GradeLevelsIds = gradeLevels,
                    StudentId = studentId
                })).Announcements.Select(x => x.AdminAnnouncementData).ToList();
        }

        public IList<AdminAnnouncement> GetAdminAnnouncementsByFilter(string filter)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetAdminAnnouncementsByFilter(filter, Context.PersonId.Value));
        }
        
        public new AdminAnnouncement GetLastDraft()
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetLastDraft(Context.PersonId.Value));
        }

        public override IList<Person> GetAnnouncementRecipientPersons(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetAnnouncementRecipientPersons(announcementId, Context.PersonId.Value));
        }
    }
}
