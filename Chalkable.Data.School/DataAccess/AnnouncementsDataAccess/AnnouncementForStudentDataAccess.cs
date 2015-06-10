using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForStudentDataAccess : ClassAnnouncementDataAccess
    {
        public AnnouncementForStudentDataAccess(UnitOfWork unitOfWork, int schoolId) : base(unitOfWork, schoolId)
        {
        }
        
        private const string GET_STUDENT_ANNOUNCEMENTS = "spGetStudentAnnouncements";

        private const string COMPLETE = "complete";
        private const string ADMIN_ONLY = "adminOnly";

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {COMPLETE, query.Complete},
                    {ADMIN_ONLY, query.AdminOnly}
                };
            return GetAnnouncementsComplex(GET_STUDENT_ANNOUNCEMENTS, parameters, query);
        }
        protected override void BuildConditionForGetSimpleAnnouncement(Common.Orm.DbQuery dbQuery, int callerId)
        {
            dbQuery.Sql.Append(@" and (Announcement.ClassRef in (select cp.ClassRef from ClassPerson cp where cp.PersonRef = @callerId))");
            dbQuery.Parameters.Add("callerId", callerId);
        }
    }
}
