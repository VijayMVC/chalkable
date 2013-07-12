using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentAnnouncementDataAccess : DataAccessBase<StudentAnnouncement>
    {
        public StudentAnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        private const string DROP_PARAM = "drop";

        public void Update(Guid announcementId, bool drop)
        {
            var conds = new Dictionary<string, object> { { StudentAnnouncement.ANNOUNCEMENT_REF_FIELD_NAME, announcementId } };
            var updateParams = new Dictionary<string, object> {{DROP_PARAM, drop}};
            SimpleUpdate<StudentAnnouncement>(updateParams, conds);
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncementsDetails(Guid announcementId, Guid personId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"announcementId", announcementId},
                    {"personId", personId},
                };
            using (var reader = ExecuteStoredProcedureReader("spGetStudentAnnouncementsForAnnouncement", parameters))
            {
                var res = new List<StudentAnnouncementDetails>();
                while (reader.Read())
                {
                    res.Add(ReadStudentAnnouncement(reader));
                }
                return res;
            }
        }
        
        public  static  StudentAnnouncementDetails ReadStudentAnnouncement(SqlDataReader reader)
        {
            var res = reader.Read<StudentAnnouncementDetails>(true);
            res.Person = PersonDataAccess.ReadPersonData(reader);
            return res;
        }

        public IList<StudentAnnouncement> GetList(StudentAnnouncementQuery query)
        {
            return SelectMany<StudentAnnouncement>(BuildConditions(query));
        } 
        private Dictionary<string, object> BuildConditions(StudentAnnouncementQuery query)
        {
            var res = new Dictionary<string, object>();
            if(query.AnnouncementId.HasValue)
                res.Add("announcementRef", query.AnnouncementId);
            if(query.State.HasValue)
                res.Add("state", query.State);

            return res;
        } 
    }

    public class StudentAnnouncementQuery
    {
        public Guid? AnnouncementId { get; set; }
        public Guid? MarkingPeriodClassId { get; set; }
        public StudentAnnouncementStateEnum? State { get; set; }     
    }
}
