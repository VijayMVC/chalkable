using System;
using System.Collections.Generic;
using System.Data.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentAnnouncementDataAccess : DataAccessBase<StudentAnnouncement>
    {
        public StudentAnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public void Update(Guid announcementId, bool drop)
        {
            var conds = new AndQueryCondition { { StudentAnnouncement.ANNOUNCEMENT_REF_FIELD_NAME, announcementId } };
            var updateParams = new Dictionary<string, object> { { StudentAnnouncement.DROPPED_FIELD, drop } };
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
                return ReadListStudentAnnouncement(reader);
            }
        }
        

        public static IList<StudentAnnouncementDetails> ReadListStudentAnnouncement(DbDataReader reader)
        {
            var res = new List<StudentAnnouncementDetails>();
            while (reader.Read())
            {
                res.Add(ReadStudentAnnouncement(reader));
            }
            return res;
        }
        public  static  StudentAnnouncementDetails ReadStudentAnnouncement(DbDataReader reader)
        {
            var res = reader.Read<StudentAnnouncementDetails>(true);
            res.Person = PersonDataAccess.ReadPersonData(reader);
            return res;
        }

        public IList<StudentAnnouncement> GetList(StudentAnnouncementQuery query)
        {
            return SelectMany<StudentAnnouncement>(BuildConditions(query));
        } 
        private QueryCondition BuildConditions(StudentAnnouncementQuery query)
        {
            var res = new AndQueryCondition();
            if(query.AnnouncementId.HasValue)
                res.Add(StudentAnnouncement.ANNOUNCEMENT_REF_FIELD_NAME, query.AnnouncementId);
            if (query.State.HasValue)
                res.Add(StudentAnnouncement.STATE_FIELD, query.State);
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
