using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface ISupplementalAnnouncementService : IBaseAnnouncementService
    {
        AnnouncementDetails Create(int classId, DateTime expiresDate);
        AnnouncementDetails Edit(int supplementalAnnouncementPlanId, int classId, int? galleryCategoryId, string title, string content, DateTime? expiresDate, bool visibleForStudent);
        void SetVisibleForStudent(int supplementalAnnouncementPlanId, bool visible);
        SupplementalAnnouncement GetSupplementalAnnouncementById(int supplementalAnnouncementPlanId);
        void SubmitRecipientsToSupplementalAnnouncement(int supplementalAnnouncementPlanId, IList<int> recipientsIds);
        IList<SupplementalAnnouncement> GetSupplementalAnnouncement(DateTime? fromDate, DateTime? toDate, int? classId, int? teacherId, bool filterByStartDate = true);
        IList<AnnouncementComplex> GetSupplementalAnnouncementForFeed(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, int start = 0, int count = int.MaxValue, bool? ownedOnly = null);
        bool Exists(string title, int? excludeSupplementalAnnouncementPlanId);
        bool ExistsInGallery(string title, int? excludeSupplementalAnnouncementPlanId);
        void ReplaceSupplementalAnnouncementInGallery(int oldSupplementalAnnouncementId, int newSupplementalAnnouncementId);
        void RemoveFromGallery(int supplementalAnnouncementId);
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
            throw new NotImplementedException();
        }

        public override void DeleteAnnouncement(int announcementId)
        {
            throw new NotImplementedException();
        }

        public override Announcement EditTitle(int announcementId, string title)
        {
            throw new NotImplementedException();
        }

        public override void Submit(int announcementId)
        {
            throw new NotImplementedException();
        }

        public override void SetAnnouncementsAsComplete(DateTime? date, bool complete)
        {
            throw new NotImplementedException();
        }

        public override bool CanAddStandard(int announcementId)
        {
            throw new NotImplementedException();
        }

        protected override BaseAnnouncementDataAccess<SupplementalAnnouncement> CreateDataAccess(UnitOfWork unitOfWork)
        {
            throw new NotImplementedException();
        }

        protected override void SetComplete(Announcement announcement, bool complete)
        {
            throw new NotImplementedException();
        }

        protected override void SetComplete(int schoolYearId, int personId, int roleId, DateTime startDate, DateTime endDate, int? classId,
            bool filterByExpiryDate)
        {
            throw new NotImplementedException();
        }

        protected override void SetUnComplete(int schoolYearId, int personId, int roleId, DateTime startDate, DateTime endDate, int? classId,
            bool filterByExpiryDate)
        {
            throw new NotImplementedException();
        }

        public AnnouncementDetails Create(int classId, DateTime expiresDate)
        {
            throw new NotImplementedException();
        }

        public AnnouncementDetails Edit(int supplementalAnnouncementPlanId, int classId, int? galleryCategoryId, string title,
            string content, DateTime? expiresDate, bool visibleForStudent)
        {
            throw new NotImplementedException();
        }

        public void SetVisibleForStudent(int supplementalAnnouncementPlanId, bool visible)
        {
            throw new NotImplementedException();
        }

        public SupplementalAnnouncement GetSupplementalAnnouncementById(int supplementalAnnouncementPlanId)
        {
            throw new NotImplementedException();
        }

        public void SubmitRecipientsToSupplementalAnnouncement(int supplementalAnnouncementPlanId, IList<int> recipientsIds)
        {
            throw new NotImplementedException();
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

        public bool ExistsInGallery(string title, int? excludeSupplementalAnnouncementPlanId)
        {
            throw new NotImplementedException();
        }

        public void ReplaceSupplementalAnnouncementInGallery(int oldSupplementalAnnouncementId, int newSupplementalAnnouncementId)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromGallery(int supplementalAnnouncementId)
        {
            throw new NotImplementedException();
        }
    }
}