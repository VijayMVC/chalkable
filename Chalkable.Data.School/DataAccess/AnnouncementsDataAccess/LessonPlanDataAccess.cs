using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class LessonPlanDataAccess : BaseAnnouncementDataAccess<LessonPlan>
    {
        protected int schoolYearId;
        public LessonPlanDataAccess(UnitOfWork unitOfWork, int schoolYearId) : base(unitOfWork)
        {
            this.schoolYearId = schoolYearId;
        }

        public override void Insert(LessonPlan entity)
        {
            throw new NotImplementedException();
            //SimpleInsert<Announcement>(entity);
            //SimpleInsert(entity);
        }

        //TODO: rewrite this later
        public override void Update(LessonPlan entity)
        {
            SimpleUpdate<Announcement>(entity);
            base.Update(entity);
        }
        
        public AnnouncementDetails Create(int classId, DateTime created, DateTime? startDate, DateTime? endDate, int personId, int schoolYearId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"schoolYearId", schoolYearId},
                    {"classId", classId},
                    {"created", created},
                    {"startDate", startDate},
                    {"endDate", endDate},
                    {"personId", personId},
                    {"state", AnnouncementState.Draft}
                };
            using (var reader = ExecuteStoredProcedureReader("spCreateLessonPlan", parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }

        public AnnouncementDetails CreateFromTemplate(int lessonPlanTemplateId, int personId, int classId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"schoolYearId", schoolYearId},
                    {"personId", personId},
                    {"lessonPlanTemplateId", lessonPlanTemplateId},
                    {"classId", classId}
                };
            using (var reader = ExecuteStoredProcedureReader("spCreateFromTemplate", parameters))
            {
                return BuildGetDetailsResult(reader);
            }
        }

        
        public IList<int> DuplicateLessonPlan(int lessonPlanId, IList<int> classIds, DateTime created)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"lessonPlanId", lessonPlanId},
                    {"classIds", classIds},
                    {"created", created}
                };
            using (var reader = ExecuteStoredProcedureReader("spDuplicateLessonPlan", parameters))
            {
                var res = new List<int>();
                while (reader.Read())
                {
                    res.Add(SqlTools.ReadInt32(reader, "LessonPlanId"));
                }
                return res;
            }
        }

        public override AnnouncementDetails GetDetails(int id, int callerId, int? roleId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"lessonPlanId", id},
                    {"callerId", callerId},
                    {"callerRole", roleId},
                    {"schoolYearId", schoolYearId}
                };
           return GetDetails("spGetLessonPlanDetails", parameters);
        }

        protected override LessonPlan ReadAnnouncementData(AnnouncementComplex announcement, SqlDataReader reader)
        {
            return reader.Read<LessonPlan>();
        }

        public override LessonPlan GetAnnouncement(int lessonPlanId, int callerId)
        {
            return GetLessonPlan(new AndQueryCondition {{Announcement.ID_FIELD, lessonPlanId}}, callerId);
        }

        public LessonPlan GetLessonPlan(QueryCondition condition, int callerId)
        {
            return GetAnnouncements(condition, callerId).FirstOrDefault();
        }
        public override IList<LessonPlan> GetAnnouncements(QueryCondition condition, int callerId)
        {
            var conds = new AndQueryCondition { condition, { LessonPlan.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId } };
            var dbQuery = SelectLessonPlan(conds, callerId);
            dbQuery = FilterLessonPlanByCallerId(dbQuery, callerId);
            return ReadMany<LessonPlan>(dbQuery);
        }

        public IList<LessonPlan> GetLessonPlans(DateTime? fromDate, DateTime? toDate, int? classId, int? galleryCategoryId, int callerId)
        {
            var conds = new AndQueryCondition {{Announcement.STATE_FIELD, AnnouncementState.Created}};
            if(fromDate.HasValue)
                conds.Add(LessonPlan.START_DATE_FIELD, "fromDate", fromDate, ConditionRelation.GreaterEqual);
            if(toDate.HasValue)
                conds.Add(LessonPlan.START_DATE_FIELD, "toDate", toDate, ConditionRelation.LessEqual);
            if(classId.HasValue)
                conds.Add(LessonPlan.CLASS_REF_FIELD, classId);
            if(galleryCategoryId.HasValue)
                conds.Add(LessonPlan.GALERRY_CATEGORY_REF_FIELD, galleryCategoryId);
            return GetAnnouncements(conds, callerId);
        }

        public IList<LessonPlan> GetLessonPlansByFilter(string filter, int callerId)
        {
            var conds = new AndQueryCondition {{LessonPlan.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}};
            var dbQuery = SelectLessonPlan(conds, callerId);
            FilterLessonPlanByCallerId(dbQuery, callerId);
            var words = filter.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 0)
            {
                dbQuery.Sql.Append(" and (");
                for (var i = 0; i < words.Length; i++)
                {
                    var filterName = string.Format("filter{0}", i);
                    dbQuery.Parameters.Add(filterName, string.Format(FILTER_FORMAT, words[i]));
                    dbQuery.Sql.Append("( ")
                        .AppendFormat("{0} like @{1} or ", LessonPlan.FULL_CLASS_NAME, filterName)
                               .AppendFormat("{0} like @{1} ", Announcement.TITLE_FIELD, filterName)
                               .Append(" ) ");
                    if (i < words.Length - 1)
                        dbQuery.Sql.Append(" or ");
                }
                dbQuery.Sql.Append(")");
            }
            return ReadMany<LessonPlan>(dbQuery);
        } 

        
        public IList<LessonPlan> GetLessonPlanTemplates(int? galleryCategoryId, string title, int? classId, int callerId)
        {
            var conds = new AndQueryCondition {{LessonPlan.GALERRY_CATEGORY_REF_FIELD, null, ConditionRelation.NotEqual}};
            if(galleryCategoryId.HasValue)
                conds.Add(LessonPlan.GALERRY_CATEGORY_REF_FIELD, galleryCategoryId);
            if(classId.HasValue)
                conds.Add(LessonPlan.CLASS_REF_FIELD, classId);
            var dbQuery = SelectLessonPlan(conds, callerId);

            dbQuery.Sql.AppendFormat(" and {0} like @{0}", Announcement.TITLE_FIELD);
            dbQuery.Parameters.Add(Announcement.TITLE_FIELD, string.Format(FILTER_FORMAT, title));
            return ReadMany<LessonPlan>(dbQuery);
        }

        protected DbQuery SelectLessonPlan(QueryCondition condition, int? callerId = null)
        {
            var dbQuery = new DbQuery();
            var classTeacherSql = !callerId.HasValue ? "cast(0 as bit)" :
                            string.Format(@"(select cast(case when count(*) > 0 then 1 else 0 end as bit)
                                                     from [{0}] where [{0}].[{1}] = {2}  and [{0}].[{3}] = [{4}].[{5}])",
                              "ClassTeacher", ClassTeacher.PERSON_REF_FIELD, callerId, ClassTeacher.CLASS_REF_FIELD,
                              LessonPlan.VW_LESSON_PLAN_NAME, LessonPlan.CLASS_REF_FIELD);

            var selectSet = string.Format("{0}.*, {1} as IsOwner", LessonPlan.VW_LESSON_PLAN_NAME, classTeacherSql);
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, selectSet, LessonPlan.VW_LESSON_PLAN_NAME);
            condition.BuildSqlWhere(dbQuery, LessonPlan.VW_LESSON_PLAN_NAME);
            return dbQuery;
        }

        protected virtual DbQuery FilterLessonPlanByCallerId(DbQuery dbQuery, int callerId)
        {
            return dbQuery;
        }

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            var parameter = new Dictionary<string, object>
                {
                    {"id", query.Id},
                    {"schoolYearId", schoolYearId},
                    {"personId", query.PersonId},
                    {"classId", query.ClassId},
                    {"roleId", query.RoleId},
                    {"ownedOnly", query.OwnedOnly},
                    {"fromDate", query.FromDate},
                    {"toDate", query.ToDate},
                    {"start", query.Start},
                    {"count", query.Count},
                    {"complete", query.Complete},
                    {"galleryCategoryId", query.GalleryCategoryId}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetLessonPlans", parameter))
            {
                return ReadAnnouncementsQueryResult(reader, query);
            }
        }

        public LessonPlan GetLastDraft(int personId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.STATE_FIELD, AnnouncementState.Draft},
                    {LessonPlan.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
                };
            var dbQuery = SelectLessonPlan(conds);
            FilterLessonPlanByCallerId(dbQuery, personId);
            Orm.OrderBy(dbQuery, LessonPlan.VW_LESSON_PLAN_NAME, Announcement.CREATED_FIELD, Orm.OrderType.Desc);
            return ReadOneOrNull<LessonPlan>(dbQuery);
        }

        public override bool CanAddStandard(int announcementId)
        {

            var dbQuery = new DbQuery();
            var classStandardTName = typeof (ClassStandard).Name;
            var classTName = typeof (Class).Name;
            dbQuery.Sql.AppendFormat(Orm.SELECT_COLUMN_FORMAT, Announcement.ID_FIELD, LessonPlan.VW_LESSON_PLAN_NAME)
                   .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, classTName, Class.ID_FIELD, LessonPlan.VW_LESSON_PLAN_NAME, LessonPlan.CLASS_REF_FIELD)
                   .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, classStandardTName, ClassStandard.CLASS_REF_FIELD, classTName, Class.ID_FIELD)
                   .Append(" or ")
                   .AppendFormat("[{0}].{1} = [{2}].{3}", classStandardTName, ClassStandard.CLASS_REF_FIELD, classTName, Class.COURSE_REF_FIELD);
            
            var conds = new AndQueryCondition
                {
                    {Announcement.ID_FIELD, announcementId},
                    {LessonPlan.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
                };
            conds.BuildSqlWhere(dbQuery, LessonPlan.VW_LESSON_PLAN_NAME);
            return Exists(dbQuery);
        }

        public IList<string> GetLastFields(int personId, int classId)
        {
            var conds = new AndQueryCondition
                {
                    {LessonPlan.CLASS_REF_FIELD, classId},
                    {LessonPlan.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
                };
            var res = ReadMany<LessonPlan>(SelectLessonPlan(conds));
            return res.Select(x => x.Content).Distinct().ToList();
        }

        //TODO: remove this method later 
        public bool Exists(string title, int? excludedLessonPlanId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.TITLE_FIELD, title},
                    {LessonPlan.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
                };
            if (excludedLessonPlanId.HasValue)
                conds.Add(Announcement.ID_FIELD, excludedLessonPlanId, ConditionRelation.NotEqual);
            var dbQuery = Orm.SimpleSelect(LessonPlan.VW_LESSON_PLAN_NAME, conds);
            return Exists(dbQuery);
        }

        public bool ExistsInGallery(string title, int? excludedLessonPlanId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.TITLE_FIELD, title},
                    {LessonPlan.GALERRY_CATEGORY_REF_FIELD, null, ConditionRelation.NotEqual}
                };
            if (excludedLessonPlanId.HasValue)
                conds.Add(Announcement.ID_FIELD, excludedLessonPlanId, ConditionRelation.NotEqual);
            var dbQuery = Orm.SimpleSelect(LessonPlan.VW_LESSON_PLAN_NAME, conds);
            return Exists(dbQuery);
        }

        
    }

    public class LessonPlanForTeacherDataAccess : LessonPlanDataAccess
    {
        public LessonPlanForTeacherDataAccess(UnitOfWork unitOfWork, int schoolYearId)
            : base(unitOfWork, schoolYearId)
        {
        }

        protected override DbQuery FilterLessonPlanByCallerId(DbQuery dbQuery, int callerId)
        {
            var callerIdParam = "callerId";
            dbQuery.Sql.AppendFormat(" and ClassRef in (select ClassRef from ClassTeacher where ClassTeacher.PersonRef =@{0})", callerIdParam);
            dbQuery.Parameters.Add(callerIdParam, callerId);
            return dbQuery;
        }
    }

    public class LessonPlanForStudentDataAccess : LessonPlanDataAccess
    {
        public LessonPlanForStudentDataAccess(UnitOfWork unitOfWork, int schoolYearId)
            : base(unitOfWork, schoolYearId)
        {
        }

        protected override DbQuery FilterLessonPlanByCallerId(DbQuery dbQuery, int callerId)
        {
            var callerIdParam = "callerId";
            dbQuery.Sql.AppendFormat(" and ClassRef in (select ClassRef from ClassPerson where ClassPerson.PersonRef =@{0})", callerIdParam);
            dbQuery.Sql.AppendFormat(" and {0} = 1", LessonPlan.VISIBLE_FOR_STUDENT_FIELD);
            dbQuery.Parameters.Add(callerIdParam, callerId);
            return dbQuery;
        }
    }
}
