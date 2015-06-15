using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

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

        public override Announcement GetAnnouncement(int id, int callerId)
        {
            return GetAnnouncements(new AnnouncementsQuery {Id = id, PersonId = callerId, Start = 0, Count = 1})
                .Announcements.FirstOrDefault();
        }

        public override Announcement GetLastDraft(int personId)
        {
            throw new System.NotImplementedException();
        }
    }
}
