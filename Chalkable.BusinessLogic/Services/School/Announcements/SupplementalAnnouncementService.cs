using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface ISupplementalAnnouncementService : IBaseAnnouncementService
    {
        AnnouncementDetails Create(int classId, DateTime expiresDate, int classAnnouncementTypeId);
        AnnouncementDetails Edit(int supplementalAnnouncementPlanId, int classId, int? galleryCategoryId, string title, string content, DateTime? expiresDate, bool visibleForStudent, IList<int> recipientsIds);
        void SetVisibleForStudent(int supplementalAnnouncementPlanId, bool visible);
        SupplementalAnnouncement GetSupplementalAnnouncementById(int supplementalAnnouncementPlanId);
        IList<SupplementalAnnouncement> GetSupplementalAnnouncement(DateTime? fromDate, DateTime? toDate, int? classId, int? teacherId, bool filterByStartDate = true);
        IList<AnnouncementComplex> GetSupplementalAnnouncementForFeed(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start = 0, int count = int.MaxValue, bool? ownedOnly = null);
        bool Exists(string title, int? excludeSupplementalAnnouncementPlanId);

    }

    public class SupplementalAnnouncementService : BaseAnnouncementService<SupplementalAnnouncement>, ISupplementalAnnouncementService
    {
        public SupplementalAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public override IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? startDate, DateTime? toDate, int? classId, bool? complete,
            bool ownerOnly = false)
        {
            throw new NotImplementedException();
        }

        public override IList<int> Copy(IList<int> classAnnouncementIds, int fromClassId, int toClassId, DateTime? startDate)
        {
            throw new NotImplementedException();
        }

        public override IList<AnnouncementComplex> GetAnnouncementsByIds(IList<int> announcementIds)
        {
            return DoRead(u => InternalGetDetailses(CreateSupplementalAnnouncementDataAccess(u), announcementIds))
                .Cast<AnnouncementComplex>().ToList();
        }

        public override void DeleteAnnouncement(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateSupplementalAnnouncementDataAccess(uow);
                var announcement = da.GetAnnouncement(announcementId, Context.PersonId.Value);
                if (!AnnouncementSecurity.CanDeleteAnnouncement(announcement, Context))
                    throw new ChalkableSecurityException();
                da.Delete(announcementId);
                uow.Commit();
            }
        }

        public override Announcement EditTitle(int announcementId, string title)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var announcement = GetSupplementalAnnouncementById(announcementId);
            if (announcement.Title != title)
            {
                using (var uow = Update())
                {
                    if (!announcement.IsOwner)
                        throw new ChalkableSecurityException();
                    var da = CreateSupplementalAnnouncementDataAccess(uow);
                    if (string.IsNullOrEmpty(title))
                        throw new ChalkableException("Title parameter is empty");
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
                var da = CreateSupplementalAnnouncementDataAccess(u);
                var res = InternalGetDetails(da, announcementId);
                var suppAnnouncement = res.SupplementalAnnouncementData;
                AnnouncementSecurity.EnsureInModifyAccess(res, Context);
                //ValidateSupplementalAnnouncement(suppAnnouncement, da, ServiceLocator, suppAnnouncement.);
                if (suppAnnouncement.IsDraft)
                {
                    suppAnnouncement.State = AnnouncementState.Created;
                    suppAnnouncement.Created = Context.NowSchoolTime.Date;
                    da.Update(suppAnnouncement);
                }
                ServiceLocator.AnnouncementAssignedAttributeService.ValidateAttributes(res.AnnouncementAttributes);
                u.Commit();
            }
        }

        public override void SetAnnouncementsAsComplete(DateTime? date, bool complete)
        {
            DoUpdate(u => new AnnouncementRecipientDataDataAccess(u)
                .UpdateAnnouncementRecipientData(null, AnnouncementTypeEnum.Supplemental, Context.SchoolYearId, Context.PersonId, Context.RoleId, 
                    complete, null, null, date, false));
        }

        public override bool CanAddStandard(int announcementId)
        {
            return DoRead(u => BaseSecurity.IsTeacher(Context) && CreateSupplementalAnnouncementDataAccess(u).CanAddStandard(announcementId));
        }

        protected SupplementalAnnouncementDataAccess CreateSupplementalAnnouncementDataAccess(UnitOfWork unitOfWork)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);

            var requiredPermissions = new List<string>
            {
                ClaimInfo.VIEW_CLASSROOM,
                ClaimInfo.MAINTAIN_CLASSROOM
            };

            if (BaseSecurity.IsTeacher(Context) && Context.Claims.HasOneOfPermissions(requiredPermissions))
                return new SupplementalAnnouncementDataAccess(unitOfWork, Context.SchoolYearId.Value);

            if (BaseSecurity.IsStudent(Context))
                return new SupplementalAnnouncementDataAccess(unitOfWork, Context.SchoolYearId.Value);

            throw new ChalkableSecurityException("You have no rights to access supplemental announcements");
        }

        protected override BaseAnnouncementDataAccess<SupplementalAnnouncement> CreateDataAccess(UnitOfWork unitOfWork)
        {
            return CreateSupplementalAnnouncementDataAccess(unitOfWork);
        }

        protected override void SetComplete(Announcement announcement, bool complete)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            DoUpdate(u => new AnnouncementRecipientDataDataAccess(u).UpdateAnnouncementRecipientData(announcement.Id, AnnouncementTypeEnum.Supplemental,
               Context.SchoolYearId.Value, Context.PersonId.Value, Context.RoleId, complete, null, null, null, false));
        }

        protected override void SetComplete(int schoolYearId, int personId, int roleId, DateTime startDate, DateTime endDate, int? classId,
            bool filterByExpiryDate)
        {
            DoUpdate(u => new AnnouncementRecipientDataDataAccess(u).UpdateAnnouncementRecipientData(null, AnnouncementTypeEnum.Supplemental,
              schoolYearId, personId, roleId, true, classId, startDate, endDate, filterByExpiryDate));
        }

        protected override void SetUnComplete(int schoolYearId, int personId, int roleId, DateTime startDate, DateTime endDate, int? classId,
            bool filterByExpiryDate)
        {
            DoUpdate(u => new AnnouncementRecipientDataDataAccess(u).UpdateAnnouncementRecipientData(null, AnnouncementTypeEnum.Supplemental,
              schoolYearId, personId, roleId, false, classId, startDate, endDate, filterByExpiryDate));
        }

        public AnnouncementDetails Create(int classId, DateTime expiresDate, int classAnnouncementTypeId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateSupplementalAnnouncementDataAccess(u).Create(classId, Context.NowSchoolTime, expiresDate, Context.PersonId.Value, classAnnouncementTypeId));
        }

        public static void ValidateSupplementalAnnouncement(SupplementalAnnouncement supplemental, SupplementalAnnouncementDataAccess da, IServiceLocatorSchool serviceLocator,
            IList<int> recipientsIds, int classId)
        {
            if (string.IsNullOrEmpty(supplemental.Title))
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Supplemented Announcement Title "));

            if(recipientsIds == null || recipientsIds.Count == 0)
                throw new ChalkableException("You can't create Supplemented Announcements without students");

            var classStudents = serviceLocator.ClassService.GetClassPersons(null, classId, true, null);
            if (!recipientsIds.All(x => classStudents.Any(y => y.PersonRef == x)))
                throw new ChalkableException("You can create Supplemented Announcements only for students of current class");
        }

        public AnnouncementDetails Edit(int supplementalAnnouncementPlanId, int classId, int? classAnnouncementTypeId, string title,
            string content, DateTime? expiresDate, bool visibleForStudent, IList<int> recipientsIds)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var suppAnnouncement = InternalGetAnnouncementById(supplementalAnnouncementPlanId);
            using (var uow = Update())
            {
                var da = CreateSupplementalAnnouncementDataAccess(uow);
                AnnouncementSecurity.EnsureInModifyAccess(suppAnnouncement, Context);

                if (suppAnnouncement.ClassRef != classId)
                {
                    if(!suppAnnouncement.IsDraft)
                        throw new ChalkableException("Class can't be changed for submited supplemental announcements");

                    suppAnnouncement.ClassRef = classId;
                    new AnnouncementApplicationDataAccess(uow).DeleteByAnnouncementId(suppAnnouncement.Id);
                    new AnnouncementStandardDataAccess(uow).DeleteNotAssignedToClass(suppAnnouncement.Id, classId);
                }

                suppAnnouncement.Title = title;
                suppAnnouncement.Content = content;
                suppAnnouncement.Expires = expiresDate ?? suppAnnouncement.Expires;
                suppAnnouncement.VisibleForStudent = visibleForStudent;

                if (suppAnnouncement.IsSubmitted)
                    ValidateSupplementalAnnouncement(suppAnnouncement, da, ServiceLocator, recipientsIds, classId);

                da.Update(suppAnnouncement);
                uow.Commit();
                return InternalGetDetails(da, supplementalAnnouncementPlanId);
            }
        }

        public void SetVisibleForStudent(int supplementalAnnouncementPlanId, bool visible)
        {
            var suppAnnouncement = GetSupplementalAnnouncementById(supplementalAnnouncementPlanId);
            AnnouncementSecurity.EnsureInModifyAccess(suppAnnouncement, Context);
            if (suppAnnouncement.VisibleForStudent == visible)
                return;

            suppAnnouncement.VisibleForStudent = visible;
            DoUpdate(u => CreateSupplementalAnnouncementDataAccess(u).Update(suppAnnouncement));
        }

        public SupplementalAnnouncement GetSupplementalAnnouncementById(int supplementalAnnouncementPlanId)
        {
            return InternalGetAnnouncementById(supplementalAnnouncementPlanId);
        }

        public IList<SupplementalAnnouncement> GetSupplementalAnnouncement(DateTime? fromDate, DateTime? toDate, int? classId, int? teacherId,
            bool filterByStartDate = true)
        {
            throw new NotImplementedException();
        }

        public IList<AnnouncementComplex> GetSupplementalAnnouncementForFeed(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete,
            int start = 0, int count = Int32.MaxValue, bool? ownedOnly = null)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string title, int? excludeSupplementalAnnouncementPlanId)
        {
            throw new NotImplementedException();
        }
    }
}