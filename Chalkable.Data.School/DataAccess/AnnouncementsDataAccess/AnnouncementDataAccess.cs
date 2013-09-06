using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public abstract class AnnouncementDataAccess : DataAccessBase<Announcement>
    {
        protected AnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        private const string CREATE_PORCEDURE = "spCreateAnnouncement";
        private const string GET_DETAILS_PROCEDURE = "spGetAnnouncementDetails";
        private const string GET_ANNOUNCEMENT_RECIPIENT_PERSON = "spGetAnnouncementRecipientPersons";
        private const string DELETE_PROCEDURE = "spDeleteAnnouncement";
        private const string REORDER_PROCEDURE = "spReorderAnnouncements";

        private const string ANNOUNCEMENT_TYPE_ID_PARAM = "announcementTypeId";
        private const string CREATED_PARAM = "created";
        private const string EXPIRES_PARAM = "expires";
        private const string PERSON_ID_PARAM = "personId";
        private const string STATE_PARAM = "state";
        private const string GRADING_STYLE_PARAM = "gradingStyle";
        private const string CLASS_ID_PARAM = "classId";
        private const string MARKING_PERIOD_ID_PARAM = "markingPeriodId";

        private const string OWNER_ID_PARAM = "ownerId";
        private const string SCHOOL_YEAR_ID_PARAM = "schoolYearId";
        private const string ID_PARAM = "id";
        private const string CALLER_ID_PARAM = "callerId";
        private const string ROLE_ID_PARAM = "roleId";
        private const string FROM_DATE_PARAM = "fromDate";
        private const string TO_DATE_PARAM = "toDate";
        
        private AnnouncementDetails BuildGetDetailsResult(SqlDataReader reader)
        {
            var announcement = reader.ReadOrNull<AnnouncementDetails>();
            if (announcement != null)
            {
                reader.NextResult();
                announcement.AnnouncementQnAs = AnnouncementQnADataAccess.ReadAnnouncementQnAComplexes(reader);
                reader.NextResult();
                announcement.StudentAnnouncements = StudentAnnouncementDataAccess.ReadListStudentAnnouncement(reader);
                reader.NextResult();
                announcement.AnnouncementAttachments = reader.ReadList<AnnouncementAttachment>();
                reader.NextResult();
                announcement.AnnouncementReminders = reader.ReadList<AnnouncementReminder>();
                reader.NextResult();
                announcement.Owner = PersonDataAccess.ReadPersonQueryResult(reader).Persons.FirstOrDefault();
                announcement.AnnouncementApplications = new List<AnnouncementApplication>();
            }
            return announcement;
        }
        protected AnnouncementQueryResult GetAnnouncementsComplex(string procedureName, Dictionary<string, object> parameters, AnnouncementsQuery query)
        {
            parameters.Add(ID_PARAM, query.Id);
            parameters.Add(ROLE_ID_PARAM, query.RoleId);
            parameters.Add(PERSON_ID_PARAM, query.PersonId);
            parameters.Add(MARKING_PERIOD_ID_PARAM, query.MarkingPeriodId);
            parameters.Add(FROM_DATE_PARAM, query.FromDate);
            parameters.Add(TO_DATE_PARAM, query.ToDate);
            parameters.Add(CLASS_ID_PARAM, query.ClassId);
            parameters.Add("start", query.Start);
            parameters.Add("count", query.Count);
            parameters.Add("now", query.Now);
            parameters.Add("ownedOnly", query.OwnedOnly);
            parameters.Add("staredOnly", query.StarredOnly);
            //parameters.Add();
            using (var reader = ExecuteStoredProcedureReader(procedureName, parameters))
            {
                return ReadAnnouncementsQueryResult(reader, query);
            }
        }

        protected AnnouncementQueryResult ReadAnnouncementsQueryResult(DbDataReader reader, AnnouncementsQuery query)
        {
            var res = new AnnouncementQueryResult { Query = query };
            bool first = true;
            while (reader.Read())
            {
                res.Announcements.Add(reader.Read<AnnouncementComplex>());
                if (first)
                {
                    res.SourceCount = SqlTools.ReadInt32(reader, AnnouncementQueryResult.ALL_COUNT);
                    first = false;
                }
            }
            return res;
        }
   
        public AnnouncementDetails Create(int announcementTypeId, Guid? classId, Guid markingPeriodId, DateTime created, Guid personId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ANNOUNCEMENT_TYPE_ID_PARAM, announcementTypeId},
                    {PERSON_ID_PARAM, personId},
                    {CREATED_PARAM, created},
                    {EXPIRES_PARAM, DateTime.MinValue},
                    {STATE_PARAM, AnnouncementState.Draft},
                    {GRADING_STYLE_PARAM, GradingStyleEnum.Numeric100},
                    {CLASS_ID_PARAM, classId},
                    {MARKING_PERIOD_ID_PARAM, markingPeriodId}
                };

            using (var reader = ExecuteStoredProcedureReader(CREATE_PORCEDURE, parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }

        public void Delete(Guid? id, Guid? personId, Guid? classId, int? announcementTypeId, AnnouncementState? state)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ID_PARAM, id},
                    {PERSON_ID_PARAM, personId},
                    {CLASS_ID_PARAM, classId},
                    {ANNOUNCEMENT_TYPE_ID_PARAM, announcementTypeId},
                    {STATE_PARAM, state}
                };
            ExecuteStoredProcedureReader(DELETE_PROCEDURE, parameters).Dispose();
        }
        
        public abstract AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query);
        
        public void ReorderAnnouncements(Guid schoolYearId, int announcementTypeId, Guid ownerId, Guid recipientId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {SCHOOL_YEAR_ID_PARAM, schoolYearId},
                    {"annType", announcementTypeId},
                    {OWNER_ID_PARAM, ownerId},
                    {CLASS_ID_PARAM, recipientId}
                };
            using (ExecuteStoredProcedureReader(REORDER_PROCEDURE, parameters))
            {
            }
        }
 
        public AnnouncementDetails GetDetails(Guid id, Guid callerId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ID_PARAM, id},
                    {CALLER_ID_PARAM, callerId}
                };
            using (var reader = ExecuteStoredProcedureReader(GET_DETAILS_PROCEDURE, parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }
        
        public IList<Person> GetAnnouncementRecipientPersons(Guid announcementId, Guid callerId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {CALLER_ID_PARAM, callerId},
                    {"announcementId", announcementId}
                };
            using (var reader = ExecuteStoredProcedureReader(GET_ANNOUNCEMENT_RECIPIENT_PERSON, parameters))
            {
                var res = new List<Person>();
                while (reader.Read())
                {
                    res.Add(PersonDataAccess.ReadPersonData(reader));
                }
                return res;
            }
        } 
       
        public Announcement GetAnnouncement(Guid id, int roleId, Guid callerId)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append("select Announcement.* from Announcement ");
            dbQuery.Sql.Append(" where ");
            dbQuery.Sql.AppendFormat(" Announcement.{0} = @{0}", Announcement.ID_FIELD);
            dbQuery.Parameters.Add(Announcement.ID_FIELD, id);
            BuildConditionForGetSimpleAnnouncement(dbQuery, roleId, callerId);
            return ReadOneOrNull<Announcement>(dbQuery);
        }
        
        protected abstract void BuildConditionForGetSimpleAnnouncement(DbQuery dbQuery, int role, Guid callerId);
        
        public Announcement GetLastDraft(Guid personId)
        {
            var conds = new Dictionary<string, object>
                {
                    {Announcement.PERSON_REF_FIELD, personId},
                    {STATE_PARAM, AnnouncementState.Draft}
                };
            var sql = @"select top 1 * from Announcement 
                      where PersonRef = @PersonRef and State = @state
                      order by Created desc";
           return  ReadOneOrNull<Announcement>(new DbQuery (sql, conds));
        }

        public IList<string> GetLastFieldValues(Guid personId, Guid classId, int announcementType, int count)
        {
            var conds = new Dictionary<string, object>
                {
                    {"personId", personId},
                    {"classId", classId},
                    {"announcementTypeId", announcementType},
                    {"count", count}
                };
            var sql = @"select Announcement.Content as Content from Announcement
                        join MarkingPeriodClass on MarkingPeriodClass.Id = Announcement.MarkingPeriodClassRef 
                        where Announcement.PersonRef = @personId 
                              and Announcement.AnnouncementTypeRef = @announcementTypeId
                              and MarkingPeriodClass.ClassRef = @classId
                              and Announcement.Content is not null and Announcement.Content <> ''
                        order by Announcement.Id desc
                        OFFSET 0 ROWS FETCH NEXT @count ROWS ONLY";
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                var res = new List<string>();
                while (reader.Read())
                {
                    res.Add(SqlTools.ReadStringNull(reader, Announcement.CONTENT_FIELD));
                }
                return res;
            }

        }
    }

    public class AnnouncementsQuery
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public Guid? Id { get; set; }
        public int? RoleId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? Now { get; set; }
        public bool StarredOnly { get; set; }
        public bool OwnedOnly { get; set; }
        public bool GradedOnly { get; set; }
        public bool AllSchoolItems { get; set; }

        public IEnumerable<Guid> GradeLevelIds { get; set; }

        public AnnouncementsQuery()
        {
            Start = 0;
            Count = int.MaxValue;
        }
    }
    public class AnnouncementQueryResult
    {
        public List<AnnouncementComplex> Announcements { get; set; }
        public const string ALL_COUNT = "AllCount";
        public int SourceCount { get; set; }
        public AnnouncementsQuery Query { get; set; }

        public AnnouncementQueryResult()
        {
            Announcements = new List<AnnouncementComplex>();
            SourceCount = 0;
        }
    }
}
