using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public abstract class AnnouncementDataAccess : BaseSchoolDataAccess<Announcement>
    {
        protected AnnouncementDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }


        private const string CREATE_PORCEDURE = "spCreateAnnouncement";
        private const string GET_DETAILS_PROCEDURE = "spGetAnnouncementDetails";
        private const string GET_ANNOUNCEMENT_RECIPIENT_PERSON = "spGetAnnouncementRecipientPersons";
        private const string DELETE_PROCEDURE = "spDeleteAnnouncement";
        private const string REORDER_PROCEDURE = "spReorderAnnouncements";

        private const string ANNOUNCEMENT_TYPE_ID_PARAM = "announcementTypeId";
        private const string CLASS_ANNOUNCEMENT_TYPE_ID_PARAM = "classAnnouncementTypeId";
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
        private const string CALLER_ROLE_PARAM = "@callerRole";
        private const string ROLE_ID_PARAM = "roleId";
        private const string FROM_DATE_PARAM = "fromDate";
        private const string TO_DATE_PARAM = "toDate";

        private const string SCHOOL_ID = "schoolId";

        private AnnouncementDetails BuildGetDetailsResult(SqlDataReader reader)
        {
            var announcement = reader.ReadOrNull<AnnouncementDetails>();
            if (announcement != null)
            {
                announcement.IsScored = announcement.MaxScore > 0;
                reader.NextResult();
                announcement.AnnouncementQnAs = AnnouncementQnADataAccess.ReadAnnouncementQnAComplexes(reader);
                reader.NextResult();
                announcement.AnnouncementAttachments = reader.ReadList<AnnouncementAttachment>();
                reader.NextResult();
                announcement.AnnouncementApplications = reader.ReadList<AnnouncementApplication>();
                reader.NextResult();
                if(reader.Read())
                    announcement.Owner = PersonDataAccess.ReadPersonData(reader);
                reader.NextResult();
                announcement.AnnouncementStandards = reader.ReadList<AnnouncementStandardDetails>();
                announcement.StudentAnnouncements = reader.ReadList<StudentAnnouncementDetails>();
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
            parameters.Add(SCHOOL_ID, schoolId);
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

        public AnnouncementDetails Create(int? classAnnouncementTypeId, int classId, DateTime created, int personId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {CLASS_ANNOUNCEMENT_TYPE_ID_PARAM, classAnnouncementTypeId},
                    {PERSON_ID_PARAM, personId},
                    {CREATED_PARAM, created},
                    {EXPIRES_PARAM, DateTime.MinValue},
                    {STATE_PARAM, AnnouncementState.Draft},
                    {GRADING_STYLE_PARAM, GradingStyleEnum.Numeric100},
                    {CLASS_ID_PARAM, classId},
                    {SCHOOL_ID, schoolId}
                };

            using (var reader = ExecuteStoredProcedureReader(CREATE_PORCEDURE, parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }

        public void Delete(int? id, int? personId, int? classId, int? classAnnouncementTypeId, AnnouncementState? state)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ID_PARAM, id},
                    {PERSON_ID_PARAM, personId},
                    {CLASS_ID_PARAM, classId},
                    {CLASS_ANNOUNCEMENT_TYPE_ID_PARAM, classAnnouncementTypeId},
                    {STATE_PARAM, state}
                };
            ExecuteStoredProcedureReader(DELETE_PROCEDURE, parameters).Dispose();
        }
        
        public abstract AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query);

        public void ReorderAnnouncements(int schoolYearId, int announcementTypeId, int classId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {SCHOOL_YEAR_ID_PARAM, schoolYearId},
                    {"classAnnType", announcementTypeId},
                    {CLASS_ID_PARAM, classId}
                };
            using (ExecuteStoredProcedureReader(REORDER_PROCEDURE, parameters))
            {
            }
        }

        public AnnouncementDetails GetDetails(int id, int callerId, int? roleId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ID_PARAM, id},
                    {CALLER_ID_PARAM, callerId},
                    {CALLER_ROLE_PARAM, roleId},
                    {SCHOOL_ID, schoolId}
                };
            using (var reader = ExecuteStoredProcedureReader(GET_DETAILS_PROCEDURE, parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }

        public IList<Person> GetAnnouncementRecipientPersons(int announcementId, int callerId)
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

        private DbQuery BuildSimpleAnnouncementQuery(int? personId)
        {
            var dbQuery = new DbQuery();
            var classTeacherSql = !personId.HasValue ? "0"
                                  : string.Format(@"(select cast(case when count(*) > 0 then 1 else 0 end as bit)
                                                     from [{0}] where [{0}].[{1}] = {2}  and [{0}].[{3}] = [{4}].[{5}])",
                                          "ClassTeacher" , ClassTeacher.PERSON_REF_FIELD, personId.Value, ClassTeacher.CLASS_REF_FIELD,
                                          "Announcement", Announcement.CLASS_REF_FIELD);

            dbQuery.Sql.AppendFormat(@"select Announcement.*, Class.[{2}] as {3}, {4} as IsOwner
                                       from Announcement join Class on Class.[{0}] = Announcement.[{1}]",
            Class.ID_FIELD, Announcement.CLASS_REF_FIELD, Class.PRIMARY_TEACHER_REF_FIELD, Announcement.PRIMARY_TEACHER_REF_FIELD, classTeacherSql);
            return dbQuery;
        }

        public Announcement GetAnnouncement(int id, int roleId, int callerId)
        {
            var dbQuery = BuildSimpleAnnouncementQuery(callerId);
            FilterBySchool(new AndQueryCondition{{Announcement.ID_FIELD, id}}).BuildSqlWhere(dbQuery, "Announcement");
            BuildConditionForGetSimpleAnnouncement(dbQuery, roleId, callerId);
            return ReadOneOrNull<Announcement>(dbQuery);
        }

        protected abstract void BuildConditionForGetSimpleAnnouncement(DbQuery dbQuery, int role, int callerId);

        public Announcement GetLastDraft(int personId)
        {
            var conds = new AndQueryCondition { {Announcement.STATE_FIELD, AnnouncementState.Draft}};
            var dbQuery = BuildSimpleAnnouncementQuery(personId);
            FilterBySchool(conds).BuildSqlWhere(dbQuery, "Announcement");
            dbQuery.Sql.AppendFormat(" and Class.[{0}] in (select [{1}].[{2}] from [{1}] where [{1}].[{3}] =@{4})"
                , Class.ID_FIELD, "ClassTeacher", ClassTeacher.CLASS_REF_FIELD, ClassTeacher.PERSON_REF_FIELD , "personId");
            dbQuery.Parameters.Add("personId", personId);
            Orm.OrderBy(dbQuery, "Announcement", Announcement.CREATED_FIELD, Orm.OrderType.Desc);
            return ReadOneOrNull<Announcement>(dbQuery);
        }

        public IList<AnnouncementComplex> GetByActivitiesIds(IList<int> activitiesIds)
        {
            if (activitiesIds == null || activitiesIds.Count == 0) return new List<AnnouncementComplex>();
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(@"select * from vwAnnouncement ");
            dbQuery.Sql.AppendFormat("where SisActivityId in ({0})", activitiesIds.Select(x => x.ToString()).JoinString(","));
            return ReadMany<AnnouncementComplex>(dbQuery);
        }

        public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int count)
        {
            var conds = new AndQueryCondition
                {
                    //{Announcement.PERSON_REF_FIELD, personId},
                    {Announcement.CLASS_REF_FIELD, classId},
                    {Announcement.CLASS_ANNOUNCEMENT_TYPE_REF_FIELD, classAnnouncementType}
                };
            var dbQuery = Orm.OrderedSelect(typeof (Announcement).Name, conds, Announcement.ID_FIELD, Orm.OrderType.Desc, count);
            var anns = ReadMany<Announcement>(dbQuery);
            if(anns.Count == 0) return new List<string>();
            return anns.Select(x => x.Content).ToList();
        }

        public bool Exists(string title, int classId, DateTime expiresDate)
        {
            var query = new AndQueryCondition
                {
                    {Announcement.TITLE_FIELD, title},
                    {Announcement.CLASS_REF_FIELD, classId},
                    {Announcement.EXPIRES_FIELD, expiresDate}
                };
            return Exists<Announcement>(query);
        }

        public bool Exists(int sisActivityId)
        {
            return Exists(new List<int> {sisActivityId});
        }
        public bool Exists(IList<int> sisActivitiesIds)
        {
            if (sisActivitiesIds != null && sisActivitiesIds.Count > 0)
            {
                var dbQuery = new DbQuery();
                var tableName = "Announcement";
                var idsString = sisActivitiesIds.Select(x => x.ToString()).JoinString(",");
                dbQuery.Sql.Append(string.Format(@"select * from [{0}] where [{0}].[{1}] in ({2})"
                    , tableName, Announcement.SIS_ACTIVITY_ID_FIELD,idsString));
                return Exists(dbQuery);
            }
            return false;
        }
        public bool CanAddStandard(int announcementId)
        {
            var dbQuery = new DbQuery();
            //dbQuery.Sql.Append(string.Format("select [{0}].[{4}] from [{0}] join [{1}] on [{1}].[{2}] = [{0}].[{3}]"
            //    , "Announcement", "ClassStandard", ClassStandard.CLASS_REF_FIELD, Announcement.CLASS_REF_FIELD, Announcement.ID_FIELD));
            dbQuery.Sql.Append(@"select [Announcement].Id from Announcement 
                                 join Class on Class.Id = Announcement.ClassRef
                                 join ClassStandard  on ClassStandard.ClassRef = Class.Id or ClassStandard.ClassRef = Class.CourseRef ");
            var conds = new AndQueryCondition {{Announcement.ID_FIELD, announcementId}};
            conds.BuildSqlWhere(dbQuery, "Announcement");
            return Exists(dbQuery);
        }
    }

    public class AnnouncementsQuery
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public int? Id { get; set; }
        public int? RoleId { get; set; }
        public int? ClassId { get; set; }
        public int? PersonId { get; set; }
        public int? MarkingPeriodId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? Now { get; set; }
        public bool? Complete { get; set; }
        public bool OwnedOnly { get; set; }
        public bool GradedOnly { get; set; }
        public bool AllSchoolItems { get; set; }

        public IList<int> GradeLevelIds { get; set; }
        public IList<int> SisActivitiesIds { get; set; }

        public bool? Graded { get; set; }

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
