using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementApplicationStorage:BaseDemoIntStorage<AnnouncementApplication>
    {
        public DemoAnnouncementApplicationStorage()
            : base(x => x.Id, true)
        {
        }

        public IList<AnnouncementApplication> GetAll(int announcementId, Guid applicationId, bool active)
        {
            return
                data.Where(
                    x =>
                        x.Value.AnnouncementRef == announcementId && x.Value.ApplicationRef == applicationId &&
                        x.Value.Active == active).Select(x => x.Value).ToList();
        }

        public IList<AnnouncementApplication> GetAll(int announcementId, bool onlyActive)
        {
            var aa = data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Value);
            if (onlyActive)
                aa = aa.Where(x => x.Active);
            return aa.ToList();
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive)
        {
            var announcementApplications = data.Select(x => x.Value);

            announcementApplications = announcementApplications.Where(x =>
            {
                var announcement = StorageLocator.AnnouncementStorage.GetAnnouncementById(x.AnnouncementRef);
                return StorageLocator.ApplicationInstallStorage.Exists(x.ApplicationRef, personId)
                       && announcement.PrimaryTeacherRef == personId
                       || StorageLocator.ClassPersonStorage.ClassPersonExists(announcement.ClassRef, personId);
            });
            return announcementApplications.ToList();
        }

        public void DeleteByAnnouncementId(int announcementId)
        {
            var announcementAttachments = GetAll(announcementId, false);
            Delete(announcementAttachments);
        }

        public AnnouncementApplication GetAnnouncementApplication(int announcementAppId)
        {
            
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnIds(IList<int> announcementIds, bool onlyActive = false)
        {
            var res = GetAll()
                       .Where(x => announcementIds.Contains(x.AnnouncementRef));
            if (onlyActive)
                res = res.Where(x => x.Active);
            return res.ToList();
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByAnnId(int announcementId, bool onlyActive = false)
        {
            return GetAnnouncementApplicationsByAnnIds(new List<int> {announcementId}, onlyActive);
        }

        public void AttachAppToAnnouncement(int announcementAppId)
        {
            var aa = GetAnnouncementApplication(announcementAppId);
            var ann = StorageLocator.AnnouncementStorage.GetTeacherStorage()
                .GetAnnouncement(aa.AnnouncementRef, Context.Role.Id, Context.PersonId.Value);
            var c = StorageLocator.ClassStorage.GetById(ann.ClassRef);
            if (Context.PersonId != c.PrimaryTeacherRef)
                throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
            aa.Active = true;
            Update(aa);
        }

        public Announcement RemoveFromAnnouncement(int announcementAppId)
        {
            try
            {
                var aa = StorageLocator.AnnouncementApplicationStorage.GetAnnouncementApplication(announcementAppId);
                StorageLocator.AnnouncementApplicationStorage.Delete(announcementAppId);
                var res = StorageLocator.AnnouncementStorage.GetAnnouncementById(aa.AnnouncementRef);
                var c = StorageLocator.ClassStorage.GetById(res.ClassRef);
                if (Context.PersonId != c.PrimaryTeacherRef)
                    throw new ChalkableSecurityException(ChlkResources.ERR_SECURITY_EXCEPTION);
                return res;
            }
            catch (Exception)
            {
                throw new ChalkableException(String.Format(ChlkResources.ERR_CANT_DELETE_ANNOUNCEMENT_APPLICATION, announcementAppId));
            }
        }
    }
}
