using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public abstract class ClassAnnouncementDataAccess : AnnouncementDataAccess
    {
        private int? schoolId;

        protected ClassAnnouncementDataAccess(UnitOfWork unitOfWork, int schoolId) : base(unitOfWork)
        {
            this.schoolId = schoolId;
        }

        private const string CREATE_PORCEDURE = "spCreateAnnouncement";
        private const string GET_DETAILS_PROCEDURE = "spGetAnnouncementDetails";
        private const string REORDER_PROCEDURE = "spReorderAnnouncements";

        private const string CLASS_ANNOUNCEMENT_TYPE_ID_PARAM = "classAnnouncementTypeId";
        private const string CREATED_PARAM = "created";
        private const string EXPIRES_PARAM = "expires";
        private const string PERSON_ID_PARAM = "personId";
        private const string STATE_PARAM = "state";
        private const string GRADING_STYLE_PARAM = "gradingStyle";
        private const string CLASS_ID_PARAM = "classId";
        private const string MARKING_PERIOD_ID_PARAM = "markingPeriodId";

        private const string SCHOOL_YEAR_ID_PARAM = "schoolYearId";
        private const string ID_PARAM = "id";
        private const string CALLER_ID_PARAM = "callerId";
        private const string CALLER_ROLE_PARAM = "@callerRole";
        private const string ROLE_ID_PARAM = "roleId";
        private const string FROM_DATE_PARAM = "fromDate";
        private const string TO_DATE_PARAM = "toDate";

        private const string SCHOOL_ID = "schoolId";

        private const string START_PARAM = "start";
        private const string COUNT_PARAM = "count";
        private const string NOW_PARAM = "now";
        private const string OWNED_ONLY_PARAM = "ownedOnly";

        protected AnnouncementQueryResult GetAnnouncementsComplex(string procedureName, Dictionary<string, object> parameters, AnnouncementsQuery query)
        {
            parameters.Add(ID_PARAM, query.Id);
            parameters.Add(ROLE_ID_PARAM, query.RoleId);
            parameters.Add(PERSON_ID_PARAM, query.PersonId);
            parameters.Add(MARKING_PERIOD_ID_PARAM, query.MarkingPeriodId);
            parameters.Add(FROM_DATE_PARAM, query.FromDate);
            parameters.Add(TO_DATE_PARAM, query.ToDate);
            parameters.Add(CLASS_ID_PARAM, query.ClassId);
            parameters.Add(START_PARAM, query.Start);
            parameters.Add(COUNT_PARAM, query.Count);
            parameters.Add(NOW_PARAM, query.Now);
            parameters.Add(OWNED_ONLY_PARAM, query.OwnedOnly);
            parameters.Add(SCHOOL_ID, schoolId);
            using (var reader = ExecuteStoredProcedureReader(procedureName, parameters))
            {
                return ReadAnnouncementsQueryResult(reader, query);
            }
        }
        
        public override AnnouncementDetails Create(int? classAnnouncementTypeId, int? classId, DateTime created, DateTime expiresDate, int personId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {CLASS_ANNOUNCEMENT_TYPE_ID_PARAM, classAnnouncementTypeId},
                    {PERSON_ID_PARAM, personId},
                    {CREATED_PARAM, created},
                    {EXPIRES_PARAM, expiresDate},
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

        public override AnnouncementDetails GetDetails(int id, int callerId, int? roleId)
        {
            return GetDetails(id, callerId, roleId, schoolId);
        }
        
        private const string CLASS_ANN_TYPE_PARAM = "classAnnType";
        public override void ReorderAnnouncements(int schoolYearId, int announcementTypeId, int classId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {SCHOOL_YEAR_ID_PARAM, schoolYearId},
                    {CLASS_ANN_TYPE_PARAM, announcementTypeId},
                    {CLASS_ID_PARAM, classId}
                };
            using (ExecuteStoredProcedureReader(REORDER_PROCEDURE, parameters))
            {
            }
        }
        
        public override Announcement GetAnnouncement(int id, int callerId)
        {
            var dbQuery = BuildSimpleAnnouncementQuery(callerId);
            (new AndQueryCondition
                {
                    { Announcement.ID_FIELD, id },
                    { Announcement.SCHOOL_REF_FIELD, schoolId },
                }).BuildSqlWhere(dbQuery, "Announcement");
            BuildConditionForGetSimpleAnnouncement(dbQuery, callerId);
            return ReadOneOrNull<Announcement>(dbQuery);
        }

        protected abstract void BuildConditionForGetSimpleAnnouncement(DbQuery dbQuery, int callerId);
 

        private DbQuery BuildSimpleAnnouncementQuery(int? personId)
        {
            var dbQuery = new DbQuery();
            var classTeacherSql = !personId.HasValue ? "0"
                                  : string.Format(@"(select cast(case when count(*) > 0 then 1 else 0 end as bit)
                                                     from [{0}] where [{0}].[{1}] = {2}  and [{0}].[{3}] = [{4}].[{5}])",
                                          "ClassTeacher", ClassTeacher.PERSON_REF_FIELD, personId.Value, ClassTeacher.CLASS_REF_FIELD,
                                          "Announcement", Announcement.CLASS_REF_FIELD);

            dbQuery.Sql.AppendFormat(@"select Announcement.*, Class.[{2}] as {3}, {4} as IsOwner
                                       from Announcement join Class on Class.[{0}] = Announcement.[{1}]",
            Class.ID_FIELD, Announcement.CLASS_REF_FIELD, Class.PRIMARY_TEACHER_REF_FIELD, Announcement.PRIMARY_TEACHER_REF_FIELD, classTeacherSql);
            return dbQuery;
        }


        public override Announcement GetLastDraft(int personId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.STATE_FIELD, AnnouncementState.Draft},
                    {Announcement.SCHOOL_REF_FIELD, schoolId}
                };
            var dbQuery = BuildSimpleAnnouncementQuery(personId);
            conds.BuildSqlWhere(dbQuery, "Announcement");
            dbQuery.Sql.AppendFormat(" and Class.[{0}] in (select [{1}].[{2}] from [{1}] where [{1}].[{3}] =@{4})"
                , Class.ID_FIELD, "ClassTeacher", ClassTeacher.CLASS_REF_FIELD, ClassTeacher.PERSON_REF_FIELD, "personId");
            dbQuery.Parameters.Add("personId", personId);
            Orm.OrderBy(dbQuery, "Announcement", Announcement.CREATED_FIELD, Orm.OrderType.Desc);
            return ReadOneOrNull<Announcement>(dbQuery);
        }

        public override IList<AnnouncementComplex> GetByActivitiesIds(IList<int> activitiesIds)
        {
            if (activitiesIds == null || activitiesIds.Count == 0) return new List<AnnouncementComplex>();
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(@"select * from vwAnnouncement ");
            dbQuery.Sql.AppendFormat("where SisActivityId in ({0})", activitiesIds.Select(x => x.ToString(CultureInfo.InvariantCulture)).JoinString(","));
            return ReadMany<AnnouncementComplex>(dbQuery);
        }

        public override IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int count)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.CLASS_REF_FIELD, classId},
                    {Announcement.CLASS_ANNOUNCEMENT_TYPE_REF_FIELD, classAnnouncementType},
                    {Announcement.SCHOOL_REF_FIELD, schoolId}
                };
            var dbQuery = Orm.OrderedSelect(typeof(Announcement).Name, conds, Announcement.ID_FIELD, Orm.OrderType.Desc, count);
            var anns = ReadMany<Announcement>(dbQuery);
            if (anns.Count == 0) return new List<string>();
            return anns.Select(x => x.Content).ToList();
        }

        public override bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            var query = new AndQueryCondition
                {
                    {Announcement.TITLE_FIELD, title},
                    {Announcement.CLASS_REF_FIELD, classId},
                    {Announcement.EXPIRES_FIELD, expiresDate},
                    {Announcement.SCHOOL_REF_FIELD, schoolId}
                };

            if (excludeAnnouncementId.HasValue)
                query.Add(Announcement.ID_FIELD, Announcement.ID_FIELD, excludeAnnouncementId, ConditionRelation.NotEqual);

            return Exists<Announcement>(query);
        }

        public override bool Exists(int sisActivityId)
        {
            return Exists(new List<int> { sisActivityId });
        }
        public override bool Exists(IList<int> sisActivitiesIds)
        {
            if (sisActivitiesIds != null && sisActivitiesIds.Count > 0)
            {
                var dbQuery = new DbQuery();
                var tableName = "Announcement";
                var idsString = sisActivitiesIds.Select(x => x.ToString(CultureInfo.InvariantCulture)).JoinString(",");
                dbQuery.Sql.Append(string.Format(@"select * from [{0}] where [{0}].[{1}] in ({2})"
                    , tableName, Announcement.SIS_ACTIVITY_ID_FIELD, idsString));
                return Exists(dbQuery);
            }
            return false;
        }
        public override bool CanAddStandard(int announcementId)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append(@"select [Announcement].Id from Announcement 
                                 join Class on Class.Id = Announcement.ClassRef
                                 join ClassStandard  on ClassStandard.ClassRef = Class.Id or ClassStandard.ClassRef = Class.CourseRef ");
            var conds = new AndQueryCondition
                {
                    {Announcement.ID_FIELD, announcementId},
                    {Announcement.SCHOOL_REF_FIELD, schoolId}
                };
            conds.BuildSqlWhere(dbQuery, typeof(Announcement).Name);
            return Exists(dbQuery);
        }

    }
}
