using System;
using System.Collections.Generic;
using System.Data;
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
        protected int? schoolYearId;

        protected ClassAnnouncementDataAccess(UnitOfWork unitOfWork, int schoolYearId) : base(unitOfWork)
        {
            this.schoolYearId = schoolYearId;
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
        
        
        public override ClassAnnouncement GetById(int key)
        {
            var conds = new AndQueryCondition {{Announcement.ID_FIELD, key}};
            var dbQuery = Orm.SimpleSelect(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, conds);
            return ReadOne<ClassAnnouncement>(dbQuery);
        }

        public override IList<ClassAnnouncement> GetByIds(IList<int> keys)
        {
            var query = $@"Select * From vwClassAnnouncement Where Id in(select * from @keys)";
            var @params = new Dictionary<string, object>
            {
                ["keys"] = keys
            };

            return ReadMany<ClassAnnouncement>(new DbQuery(query, @params));
        }

        public override IList<ClassAnnouncement> GetAll(QueryCondition conditions = null)
        {
            var dbQuery = Orm.SimpleSelect(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, conditions);
            return ReadMany<ClassAnnouncement>(dbQuery);
        }
        
        public override void Insert(ClassAnnouncement entity)
        {
            Insert(new List<ClassAnnouncement>{ entity });
        }

        public override void Insert(IList<ClassAnnouncement> entities)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"classAnnouncements", InputClassAnnouncement.Create(entities)}
                };
            ExecuteStoredProcedure("spInsertClassAnnouncement", parameters);
        }

        public override void Update(IList<ClassAnnouncement> entities)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"classAnnouncements", InputClassAnnouncement.Create(entities)}
                };
            ExecuteStoredProcedure("spUpdateClassAnnouncement", parameters);
        }

        public override void Update(ClassAnnouncement entity)
        {
            Update(new List<ClassAnnouncement>{ entity });
        }



        public IList<ClassAnnouncement> GetClassAnnouncementByFilter(string filter, int callerId)
        {
            var conds = new AndQueryCondition();
            var dbQuery = SelectClassAnnouncements(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, callerId);
            conds.BuildSqlWhere(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME);
            FilterClassAnnouncementByCaller(dbQuery, callerId);

            var words = filter.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 0)
            {
                dbQuery.Sql.Append(" and (");
                for (var i = 0; i < words.Length; i++)
                {
                    var filterName = $"filter{i}";
                    dbQuery.Parameters.Add(filterName, string.Format(FILTER_FORMAT, words[i]));
                    dbQuery.Sql.Append("( ")
                               .AppendFormat($"{ClassAnnouncement.FULL_CLASS_NAME} like @{filterName} or ")
                               .AppendFormat($"{Announcement.TITLE_FIELD} like @{filterName} ")
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
        public override IList<ClassAnnouncement> GetAnnouncements(QueryCondition conds, int callerId)
        {
            var dbQuery = SelectClassAnnouncements(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, callerId);
            conds.BuildSqlWhere(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME);
            FilterClassAnnouncementByCaller(dbQuery, callerId);
            return ReadMany<ClassAnnouncement>(dbQuery);
        }


        //common implementation for teacher or admin 
        protected virtual DbQuery SelectClassAnnouncements(string tableName, int callerId)
        {
            var dbQuery = new DbQuery();
            var classTeacherSql = string.Format(@"(select cast(case when count(*) > 0 then 1 else 0 end as bit)
                                                     from [{0}] where [{0}].[{1}] = {2}  and [{0}].[{3}] = [{4}].[{5}])",
                                          "ClassTeacher", ClassTeacher.PERSON_REF_FIELD, callerId, ClassTeacher.CLASS_REF_FIELD,
                                          tableName, ClassAnnouncement.CLASS_REF_FIELD);
            var selectSet = $"{tableName}.*, {classTeacherSql} as {nameof(Announcement.IsOwner)}";
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, selectSet, tableName);
            return dbQuery;
        }
        protected abstract DbQuery FilterClassAnnouncementByCaller(DbQuery dbQuery, int callerId);


        //common implementation for teacher or admin 
        public virtual ClassAnnouncement GetLastDraft(int personId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.STATE_FIELD, AnnouncementState.Draft},
                    {ClassAnnouncement.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
                };
            var dbQuery = SelectClassAnnouncements(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, personId);
            conds.BuildSqlWhere(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME);
            
            dbQuery.Sql.Append(@" and (ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where ClassTeacher.PersonRef = @callerId))");
            dbQuery.Parameters.Add("callerId", personId);

            Orm.OrderBy(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, Announcement.CREATED_FIELD, Orm.OrderType.Desc);
            return ReadOneOrNull<ClassAnnouncement>(dbQuery);
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
                    {SCHOOL_YEAR_ID_PARAM, schoolYearId}
                };

            using (var reader = ExecuteStoredProcedureReader(CREATE_PORCEDURE, parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }
        
        public override IList<AnnouncementDetails> GetDetailses(IList<int> ids, int callerId, int? roleId, bool onlyOwner = true)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"classAnnouncementIds", ids},
                    {"callerId", callerId},
                    {"callerRole", roleId},
                    {"schoolYearId", schoolYearId},
                    {"onlyOwner", !CanGetAllItems }
                };
            return GetDetailses("spGetListOfClassAnnouncementDetails", parameters);
        }

        protected override ClassAnnouncement ReadAnnouncementData(AnnouncementComplex announcement, SqlDataReader reader)
        {
            var res = reader.Read<ClassAnnouncement>();
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
            var parameters = new Dictionary<string, object>
                {
                    {"personId", personId},
                    {"sisActivityIds", activitiesIds}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetClassAnnouncementsBySisActivities", parameters))
            {
                var query = new ClassAnnouncementsQuery {SisActivitiesIds = activitiesIds, PersonId = personId};
                return ReadAnnouncementsQueryResult(reader, query).Announcements;
            }
        }

        public IList<string> GetLastFieldValues(int classId, int classAnnouncementType, int count)
        {
            var conds = new AndQueryCondition
                {
                    {ClassAnnouncement.CLASS_REF_FIELD, classId},
                    {ClassAnnouncement.CLASS_ANNOUNCEMENT_TYPE_REF_FIELD, classAnnouncementType},
                    {ClassAnnouncement.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
                };
            var dbQuery = Orm.OrderedSelect(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, conds, Announcement.ID_FIELD, Orm.OrderType.Desc, count);
            var anns = ReadMany<ClassAnnouncement>(dbQuery);
            return anns.Count == 0 ? new List<string>() : anns.Select(x => x.Content).Distinct().ToList();
        }

        public bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.TITLE_FIELD, title},
                    {ClassAnnouncement.CLASS_REF_FIELD, classId},
                    {ClassAnnouncement.EXPIRES_FIELD, expiresDate},
                    {ClassAnnouncement.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
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
                    {ClassAnnouncement.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
                };
            conds.BuildSqlWhere(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME);
            return Exists(dbQuery);
        }
        
        /// <summary>
        /// Created copies of class announcements.
        /// </summary>
        /// <param name="sisCopyResult">Key is SOURCE sis activity id. Value is NEW sis activity id</param>
        /// <returns>Key is SOURCE announcement id. Value is NEW announcement id</returns>
        public IDictionary<int, int> CopyClassAnnouncementsToClass(IDictionary<int, int> sisCopyResult, int toClassId, DateTime created)
        {
            var sisCopyRes = SisActivityCopyResult.Create(sisCopyResult);
            var @params = new Dictionary<string, object>
            {
                ["sisCopyResult"] = sisCopyRes,
                ["toClassId"] = toClassId,
                ["created"] = created
            };

            using (var reader = ExecuteStoredProcedureReader("spCopyClassAnnouncementsToClass", @params))
            {
                return reader.ReadList<AnnouncementCopyResult>()
                    .ToDictionary(x => x.FromAnnouncementId, y => y.ToAnnouncementId);
            }
        }
    }

    public class ClassAnnouncementsQuery : AnnouncementsQuery
    {
        public int? ClassId { get; set; }
        public IList<int> SisActivitiesIds { get; set; }
        public bool? Graded { get; set; }
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
    }
}
