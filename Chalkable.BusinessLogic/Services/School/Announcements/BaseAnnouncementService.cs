using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public enum MarkDoneOptions
    {
        TillToday = 1,
        Till30Days,
        All
    }

    public enum StudentFilterEnum
    {
        All,
        MySchoolOnly,
        MyStudentsOnly
    }

    public interface IBaseAnnouncementService 
    {
        Announcement GetAnnouncementById(int id);
        AnnouncementDetails GetAnnouncementDetails(int announcementId);
        IList<AnnouncementComplex> GetAnnouncementsByIds(IList<int> announcementIds); 
        void DeleteAnnouncement(int announcementId);
        void DeleteDrafts(int schoolpersonid);
        Announcement EditTitle(int announcementId, string title);
        void Submit(int announcementId);

        int GetNewAnnouncementItemOrder(AnnouncementDetails announcement);
        void SetComplete(int id, bool complete);
        void SetComplete(int? classId, MarkDoneOptions option, bool complete);
        void SetAnnouncementsAsComplete(DateTime? date, bool complete);
        bool CanAddStandard(int announcementId);

        IList<Standard> SubmitStandardsToAnnouncement(int announcementId, IList<int> standardsIds); 
        Standard AddAnnouncementStandard(int announcementId, int standardId);
        Standard RemoveStandard(int announcementId, int standardId);
        void RemoveAllAnnouncementStandards(int standardId);
        IList<AnnouncementStandard> GetAnnouncementStandards(int classId);
        IList<Person> GetAnnouncementRecipientPersons(int announcementId, StudentFilterEnum studentFilter, int start = 0, int count = int.MaxValue);
        IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? startDate, DateTime? toDate, int? classId, bool? complete, bool ownerOnly = false);
        IList<int> Copy(IList<int> classAnnouncementIds, int fromClassId, int toClassId, DateTime? startDate);

        void AdjustDates(IList<int> ids, DateTime startDate, int classId);
    }


    public abstract class BaseAnnouncementService<TAnnouncement> : SisConnectedService, IBaseAnnouncementService where TAnnouncement : Announcement, new() 
    {
        protected BaseAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public abstract IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? startDate, DateTime? toDate, int? classId, bool? complete, bool ownerOnly = false);
        public abstract IList<int> Copy(IList<int> classAnnouncementIds, int fromClassId, int toClassId, DateTime? startDate);
        public abstract void AdjustDates(IList<int> ids, DateTime startDate, int classId);
        public abstract IList<AnnouncementComplex> GetAnnouncementsByIds(IList<int> announcementIds);
        
        public abstract void DeleteAnnouncement(int announcementId);
        public abstract Announcement EditTitle(int announcementId, string title);
        public abstract void Submit(int announcementId);
        public abstract void SetAnnouncementsAsComplete(DateTime? date, bool complete);
        public abstract bool CanAddStandard(int announcementId);
        protected abstract BaseAnnouncementDataAccess<TAnnouncement> CreateDataAccess(UnitOfWork unitOfWork);

        public void DeleteDrafts(int personId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            if(!AnnouncementSecurity.CanDeleteAnnouncement(personId, Context))
                throw new ChalkableSecurityException();
            DoUpdate(u =>
            {
                var da = CreateDataAccess(u);
                var conds = new AndQueryCondition { { Announcement.STATE_FIELD, AnnouncementState.Draft } };
                var drafts = da.GetAnnouncements(conds, Context.PersonId.Value);
                da.Delete(drafts.Select(x => x.Id).ToList());
            });
        }

        public IList<Person> GetAnnouncementRecipientPersons(int announcementId, StudentFilterEnum studentFilter, int start, int count)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            return DoRead(u => CreateDataAccess(u)
                .GetAnnouncementRecipientPersons(announcementId, Context.PersonId.Value, (int)studentFilter, Context.SchoolYearId.Value, start, count));
        }

        
        public int GetNewAnnouncementItemOrder(AnnouncementDetails announcement)
        {
            var attOrder = announcement.AnnouncementAttachments.Max(x => (int?)x.Order);
            var appOrder = announcement.AnnouncementApplications.Max(x => (int?)x.Order);
            var order = 0;
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

        public void SetComplete(int id, bool complete)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var ann = GetAnnouncementById(id);
            if (!ann.IsSubmitted)
                throw new ChalkableException("Not created item can't be starred");

            SetComplete(ann, complete);
        }

        protected abstract void SetComplete(Announcement announcement, bool complete);


        public IList<Standard> SubmitStandardsToAnnouncement(int announcementId, IList<int> standardsIds)
        {
            var ann = InternalGetAnnouncementById(announcementId);
            AnnouncementSecurity.EnsureInModifyAccess(ann, Context);
            standardsIds = standardsIds ?? new List<int>();
            var annStandards = standardsIds.Select(sId => new AnnouncementStandard
            {
                AnnouncementRef = announcementId,
                StandardRef = sId
            }).ToList();
            using (var uow = Update())
            {
                var da = new AnnouncementStandardDataAccess(uow);
                da.Delete(announcementId, null);
                da.Insert(annStandards);
                AfterSubmitStandardsToAnnouncement(ann, standardsIds);
                uow.Commit();
                return new StandardDataAccess(uow).GetStandardsByIds(standardsIds);
            }
        }

        public Standard AddAnnouncementStandard(int announcementId, int standardId)
        {
            var ann = InternalGetAnnouncementById(announcementId);
            AnnouncementSecurity.EnsureInModifyAccess(ann, Context);
            var annStandard = new AnnouncementStandard
            {
                AnnouncementRef = announcementId,
                StandardRef = standardId
            };
            using (var uow = Update())
            {

                new AnnouncementStandardDataAccess(uow).Insert(annStandard);
                AfterAddingStandard(ann, annStandard);
                uow.Commit();
                return new StandardDataAccess(uow).GetById(standardId);
            }
        }

        public Standard RemoveStandard(int announcementId, int standardId)
        {
            var ann = InternalGetAnnouncementById(announcementId);
            AnnouncementSecurity.EnsureInModifyAccess(ann, Context);

            using (var uow = Update())
            {
                new AnnouncementStandardDataAccess(uow).Delete(announcementId, standardId);
                AfterRemovingStandard(ann, standardId);
                uow.Commit();
                return new StandardDataAccess(uow).GetById(standardId);
            }
        }

        protected virtual void AfterSubmitStandardsToAnnouncement(TAnnouncement announcement,  IList<int> standardsIds) {}
        protected virtual void AfterAddingStandard(TAnnouncement announcement, AnnouncementStandard announcementStandard) {}
        protected virtual void AfterRemovingStandard(TAnnouncement announcement, int standardId){}


        public void RemoveAllAnnouncementStandards(int standardId)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AnnouncementStandardDataAccess(u).Delete(null, standardId));
        }

        public IList<AnnouncementStandard> GetAnnouncementStandards(int classId)
        {
            return DoRead(u => new AnnouncementStandardDataAccess(u).GetAnnouncementStandardsByClassId(classId));
        }

        public void SetComplete(int? classId, MarkDoneOptions option, bool complete)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            DateTime toDate;
            DateTime fromDate;
            var filterByExpiryDate = false;
            GetDateRangeForMarking(out fromDate, out toDate);
            switch (option)
            {
                case MarkDoneOptions.Till30Days:
                    if(toDate > Context.NowSchoolTime.AddDays(-30))
                        toDate = Context.NowSchoolTime.AddDays(-30);
                    break;
                case MarkDoneOptions.TillToday:
                    if (toDate > Context.NowSchoolTime.AddDays(-1))
                        toDate = Context.NowSchoolTime.AddDays(-1);
                    filterByExpiryDate = true;
                    break;
            }
            if(fromDate <= toDate)
                SetComplete(Context.SchoolYearId.Value, Context.PersonId.Value, Context.RoleId, fromDate, toDate, classId, filterByExpiryDate, complete);
        }
        protected abstract void SetComplete(int schoolYearId, int personId, int roleId, DateTime startDate, DateTime endDate, int? classId, bool filterByExpiryDate, bool complete);
        
        private void GetDateRangeForMarking(out DateTime startDate, out DateTime endDate)
        {
            var feedSettings = ServiceLocator.AnnouncementFetchService.GetSettingsForFeed();

            if (feedSettings.FromDate.HasValue)
                startDate = feedSettings.FromDate.Value;
            else
                startDate = Context.SchoolYearStartDate ?? DateTime.MinValue;

            if (feedSettings.ToDate.HasValue)
                endDate = feedSettings.ToDate.Value;
            else
                endDate = Context.SchoolYearEndDate ?? DateTime.MaxValue;
        }

        public Announcement GetAnnouncementById(int id)
        {
            return InternalGetAnnouncementById(id);
        }

        protected virtual TAnnouncement InternalGetAnnouncementById(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u =>
            {
                var res = CreateDataAccess(u).GetAnnouncement(announcementId, Context.PersonId.Value);
                if (res == null)
                    throw new NoAnnouncementException();
                return res;
            });
        }

        public virtual AnnouncementDetails GetAnnouncementDetails(int announcementId)
        {
            return DoRead(u => InternalGetDetails(CreateDataAccess(u), announcementId));
        }

        protected virtual AnnouncementDetails InternalGetDetails(BaseAnnouncementDataAccess<TAnnouncement> dataAccess, int announcementId)
        {
            return InternalGetDetails(dataAccess, announcementId, true);
        }

        protected AnnouncementDetails InternalGetDetails(BaseAnnouncementDataAccess<TAnnouncement> dataAccess, int announcementId, bool onlyOwner)
        {
            var ann = InternalGetDetailses(dataAccess, new List<int> { announcementId }, onlyOwner).FirstOrDefault();
            if (ann == null)
                throw new NoAnnouncementException();
            return ann;
        }

        protected virtual IList<AnnouncementDetails> InternalGetDetailses(BaseAnnouncementDataAccess<TAnnouncement> dataAccess, IList<int> announcementIds, bool onlyOnwer = true)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var anns = dataAccess.GetDetailses(announcementIds, Context.PersonId.Value, Context.Role.Id, onlyOnwer);
            return anns;
        }

        protected DateTime CalculateStartDateForCopying(int classId)
        {
            var grs = ServiceLocator.GradingPeriodService.GetGradingPeriodsDetailsByClassId(classId);
            if (grs.Count == 0)
                throw new ChalkableException("Target class for copying hasn't any grading periods");

            return grs.Min(x => x.StartDate);
        }
    }
}
