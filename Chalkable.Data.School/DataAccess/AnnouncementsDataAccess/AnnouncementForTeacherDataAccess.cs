using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using System.Linq;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForTeacherDataAccess : ClassAnnouncementDataAccess
    {
        public AnnouncementForTeacherDataAccess(UnitOfWork unitOfWork, int schoolId) : base(unitOfWork, schoolId)
        {
        }
        private const string GET_TEACHER_ANNOUNCEMENTS = "spGetTeacherAnnouncements";
        private const string GRADED_ONLY_PARAM = "gradedOnly";
        private const string ALL_SCHOOL_ITEMS_PARAM = "allSchoolItems";
        private const string SIS_ACTIVITIES_IDS_PARAM = "sisActivitiesIds";
        private const string NOW_PARAM = "now";
        private const string OWNED_ONLY_PARAM = "ownedOnly";
        private const string MARKING_PERIOD_ID_PARAM = "markingPeriodId";
        private const string ROLE_ID_PARAM = "roleId";

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ROLE_ID_PARAM, query.RoleId},
                    {MARKING_PERIOD_ID_PARAM, query.MarkingPeriodId},
                    {NOW_PARAM, query.Now},
                    {OWNED_ONLY_PARAM, query.OwnedOnly},
                    {GRADED_ONLY_PARAM, query.GradedOnly},
                    {ALL_SCHOOL_ITEMS_PARAM, query.AllSchoolItems},
                    {SIS_ACTIVITIES_IDS_PARAM, query.SisActivitiesIds != null ? query.SisActivitiesIds.Select(x => x.ToString()).JoinString(",") : null}
                };
            return GetAnnouncementsComplex(GET_TEACHER_ANNOUNCEMENTS, parameters, query);
        }
        protected override void BuildConditionForGetSimpleAnnouncement(DbQuery dbQuery, int callerId)
        {
            dbQuery.Sql.Append(@" and (Announcement.ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where ClassTeacher.PersonRef = @callerId))");
            dbQuery.Parameters.Add("callerId", callerId);
        }
    }
}
