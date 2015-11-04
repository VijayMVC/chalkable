﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
    public enum MarkDoneOptions
    {
        TillToday = 1,
        Till30Days,
        All
    }

    public interface IBaseAnnouncementService 
    {
        Announcement GetAnnouncementById(int id);
        AnnouncementDetails GetAnnouncementDetails(int announcementId);
        void DeleteAnnouncement(int announcementId);
        void DeleteDrafts(int schoolpersonid);
        Announcement EditTitle(int announcementId, string title);
        void Submit(int announcementId);

        int GetNewAnnouncementItemOrder(AnnouncementDetails announcement);
        void SetComplete(int id, bool complete);
        void SetComplete(int? classId, MarkDoneOptions option);
        void SetAnnouncementsAsComplete(DateTime? date, bool complete);
        bool CanAddStandard(int announcementId);

        Standard AddAnnouncementStandard(int announcementId, int standardId);
        Standard RemoveStandard(int announcementId, int standardId);
        void RemoveAllAnnouncementStandards(int standardId);
        IList<AnnouncementStandard> GetAnnouncementStandards(int classId);
        IList<Person> GetAnnouncementRecipientPersons(int announcementId);

    }


    public abstract class BaseAnnouncementService<TAnnouncement> : SisConnectedService, IBaseAnnouncementService where TAnnouncement : Announcement, new() 
    {
        protected BaseAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public abstract AnnouncementDetails GetAnnouncementDetails(int announcementId);
        public abstract void DeleteAnnouncement(int announcementId);

        public abstract Announcement GetAnnouncementById(int id);
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

        public IList<Person> GetAnnouncementRecipientPersons(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => CreateDataAccess(u).GetAnnouncementRecipientPersons(announcementId, Context.PersonId.Value));
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
        
        public Standard AddAnnouncementStandard(int announcementId, int standardId)
        {
            var ann = GetAnnouncementById(announcementId);
            AnnouncementSecurity.EnsureInModifyAccess(ann, Context);
            using (var uow = Update())
            {
                var annStandard = new AnnouncementStandard
                    {
                        AnnouncementRef = announcementId,
                        StandardRef = standardId
                    };
                new AnnouncementStandardDataAccess(uow).Insert(annStandard);
                AfterAddingStandard(ann, annStandard);
                uow.Commit();
                return new StandardDataAccess(uow).GetById(standardId);
            }
        }

        public Standard RemoveStandard(int announcementId, int standardId)
        {
            var ann = GetAnnouncementById(announcementId);
            if (!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new AnnouncementStandardDataAccess(uow).Delete(announcementId, standardId);
                AfterRemovingStandard(ann, standardId);
                uow.Commit();
                return new StandardDataAccess(uow).GetById(standardId);
            }
        }

        protected virtual void AfterAddingStandard(Announcement announcement, AnnouncementStandard announcementStandard) {}
        protected virtual void AfterRemovingStandard(Announcement announcement, int standardId){}


        public void RemoveAllAnnouncementStandards(int standardId)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AnnouncementStandardDataAccess(u).DeleteAll(standardId));
        }

        public IList<AnnouncementStandard> GetAnnouncementStandards(int classId)
        {
            return DoRead(u => new AnnouncementStandardDataAccess(u).GetAnnouncementStandardsByClassId(classId));
        }

        public void SetComplete(int? classId, MarkDoneOptions option)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            DateTime? tillDateToUpdate;
            switch (option)
            {
                case MarkDoneOptions.Till30Days:
                    tillDateToUpdate = Context.NowSchoolTime.AddMonths(-1);
                    break;
                case MarkDoneOptions.TillToday:
                    tillDateToUpdate = Context.NowSchoolTime.AddDays(-1);
                    break;
                default:
                    tillDateToUpdate = GetEndDateFromFeedSettings();
                    break;

            }
            SetComplete(Context.SchoolYearId.Value, Context.PersonId.Value, Context.RoleId, tillDateToUpdate, classId);
        }

        private DateTime? GetEndDateFromFeedSettings()
        {
            var feedEndDateSetting = ServiceLocator.PersonSettingService.GetSettingsForPerson(Context.PersonId.Value, Context.SchoolYearId.Value,
                new List<string> { PersonSetting.FEED_END_DATE });
            if (feedEndDateSetting.Count > 0 && !string.IsNullOrWhiteSpace(feedEndDateSetting[PersonSetting.FEED_END_DATE]))
                return DateTime.ParseExact(feedEndDateSetting[PersonSetting.FEED_END_DATE], Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
         
           return Context.SchoolYearEndDate;
        }

        protected abstract void SetComplete(int schoolYearId, int personId, int roleId, DateTime? tillDateToUpdate, int? classId);
    }
}
