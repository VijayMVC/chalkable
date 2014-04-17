using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementApplicationStorage:BaseDemoStorage<int ,AnnouncementApplication>
    {
        public DemoAnnouncementApplicationStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(AnnouncementApplication aa)
        {
            var id = GetNextFreeId();
            aa.Id = id;
            data.Add(aa.Id, aa);
        }

        public IList<AnnouncementApplication> GetAll(int announcementId, Guid applicationId, bool active)
        {
            return
                data.Where(
                    x =>
                        x.Value.AnnouncementRef == announcementId && x.Value.ApplicationRef == applicationId &&
                        x.Value.Active == active).Select(x => x.Value).ToList();
        }

        public void Update(AnnouncementApplication aa)
        {
            if (data.ContainsKey(aa.Id))
                data[aa.Id] = aa;
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
                       && announcement.PersonRef == personId
                       || Storage.ClassPersonStorage.Exists(new ClassPersonQuery
                       {
                           ClassId = announcement.ClassRef,
                           PersonId = personId
                       });
            });
            return announcementApplications.ToList();
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
