using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Model;
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
        IList<AnnouncementComplex> GetAnnouncementsComplex(DateTime? startDate, DateTime? endDate, IList<int> gradeLevels, bool? complete, bool ownedOnly = true, int start = 0, int count = int.MaxValue); 
        IList<AdminAnnouncement> GetAdminAnnouncements(IList<int> gradeLevels, DateTime? fromDate, DateTime? toDate, int? studentId);
        IList<AdminAnnouncement> GetAdminAnnouncementsByFilter(string filter); 
        AdminAnnouncement GetLastDraft();
    }

    public class AdminAnnouncementService : BaseAnnouncementService<AdminAnnouncement>, IAdminAnnouncementService
    {
        public AdminAnnouncementService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        protected override BaseAnnouncementDataAccess<AdminAnnouncement> CreateDataAccess(UnitOfWork unitOfWork)
        {
            return CreateAdminAnnouncementDataAccess(unitOfWork);
        }

        protected AdminAnnouncementDataAccess CreateAdminAnnouncementDataAccess(UnitOfWork unitOfWork)
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
                var res = CreateAdminAnnouncementDataAccess(u).Create(date, date, Context.PersonId.Value);
                u.Commit();
                return res;
            }
        }

        public AnnouncementDetails Edit(int adminAnnouncementId, string title, string content, DateTime? expiresDate)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateAdminAnnouncementDataAccess(uow);
                var adminAnnouncement = da.GetAnnouncement(adminAnnouncementId, Context.PersonId.Value);
                AnnouncementSecurity.EnsureInModifyAccess(adminAnnouncement, Context);

                adminAnnouncement.Title = title;
                adminAnnouncement.Content = content;
                if (expiresDate.HasValue)
                    adminAnnouncement.Expires = expiresDate.Value;

                if(adminAnnouncement.IsSubmitted)
                    ValidateAdminAnnouncemen(adminAnnouncement, uow, da);
                da.Update(adminAnnouncement);
                uow.Commit();
                return InternalGetDetails(da, adminAnnouncementId); // da.GetDetails(adminAnnouncementId, Context.PersonId.Value, Context.RoleId);
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
                    var da = CreateAdminAnnouncementDataAccess(uow);
                    if (string.IsNullOrEmpty(title))
                        throw new ChalkableException("Title parameter is empty");
                    if (da.Exists(title, Context.PersonId.Value, announcement.Id))
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
            Trace.Assert(Context.PersonId.HasValue);
            BaseSecurity.EnsureDistrictAdmin(Context);
            var res = GetAnnouncementDetails(announcementId);
            using (var uow = Update())
            {
                var da = CreateAdminAnnouncementDataAccess(uow);
                var ann = da.GetAnnouncement(announcementId, Context.PersonId.Value);
                AnnouncementSecurity.EnsureInModifyAccess(ann, Context);
                ValidateAdminAnnouncemen(ann, uow, da);
                ServiceLocator.AnnouncementAssignedAttributeService.ValidateAttributes(res.AnnouncementAttributes);
                if (ann.IsDraft)
                {
                    ann.Created = Context.NowSchoolTime;
                    ann.State = AnnouncementState.Created;
                }
                da.Update(ann);
                uow.Commit();
            }
        }

        private void ValidateAdminAnnouncemen(AdminAnnouncement announcement, UnitOfWork unitOfWork, AdminAnnouncementDataAccess dataAccess)
        {
            var annRecipients = new DataAccessBase<AnnouncementGroup>(unitOfWork)
                   .GetAll(new AndQueryCondition { { AnnouncementGroup.ANNOUNCEMENT_REF_FIELD, announcement.Id } });
            if (annRecipients.Count == 0)
                throw new ChalkableException("Admin Announcement has no groups. You can't sumbit admin announcement without selected groups");
            if (string.IsNullOrEmpty(announcement.Title))
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Admin Announcement Title "));
            if (dataAccess.Exists(announcement.Title, Context.PersonId.Value, announcement.Id))
                throw new ChalkableException("The item with current title already exists");
            
        }

        public override IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? startDate, DateTime? toDate, int? classId, bool? complete, bool ownerOnly = false)
        {
            var anns = GetAnnouncementsComplex(startDate, toDate, null, complete, ownerOnly);
            return DoRead(u => InternalGetDetailses(CreateDataAccess(u), anns.Select(x=>x.Id).ToList()));
        }

        public override void DeleteAnnouncement(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateAdminAnnouncementDataAccess(uow);
                var announcement = da.GetAnnouncement(announcementId, Context.PersonId.Value);
                if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                    throw new ChalkableSecurityException();
                da.Delete(announcementId);
                uow.Commit();
            }
        }

        public IList<string> GetLastFieldValues(int personId)
        {
            return DoRead(u => CreateAdminAnnouncementDataAccess(u).GetLastFieldValues(personId, int.MaxValue));
        }

        public void SubmitGroupsToAnnouncement(int adminAnnouncementId, IList<int> groupsIds)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var ann = GetAnnouncementById(adminAnnouncementId); //security check
            DoUpdate(u => SubmitAnnouncementGroups(ann.Id, groupsIds, u));
        }
        
        public AdminAnnouncement GetAdminAnnouncementById(int adminAnnouncementId)
        {
            return InternalGetAnnouncementById(adminAnnouncementId);
        }


        private void SubmitAnnouncementGroups(int announcementId, IEnumerable<int> groupsIds, UnitOfWork uow)
        {
            var da = new DataAccessBase<AnnouncementGroup, int>(uow);
            var annGroups = da.GetAll(new AndQueryCondition { { AnnouncementGroup.ANNOUNCEMENT_REF_FIELD, announcementId } });
            da.Delete(annGroups);
            if (groupsIds == null) return;
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
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateAdminAnnouncementDataAccess(u).Exists(title, Context.PersonId.Value, excludedLessonPlaId));
        }

        public IList<AnnouncementComplex> GetAnnouncementsComplex(DateTime? startDate, DateTime? endDate, IList<int> gradeLevels, bool? complete, bool ownedOnly = true
            , int start = 0, int count = int.MaxValue)
        {
            return DoRead(u => CreateAdminAnnouncementDataAccess(u).GetAnnouncements(new AnnouncementsQuery
            {
                Complete = complete,
                FromDate = startDate,
                ToDate = endDate,
                Start = start,
                Count = count,
                PersonId = Context.PersonId,
                RoleId = Context.RoleId,
                GradeLevelsIds = gradeLevels,
                OwnedOnly = ownedOnly
            })).Announcements;
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
                var anns = CreateAdminAnnouncementDataAccess(u)
                    .GetAnnouncements(new AnnouncementsQuery { PersonId = personId, ToDate = toDate })
                    .Announcements;
                var da = new AnnouncementRecipientDataDataAccess(u);
                foreach (var ann in anns)
                    da.UpdateAnnouncementRecipientData(ann.Id, (int) AnnouncementTypeEnum.Admin ,null, personId, null, complete, null, null);
            });
        }

        protected override void SetComplete(Announcement announcement, bool complete)
        {
            DoUpdate(
                u =>
                    new AnnouncementRecipientDataDataAccess(u).UpdateAnnouncementRecipientData(announcement.Id, (int)AnnouncementTypeEnum.Admin, null,
                        Context.PersonId.Value, null, complete, null, null));
        }

        public override bool CanAddStandard(int announcementId)
        {
            return DoRead(u => BaseSecurity.IsDistrictAdmin(Context) && CreateAdminAnnouncementDataAccess(u).CanAddStandard(announcementId));
        }
        
        public IList<AdminAnnouncement> GetAdminAnnouncements(IList<int> gradeLevels, DateTime? fromDate, DateTime? toDate, int? studentId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateAdminAnnouncementDataAccess(u).GetAnnouncements(new AnnouncementsQuery
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    PersonId = Context.PersonId,
                    RoleId = Context.RoleId,
                    GradeLevelsIds = gradeLevels,
                    StudentId = studentId,
                    OwnedOnly = (Context.Role == CoreRoles.DISTRICT_ADMIN_ROLE)
                })).Announcements.Select(x => x.AdminAnnouncementData).ToList();
        }

        public IList<AdminAnnouncement> GetAdminAnnouncementsByFilter(string filter)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateAdminAnnouncementDataAccess(u).GetAdminAnnouncementsByFilter(filter, Context.PersonId.Value));
        }
        
        public AdminAnnouncement GetLastDraft()
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateAdminAnnouncementDataAccess(u).GetLastDraft(Context.PersonId.Value));
        }

        protected override void SetComplete(int schoolYearId, int personId, int roleId, DateTime? tillDateToUpdate, int? classId)
        {
            DoUpdate( u =>
                    new AnnouncementRecipientDataDataAccess(u).UpdateAnnouncementRecipientData(null, (int) AnnouncementTypeEnum.Admin,schoolYearId,
                        personId, roleId, true, tillDateToUpdate, null));
        }
    }
}
