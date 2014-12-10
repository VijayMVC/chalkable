using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using System.Linq;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForTeacherDataAccess : AnnouncementDataAccess
    {
        public AnnouncementForTeacherDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }
        private const string GET_TEACHER_ANNOUNCEMENTS = "spGetTeacherAnnouncements";
        private const string GRADED_ONLY_PARAM = "gradedOnly";
        private const string ALL_SCHOOL_ITEMS_PARAM = "allSchoolItems";

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {GRADED_ONLY_PARAM, query.GradedOnly},
                    {ALL_SCHOOL_ITEMS_PARAM, query.AllSchoolItems},
                    {"@sisActivitiesIds", query.SisActivitiesIds != null ? query.SisActivitiesIds.Select(x => x.ToString()).JoinString(",") : null}
                };
            return GetAnnouncementsComplex(GET_TEACHER_ANNOUNCEMENTS, parameters, query);
        }
        protected override void BuildConditionForGetSimpleAnnouncement(Common.Orm.DbQuery dbQuery, int role, int callerId)
        {
            dbQuery.Sql.Append(" and ");
            dbQuery.Sql.Append(@" (Announcement.ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where ClassTeacher.PersonRef = @callerId))");
            dbQuery.Parameters.Add("callerId", callerId);
            dbQuery.Parameters.Add("@roleId", role);
        }
    }
}
