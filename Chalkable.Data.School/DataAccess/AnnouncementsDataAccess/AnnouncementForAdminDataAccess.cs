using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForAdminDataAccess : AnnouncementDataAccess
    {
        public AnnouncementForAdminDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string GET_ADMIN_ANNOUNCEMENTS = "spGetAdminAnnouncements";
        private const string GRADE_LEVELS_IDS_PARAM = "gradeLevelsIds";
        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object> { { GRADE_LEVELS_IDS_PARAM, query.GradeLevelIds } };
            return GetAnnouncementsComplex(GET_ADMIN_ANNOUNCEMENTS, parameters, query);
        }

        protected override void BuildConditionForGetSimpleAnnouncement(Common.Orm.DbQuery dbQuery, int role, Guid callerId)
        {
        }
    }
}
