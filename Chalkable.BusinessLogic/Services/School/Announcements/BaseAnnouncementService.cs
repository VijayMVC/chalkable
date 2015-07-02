using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface IBaseAnnouncementService 
    {
        Announcement GetAnnouncementById(int id);
        AnnouncementDetails GetAnnouncementDetails(int announcementId);
        void DeleteAnnouncement(int announcementId);
        void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft);
        Announcement EditTitle(int announcementId, string title);
        //Announcement GetLastDraft();
        void Submit(int announcementId);

        int GetNewAnnouncementItemOrder(AnnouncementDetails announcement);
        void SetComplete(int id, bool complete);
        void SetAnnouncementsAsComplete(DateTime? date, bool complete);
        bool CanAddStandard(int announcementId);

        Standard AddAnnouncementStandard(int announcementId, int standardId);
        Standard RemoveStandard(int announcementId, int standardId);
        void RemoveAllAnnouncementStandards(int standardId);
        IList<AnnouncementStandard> GetAnnouncementStandards(int classId);
        IList<Person> GetAnnouncementRecipientPersons(int announcementId);

    }


    public abstract class BaseAnnouncementService :  SisConnectedService, IBaseAnnouncementService
    {
        protected BaseAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public abstract Announcement GetAnnouncementById(int id);
        public abstract AnnouncementDetails GetAnnouncementDetails(int announcementId);
        public abstract void DeleteAnnouncement(int announcementId);
        public abstract void DeleteAnnouncements(int schoolpersonid, AnnouncementState state = AnnouncementState.Draft);
        public abstract Announcement EditTitle(int announcementId, string title);
        public abstract void Submit(int announcementId);
        public abstract void SetAnnouncementsAsComplete(DateTime? date, bool complete);
        public abstract IList<Person> GetAnnouncementRecipientPersons(int announcementId);
        public abstract bool CanAddStandard(int announcementId);
        

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
            if (!ann.IsDraft)
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
    }
}
