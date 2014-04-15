using System;
using System.Collections.Generic;
using System.Linq;
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
            /*      var sql = string.Format(@"select AnnouncementApplication.* from 
                                        AnnouncementApplication
                                        join Announcement on AnnouncementApplication.AnnouncementRef = Announcement.Id
                                        where 
	                                        exists(select * from ApplicationInstall where ApplicationRef = AnnouncementApplication.ApplicationRef and PersonRef = @{0})
	                                        and
	                                        (Announcement.PersonRef = @{0}
	                                        or exists(select * from ClassPerson where PersonRef = @{0} and ClassRef = Announcement.ClassRef)
	                                        )
                                        ", "personId");
            if (onlyActive)
                sql += " and AnnouncementApplication.Active = 1";
            var ps = new Dictionary<string, object> {{"personId", personId}};
            return ReadMany<AnnouncementApplication>(new DbQuery (sql, ps));
            
            var announcementApplications = data.Select(x => x.Value).ToList();

            var announcements = new List<Announcement>();
            foreach (var announcementApplication in announcementApplications)
            {
                announcements.Add(Storage.AnnouncementStorage.GetById(announcementApplication.AnnouncementRef));
            }
            */
            return new List<AnnouncementApplication>();


            //var aa = data.Where(x => x.Value.)
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
