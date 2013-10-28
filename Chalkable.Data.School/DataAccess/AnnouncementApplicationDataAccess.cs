using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementApplicationDataAccess : DataAccessBase<AnnouncementApplication, int>
    {
        public AnnouncementApplicationDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive)
        {
            var sql = string.Format(@"select AnnouncementApplication.* from 
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
        }
    }
}