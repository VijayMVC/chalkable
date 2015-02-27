using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementApplicationStorage:BaseDemoIntStorage<AnnouncementApplication>
    {
        public DemoAnnouncementApplicationStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
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

                var announcement =
                    Storage.AnnouncementStorage.GetById(
                        x.AnnouncementRef);

                return Storage.ApplicationInstallStorage.Exists(x.ApplicationRef, personId)
                       && announcement.PrimaryTeacherRef == personId
                       || Storage.ClassPersonStorage.Exists(new ClassPersonQuery
                       {
                           ClassId = announcement.ClassRef,
                           PersonId = personId
                       });
            });
            return announcementApplications.ToList();
        }

        public void DeleteByAnnouncementId(int announcementId)
        {
            var announcementAttachments = GetAll(announcementId, false);
            Delete(announcementAttachments);
        }
    }
}
