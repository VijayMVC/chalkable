using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForAdminDataAccess : AnnouncementDataAccess
    {
        public AnnouncementForAdminDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }

        private const string GET_ADMIN_ANNOUNCEMENTS = "spGetAdminAnnouncements";
        private const string GRADE_LEVELS_IDS_PARAM = "gradeLevelsIds";
        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            string glIdsStr = null;
            if (query.GradeLevelIds != null && query.GradeLevelIds.Count > 0)
            {
                glIdsStr = query.GradeLevelIds.Select(x => x.ToString()).JoinString(","); 
            }
            var parameters = new Dictionary<string, object> {{GRADE_LEVELS_IDS_PARAM, glIdsStr}};
            return GetAnnouncementsComplex(GET_ADMIN_ANNOUNCEMENTS, parameters, query);
        }

        protected override void BuildConditionForGetSimpleAnnouncement(Common.Orm.DbQuery dbQuery, int role, int callerId)
        {
        }
    }
}
