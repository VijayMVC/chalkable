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
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface ISupplementalAnnouncementService : IBaseAnnouncementService
    {
        AnnouncementDetails Create(int classId, DateTime expiresDate, int classAnnouncementTypeId);
        AnnouncementDetails Edit(int supplementalAnnouncementId, int classId, int? classAnnouncementTypeId, string title, string content, DateTime? expiresDate, bool visibleForStudent, IList<int> recipientsIds, bool discussionEnabled, bool previewCommentsEnabled, bool requireCommentsEnabled);
        void SetVisibleForStudent(int supplementalAnnouncementId, bool visible);
        SupplementalAnnouncement GetSupplementalAnnouncementById(int supplementalAnnouncementId);
        IList<AnnouncementComplex> GetSupplementalAnnouncementsSortedByDate(DateTime? fromDate, DateTime? toDate, bool includeFromDate, bool includeToDate, int? classId, int start = 0, int count = int.MaxValue, bool sortDesc = false, bool? ownedOnly = null);
        IList<AnnouncementComplex> GetSupplementalAnnouncementSortedByTitle(DateTime? fromDate, DateTime? toDate, string fromTitle, string toTitle, bool includeFromTitle, bool includeToTitle, int? classId, int start = 0, int count = int.MaxValue, bool sortDesc = false, bool? ownedOnly = null);
        IList<AnnouncementComplex> GetSupplementalAnnouncementSortedByClassName(DateTime? fromDate, DateTime? toDate, string fromClassName, string toClassName, bool includeFromClassName, bool includeToClassName, int? classId, int start = 0, int count = int.MaxValue, bool sortDesc = false, bool? ownedOnly = null);
        IList<SupplementalAnnouncement> GetSupplementalAnnouncements(DateTime? fromDate, DateTime? toDate, int? classId, int? studentId, int? teacherId);
        bool Exists(string title, int? excludeSupplementalAnnouncementId);
        SupplementalAnnouncement GetLastDraft();
    }

    public class SupplementalAnnouncementService : BaseAnnouncementService<SupplementalAnnouncement>, ISupplementalAnnouncementService
    {
        public SupplementalAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public override IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? startDate, DateTime? toDate, int? classId, bool? complete, bool ownerOnly = false)
        {
            throw new NotImplementedException();
        }

        public override IList<int> Copy(IList<int> classAnnouncementIds, int fromClassId, int toClassId, DateTime? startDate)
        {
            throw new NotImplementedException();
        }

        public override IList<AnnouncementComplex> GetAnnouncementsByIds(IList<int> announcementIds)
        {
            using (var u = Read())
            {
                var announcement =InternalGetDetailses(CreateSupplementalAnnouncementDataAccess(u), announcementIds)
                        .Cast<AnnouncementComplex>().ToList();
                var recipients = new SupplementalAnnouncementRecipientDataAccess(u).GetRecipientsByAnnouncementIds(announcementIds);

                foreach (var announcementComplex in announcement)
                {
                    announcementComplex.SupplementalAnnouncementData.Recipients =recipients
                        .Where(x => x.SupplementalAnnouncementRef == announcementComplex.Id)
                        .Select(x => x.Recipient).ToList();
                }

                return announcement;
            }
        }

        public override void DeleteAnnouncement(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = CreateSupplementalAnnouncementDataAccess(uow);
                var announcement = da.GetAnnouncement(announcementId, Context.PersonId.Value);

                new SupplementalAnnouncementRecipientDataAccess(uow).DeleteByAnnouncementId(announcementId);

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
                var recipients = new SupplementalAnnouncementRecipientDataAccess(u).GetRecipientsByAnnouncementId(suppAnnouncement.Id);
                AnnouncementSecurity.EnsureInModifyAccess(res, Context);

                ValidateSupplementalAnnouncement(suppAnnouncement, da, ServiceLocator, recipients.Select(x => x.StudentRef).ToList(), suppAnnouncement.ClassRef);

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

        protected SupplementalAnnouncementDataAccess CreateSupplementalAnnouncementDataAccess(UnitOfWork unitOfWork, bool? ownedOnly = null)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);

            Trace.Assert(Context.SchoolYearId.HasValue);
            if (BaseSecurity.IsDistrictOrTeacher(Context))
            {
                if (Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN) || Context.Claims.HasPermission(ClaimInfo.MAINTAIN_CLASSROOM_ADMIN))
                    return new SupplementalAnnouncementForAdminDataAccess(unitOfWork, Context.SchoolYearId.Value, ownedOnly);
                if (Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM) || Context.Claims.HasPermission(ClaimInfo.MAINTAIN_CLASSROOM))
                    return new SupplementalAnnouncementForTeacherDataAccess(unitOfWork, Context.SchoolYearId.Value);
            }
            if (Context.Role == CoreRoles.STUDENT_ROLE)
                return new SupplementalAnnouncementForStudentDataAccess(unitOfWork, Context.SchoolYearId.Value);

            throw new ChalkableException("Not supported role for lesson plan");
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

        public AnnouncementDetails Edit(int supplementalAnnouncementId, int classId, int? classAnnouncementTypeId, string title,
            string content, DateTime? expiresDate, bool visibleForStudent, IList<int> recipientsIds
            , bool discussionEnabled, bool previewCommentsEnabled, bool requireCommentsEnabled)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var suppAnnouncement = InternalGetAnnouncementById(supplementalAnnouncementId);
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
                suppAnnouncement.ClassAnnouncementTypeRef = classAnnouncementTypeId;

                suppAnnouncement.DiscussionEnabled = discussionEnabled;
                suppAnnouncement.RequireCommentsEnabled = requireCommentsEnabled;

                if (previewCommentsEnabled && discussionEnabled && !suppAnnouncement.PreviewCommentsEnabled)
                {
                    suppAnnouncement.PreviewCommentsEnabled = true;
                    if(suppAnnouncement.IsSubmitted)
                        new AnnouncementCommentDataAccess(uow).HideAll(suppAnnouncement.Id);
                }
                
                if (suppAnnouncement.IsSubmitted)
                    ValidateSupplementalAnnouncement(suppAnnouncement, da, ServiceLocator, recipientsIds, classId);

                new SupplementalAnnouncementRecipientDataAccess(uow).UpdateRecipients(supplementalAnnouncementId, recipientsIds);
                da.Update(suppAnnouncement);
                uow.Commit();
                return InternalGetDetails(da, supplementalAnnouncementId);
            }
        }

        protected override AnnouncementDetails InternalGetDetails(BaseAnnouncementDataAccess<SupplementalAnnouncement> dataAccess, int announcementId)
        {
            var res = base.InternalGetDetails(dataAccess, announcementId);
            res.AnnouncementData = PrepareClassAnnouncementTypeData(res.SupplementalAnnouncementData);
            return res;
        }

        protected override IList<AnnouncementDetails> InternalGetDetailses(BaseAnnouncementDataAccess<SupplementalAnnouncement> dataAccess, IList<int> announcementIds, bool onlyOnwer = true)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var anns = dataAccess.GetDetailses(announcementIds, Context.PersonId.Value, Context.Role.Id, onlyOnwer);
            if (anns == null)
                return null;

            return PrepareRecipientsData(anns);
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

        protected override SupplementalAnnouncement InternalGetAnnouncementById(int id)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var unitOfWork = Read())
            {
                var announcement = CreateSupplementalAnnouncementDataAccess(unitOfWork).GetAnnouncement(id, Context.PersonId.Value);
                announcement.Recipients = new SupplementalAnnouncementRecipientDataAccess(unitOfWork).GetRecipientsByAnnouncementId(id)
                    .Select(x => x.Recipient).ToList();
                return announcement;
            }
        }

        public SupplementalAnnouncement GetSupplementalAnnouncementById(int supplementalAnnouncementPlanId)
        {
            return InternalGetAnnouncementById(supplementalAnnouncementPlanId);
        }

       public IList<AnnouncementComplex> GetSupplementalAnnouncementsSortedByDate(DateTime? fromDate, DateTime? toDate, bool includeFromDate,
            bool includeToDate, int? classId, int start = 0, int count = int.MaxValue, bool sortDesc = false,
            bool? ownedOnly = null)
        {

            return GetSupplementalAnnouncements(new SupplementalAnnouncementQuery
            {
                ClassId = classId,
                Start = start,
                Count = count,
                FromDate = fromDate,
                ToDate = toDate,
                PersonId = Context.PersonId,
                RoleId = Context.RoleId,
                IncludeFrom = includeFromDate,
                IncludeTo = includeToDate,
                Sort = sortDesc
            }, (da, q) => da.GetSupplementalAnnouncementOrderedByDate(q));
        }

        public IList<AnnouncementComplex> GetSupplementalAnnouncementSortedByTitle(DateTime? fromDate, DateTime? toDate, string fromTitle, string toTitle,
            bool includeFromTitle, bool includeToTitle, int? classId, int start = 0, int count = int.MaxValue,
            bool sortDesc = false, bool? ownedOnly = null)
        {

            return GetSupplementalAnnouncements(new SupplementalAnnouncementQuery
            {
                ClassId = classId,
                Start = start,
                Count = count,
                FromDate = fromDate,
                ToDate = toDate,
                PersonId = Context.PersonId,
                RoleId = Context.RoleId,
                IncludeFrom = includeFromTitle,
                IncludeTo = includeToTitle,
                Sort = sortDesc
            }, (da, q) => da.GetSupplementalAnnouncementOrderedByTitle(q));
        }

        public IList<AnnouncementComplex> GetSupplementalAnnouncementSortedByClassName(DateTime? fromDate, DateTime? toDate, string fromClassName,
            string toClassName, bool includeFromClassName, bool includeToClassName, int? classId, int start = 0,
            int count = int.MaxValue, bool sortDesc = false, bool? ownedOnly = null)
        {

            return GetSupplementalAnnouncements(new SupplementalAnnouncementQuery
            {
                ClassId = classId,
                Start = start,
                Count = count,
                FromDate = fromDate,
                ToDate = toDate,
                PersonId = Context.PersonId,
                RoleId = Context.RoleId,
                IncludeFrom = includeFromClassName,
                IncludeTo = includeToClassName,
                FromClassName = fromClassName,
                ToClassName = toClassName,
                Sort = sortDesc
            }, (da, q)=> da.GetSupplementalAnnouncementOrderedByClassName(q));
        }

        public IList<SupplementalAnnouncement> GetSupplementalAnnouncements(DateTime? fromDate, DateTime? toDate, int? classId, int? studentId, int? teacherId)
        {
            return GetSupplementalAnnouncements(new SupplementalAnnouncementQuery
            {
                ClassId = classId,
                FromDate = fromDate,
                ToDate = toDate,
                PersonId = Context.PersonId,
                RoleId = Context.RoleId,
                IncludeFrom = true,
                IncludeTo = true,
                Sort = false,
                TeacherId = teacherId,
                StudentId = studentId
            }, (da, q) => da.GetSupplementalAnnouncementOrderedByDate(q))
            .Select(x=>x.SupplementalAnnouncementData).ToList();
        }


        private IList<AnnouncementComplex> GetSupplementalAnnouncements(SupplementalAnnouncementQuery query,
            Func<SupplementalAnnouncementDataAccess, SupplementalAnnouncementQuery, AnnouncementQueryResult> getAnnsMethod)
        {
            var anns = DoRead(u => getAnnsMethod(CreateSupplementalAnnouncementDataAccess(u), query).Announcements);
            var classIds = anns.Where(x=>x.ClassRef.HasValue).Select(x => x.ClassRef.Value).Distinct().ToList();
            var types =  ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classIds);
            foreach (var ann in anns)
            {
                var type = types.FirstOrDefault(x => x.Id == ann.SupplementalAnnouncementData.ClassAnnouncementTypeRef);
                ann.SupplementalAnnouncementData.ClassAnnouncementTypeName = type?.Name;
                ann.SupplementalAnnouncementData.ChalkableAnnouncementType = type?.ChalkableAnnouncementTypeRef;
            }
            return PrepareRecipientsData(anns);
        }

        private IList<TAnnouncement> PrepareRecipientsData<TAnnouncement>(IList<TAnnouncement> anns) where TAnnouncement : AnnouncementComplex
        {
            if (BaseSecurity.IsDistrictOrTeacher(Context))
            {
                var recipients = DoRead(u => new SupplementalAnnouncementRecipientDataAccess(u).GetRecipientsByAnnouncementIds(anns.Select(x => x.Id).ToList()));
                foreach (var announcementDetailse in anns)
                {
                    announcementDetailse.SupplementalAnnouncementData.Recipients = recipients
                        .Where(x => x.SupplementalAnnouncementRef == announcementDetailse.Id)
                        .Select(x => x.Recipient).ToList();
                }
            }
            return anns;
        } 

        private SupplementalAnnouncement PrepareClassAnnouncementTypeData(SupplementalAnnouncement supplementalAnnouncement)
        {
            if (supplementalAnnouncement.ClassAnnouncementTypeRef.HasValue)
            {
                if (string.IsNullOrEmpty(supplementalAnnouncement.ClassAnnouncementTypeName))
                {
                    var classAnnType = ServiceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypeById(supplementalAnnouncement.ClassAnnouncementTypeRef.Value);
                    supplementalAnnouncement.ClassAnnouncementTypeName = classAnnType.Name;
                    supplementalAnnouncement.ChalkableAnnouncementType = classAnnType.ChalkableAnnouncementTypeRef;
                }
                else
                {
                    var chlkAnnType = ServiceLocator.ClassAnnouncementTypeService.GetChalkableAnnouncementTypeByAnnTypeName(supplementalAnnouncement.ClassAnnouncementTypeName);
                    supplementalAnnouncement.ChalkableAnnouncementType = chlkAnnType?.Id;
                }
            }
            return supplementalAnnouncement;
        }


        public bool Exists(string title, int? excludeSupplementalAnnouncementPlanId)
        {
            throw new NotImplementedException();
        }

        public SupplementalAnnouncement GetLastDraft()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            using (var unitOfWork = Read())
            {
                var announcement = CreateSupplementalAnnouncementDataAccess(unitOfWork).GetLastDraft(Context.PersonId.Value);
                if(announcement != null)
                    announcement.Recipients = new SupplementalAnnouncementRecipientDataAccess(unitOfWork).GetRecipientsByAnnouncementId(announcement.Id)
                        .Select(x => x.Recipient).ToList();
                return announcement;
            }
        }
    }
}