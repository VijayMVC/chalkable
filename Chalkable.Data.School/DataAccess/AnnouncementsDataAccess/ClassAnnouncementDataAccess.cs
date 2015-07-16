using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public abstract class ClassAnnouncementDataAccess : BaseAnnouncementDataAccess<ClassAnnouncement>
    {
        protected int? schoolId;

        protected ClassAnnouncementDataAccess(UnitOfWork unitOfWork, int schoolId) : base(unitOfWork)
        {
            this.schoolId = schoolId;
        }

        private const string CREATE_PORCEDURE = "spCreateClasssAnnouncement";
        private const string REORDER_PROCEDURE = "spReorderAnnouncements";

        private const string CLASS_ANNOUNCEMENT_TYPE_ID_PARAM = "classAnnouncementTypeId";
        private const string CREATED_PARAM = "created";
        private const string EXPIRES_PARAM = "expires";
        private const string PERSON_ID_PARAM = "personId";
        private const string STATE_PARAM = "state";
        private const string GRADING_STYLE_PARAM = "gradingStyle";
        private const string CLASS_ID_PARAM = "classId";
        
        private const string SCHOOL_YEAR_ID_PARAM = "schoolYearId";
        private const string ID_PARAM = "id";
        private const string FROM_DATE_PARAM = "fromDate";
        private const string TO_DATE_PARAM = "toDate";

        private const string SCHOOL_ID = "schoolId";

        private const string START_PARAM = "start";
        private const string COUNT_PARAM = "count";

        public abstract ClassAnnouncement GetLastDraft(int personId, int schoolYearId);


        protected AnnouncementQueryResult GetAnnouncementsComplex(string procedureName, Dictionary<string, object> parameters, AnnouncementsQuery query)
        {
            parameters.Add(ID_PARAM, query.Id);
            parameters.Add(PERSON_ID_PARAM, query.PersonId);
            parameters.Add(FROM_DATE_PARAM, query.FromDate);
            parameters.Add(TO_DATE_PARAM, query.ToDate);
            parameters.Add(CLASS_ID_PARAM, query.ClassId);
            parameters.Add(START_PARAM, query.Start);
            parameters.Add(COUNT_PARAM, query.Count);
            parameters.Add(SCHOOL_ID, schoolId);
            using (var reader = ExecuteStoredProcedureReader(procedureName, parameters))
            {
                return ReadAnnouncementsQueryResult(reader, query);
            }
        }

        public override ClassAnnouncement GetById(int key)
        {
            var conds = new AndQueryCondition {{Announcement.ID_FIELD, key}};
            var dbQuery = Orm.SimpleSelect(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, conds);
            return ReadOne<ClassAnnouncement>(dbQuery);
        }
        public override IList<ClassAnnouncement> GetAll(QueryCondition conditions = null)
        {
            var dbQuery = Orm.SimpleSelect(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, conditions);
            return ReadMany<ClassAnnouncement>(dbQuery);
        }

        //TODO : move this to stored procedure later
        public override void Insert(ClassAnnouncement entity)
        {
            SimpleInsert<Announcement>(entity);
            var id = SelectMany<Announcement>(new AndQueryCondition()).OrderByDescending(x=>x.Id).First().Id;
            entity.Id = id;
            var t = typeof (ClassAnnouncement);
            var fileds = Orm.Fields(t, true, true, true);
            fileds.Add(Announcement.ID_FIELD);
            var q = Orm.SimpleListInsert(t, new List<ClassAnnouncement> { entity }, fileds, false);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        public override void Insert(IList<ClassAnnouncement> entities)
        {
            foreach (var classAnnouncement in entities)
            {
                Insert(classAnnouncement);
            }
        }

        public override void Update(IList<ClassAnnouncement> entities)
        {
            foreach (var classAnnouncement in entities)
            {
                Update(classAnnouncement);
            }
        }

        public override void Update(ClassAnnouncement entity)
        {
            SimpleUpdate<Announcement>(entity);
            base.Update(entity);
        }



        public IList<ClassAnnouncement> GetClassAnnouncementByFilter(string filter, int callerId)
        {
            var conds = new AndQueryCondition();
            var dbQuery = SeletClassAnnouncements(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, callerId);
            conds.BuildSqlWhere(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME);
            FilterClassAnnouncementByCaller(dbQuery, callerId);

            var words = filter.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 0)
            {
                dbQuery.Sql.Append(" and (");
                for (var i = 0; i < words.Length; i++)
                {
                    var filterName = string.Format("filter{0}", i);
                    dbQuery.Parameters.Add(filterName, string.Format(FILTER_FORMAT, words[i]));
                    dbQuery.Sql.Append("( ")
                               .AppendFormat("{0} like @{1} or ", ClassAnnouncement.FULL_CLASS_NAME, filterName)
                               .AppendFormat("{0} like @{1} ", Announcement.TITLE_FIELD, filterName)
                               .Append(" )");
                    if (i < words.Length - 1)
                        dbQuery.Sql.Append(" or ");
                }
                dbQuery.Sql.Append(")");
            }
            return ReadMany<ClassAnnouncement>(dbQuery);
        }
        
        public override ClassAnnouncement GetAnnouncement(int id, int callerId)
        {
            var conds = new AndQueryCondition {{Announcement.ID_FIELD, id}};
            return GetAnnouncements(conds, callerId).FirstOrDefault();
        }

        protected abstract DbQuery SeletClassAnnouncements(string tableName, int callerId);
        protected abstract DbQuery FilterClassAnnouncementByCaller(DbQuery dbQuery, int callerId);

        public override IList<ClassAnnouncement> GetAnnouncements(QueryCondition conds, int callerId)
        {
            var dbQuery = SeletClassAnnouncements(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, callerId);
            conds.BuildSqlWhere(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME);
            FilterClassAnnouncementByCaller(dbQuery, callerId);
            return ReadMany<ClassAnnouncement>(dbQuery);
        }

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            throw new NotImplementedException();
        }

        public AnnouncementDetails Create(int? classAnnouncementTypeId, int classId, DateTime created, DateTime expiresDate, int personId)
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
            var parameters = new Dictionary<string, object>
                {
                    {"classAnnouncementId", id},
                    {"callerId", callerId},
                    {"callerRole", roleId},
                    {"schoolId", schoolId}
                };
            return GetDetails("spGetClassAnnouncementDetails", parameters);
        }

        protected override ClassAnnouncement ReadAnnouncementData(AnnouncementComplex announcement, SqlDataReader reader)
        {
            var res = reader.Read<ClassAnnouncement>();
            res.IsScored = res.MaxScore > 0;
            return res;
        }

        private const string CLASS_ANN_TYPE_PARAM = "classAnnType";

        public void ReorderAnnouncements(int schoolYearId, int classAnnouncementTypeId, int classId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {SCHOOL_YEAR_ID_PARAM, schoolYearId},
                    {CLASS_ANN_TYPE_PARAM, classAnnouncementTypeId},
                    {CLASS_ID_PARAM, classId}
                };
            using (ExecuteStoredProcedureReader(REORDER_PROCEDURE, parameters))
            {
            }
        }
        
        
        public IList<AnnouncementComplex> GetByActivitiesIds(IList<int> activitiesIds, int personId)
        {
            if (activitiesIds == null || activitiesIds.Count == 0) return new List<AnnouncementComplex>();
            var strIds = activitiesIds.Select(x => x.ToString(CultureInfo.InvariantCulture)).JoinString(",");
            var parameters = new Dictionary<string, object>
                {
                    {"personId", personId},
                    {"sisActivityIds", strIds}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetClassAnnouncementsBySisActivities", parameters))
            {
                var query = new AnnouncementsQuery {SisActivitiesIds = activitiesIds, PersonId = personId};
                return ReadAnnouncementsQueryResult(reader, query).Announcements;
            }
        }

        public IList<string> GetLastFieldValues(int classId, int classAnnouncementType, int count)
        {
            var conds = new AndQueryCondition
                {
                    {ClassAnnouncement.CLASS_REF_FIELD, classId},
                    {ClassAnnouncement.CLASS_ANNOUNCEMENT_TYPE_REF_FIELD, classAnnouncementType},
                    {ClassAnnouncement.SCHOOL_REF_FIELD, schoolId}
                };
            var dbQuery = Orm.OrderedSelect(AdminAnnouncement.VW_ADMIN_ANNOUNCEMENT_NAME, conds, Announcement.ID_FIELD, Orm.OrderType.Desc, count);
            var anns = ReadMany<Announcement>(dbQuery);
            return anns.Count == 0 ? new List<string>() : anns.Select(x => x.Content).Distinct().ToList();
        }

        public bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.TITLE_FIELD, title},
                    {ClassAnnouncement.CLASS_REF_FIELD, classId},
                    {ClassAnnouncement.EXPIRES_FIELD, expiresDate},
                    {ClassAnnouncement.SCHOOL_REF_FIELD, schoolId}
                };

            if (excludeAnnouncementId.HasValue)
                conds.Add(Announcement.ID_FIELD, Announcement.ID_FIELD, excludeAnnouncementId, ConditionRelation.NotEqual);
            return Exists(Orm.SimpleSelect(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, conds));
        }

        public bool Exists(int sisActivityId)
        {
            return Exists(new List<int> { sisActivityId });
        }
        public bool Exists(IList<int> sisActivitiesIds)
        {
            if (sisActivitiesIds != null && sisActivitiesIds.Count > 0)
            {
                var dbQuery = new DbQuery();
                var tableName = typeof (ClassAnnouncement).Name;
                var idsString = sisActivitiesIds.Select(x => x.ToString(CultureInfo.InvariantCulture)).JoinString(",");
                dbQuery.Sql.Append(string.Format(@"select * from [{0}] where [{0}].[{1}] in ({2})"
                    , tableName, ClassAnnouncement.SIS_ACTIVITY_ID_FIELD, idsString));
                return Exists(dbQuery);
            }
            return false;
        }
        public override bool CanAddStandard(int announcementId)
        {
            var dbQuery = new DbQuery();
            var classStandardTName = typeof (ClassStandard).Name;
            var classTName = typeof (Class).Name;
            dbQuery.Sql.AppendFormat(Orm.SELECT_COLUMN_FORMAT, Announcement.ID_FIELD, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME)
                   .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, classTName, Class.ID_FIELD, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, ClassAnnouncement.CLASS_REF_FIELD)
                   .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, classStandardTName, ClassStandard.CLASS_REF_FIELD, classTName, Class.ID_FIELD)
                   .Append(" or ")
                   .AppendFormat("[{0}].{1} = [{2}].{3}", classStandardTName, ClassStandard.CLASS_REF_FIELD, classTName, Class.COURSE_REF_FIELD);
            
            var conds = new AndQueryCondition
                {
                    {Announcement.ID_FIELD, announcementId},
                    {ClassAnnouncement.SCHOOL_REF_FIELD, schoolId}
                };
            conds.BuildSqlWhere(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME);
            return Exists(dbQuery);
        }
    }
}
