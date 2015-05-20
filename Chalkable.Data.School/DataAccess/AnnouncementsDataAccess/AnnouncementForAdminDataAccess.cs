using System;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForAdminDataAccess : AnnouncementDataAccess
    {
        public AnnouncementForAdminDataAccess(UnitOfWork unitOfWork, int schoolId) : base(unitOfWork, schoolId)
        {
        }

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            throw new NotImplementedException();
        }

        protected override void BuildConditionForGetSimpleAnnouncement(DbQuery dbQuery, int role, int callerId)
        {
            throw new NotImplementedException();
        }

        public override Model.Announcement GetLastDraft(int personId)
        {
            throw new NotImplementedException();
            //return base.GetLastDraft(personId);
        }
    }
}
