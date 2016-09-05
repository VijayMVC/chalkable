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

        public override IList<LessonPlan> GetByIds(IList<int> keys)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append($@"Select * From {LessonPlan.VW_LESSON_PLAN_NAME} Where Id in(Select * from @keys)");
            dbQuery.Parameters.Add("keys", keys);

            return ReadMany<LessonPlan>(dbQuery);
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
        public override void Update(IList<LessonPlan> entities)
        {
            foreach (var lessonPlan in entities)
            {
                Update(lessonPlan);
            }
        }
        public AnnouncementDetails Create(int? classId, DateTime created, DateTime? startDate, DateTime? endDate, int personId, int schoolYearId, int callerRole)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"schoolYearId", schoolYearId},
                    {"classId", classId},
                    {"created", created},
                    {"startDate", startDate},
                    {"endDate", endDate},
                    {"personId", personId},
                    {"state", AnnouncementState.Draft},
                    {"callerRole", callerRole}
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
        public override IList<AnnouncementDetails> GetDetailses(IList<int> ids, int callerId, int? roleId, bool onlyOwner = true)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"lessonPlanIds", ids},
                    {"callerId", callerId},
                    {"callerRole", roleId},
                    {"schoolYearId", schoolYearId},
                    {"onlyOwner", !CanGetAllItems } 
                };
            return GetDetailses("spGetListOfLessonPlansDetails", parameters);
        }

        protected override bool CanGetAllItems  => false;

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

        public IList<LessonPlan> GetLessonPlans(DateTime? fromDate, DateTime? toDate, int? classId, int? lpGalleryCategoryId, int callerId, int? studentId, int? teacherId, bool filterByStartDate = true)
        {
            //TODO: move this to the stored procedure later 

            var conds = new AndQueryCondition
            {
                {Announcement.STATE_FIELD, AnnouncementState.Created},
            };
            if (fromDate.HasValue)
                conds.Add(LessonPlan.END_DATE_FIELD, "fromDate", fromDate, ConditionRelation.GreaterEqual);
            if (toDate.HasValue)
                conds.Add(LessonPlan.START_DATE_FIELD, "toDate", toDate, ConditionRelation.LessEqual);
            
            if(filterByStartDate)
                conds.Add(LessonPlan.START_DATE_FIELD, "fromDate", fromDate, ConditionRelation.GreaterEqual);
            
            //if classId was set there is no need to filter by schoolYear ... possible case when class is no in the current schoolYear.
            if (classId.HasValue)
                conds.Add(LessonPlan.CLASS_REF_FIELD, classId);
            else
                conds.Add(LessonPlan.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId);           

            if (lpGalleryCategoryId.HasValue)
                conds.Add(LessonPlan.LP_GALERRY_CATEGORY_REF_FIELD, lpGalleryCategoryId);

            var dbQuery = SelectLessonPlan(conds, callerId);
            dbQuery = FilterLessonPlanByCallerId(dbQuery, callerId);

            if (studentId.HasValue)
            {
                dbQuery.Sql.Append($" and exists(select * from ClassPerson where PersonRef = {studentId} and ClassPerson.ClassRef = {LessonPlan.VW_LESSON_PLAN_NAME}.ClassRef)");
            }

            if (teacherId.HasValue)
            {
                dbQuery.Sql.Append($" and exists(select * from ClassTeacher where PersonRef = {teacherId} and ClassTeacher.ClassRef = {LessonPlan.VW_LESSON_PLAN_NAME}.ClassRef)");
            }
            return ReadMany<LessonPlan>(dbQuery);
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
                    var filterName = $"filter{i}";
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
        
        public IList<LessonPlan> GetLessonPlanTemplates(int? lpGalleryCategoryId, string titleFilter, int? classId, AnnouncementState? state, int callerId)
        {
            return GetLessonPlanTemplates(null, lpGalleryCategoryId, titleFilter, classId, state, callerId);
        }

        public LessonPlan GetLessonPlanTemplate(int lessonPlanId, int callerId)
        {
            return GetLessonPlanTemplates(lessonPlanId, null, null, null, null, callerId).FirstOrDefault();
        }

        protected IList<LessonPlan> GetLessonPlanTemplates(int? lessonPlanId, int? lpGalleryCategoryId, string titleFilter, int? classId, AnnouncementState? state, int callerId)
        {
            var conds = new AndQueryCondition { { LessonPlan.IN_GALLERY, 1, ConditionRelation.Equal } };
            if(lessonPlanId.HasValue)
                conds.Add(Announcement.ID_FIELD, lessonPlanId);
            if (state.HasValue)
                conds.Add(Announcement.STATE_FIELD, state);
            if (lpGalleryCategoryId.HasValue)
                conds.Add(LessonPlan.LP_GALERRY_CATEGORY_REF_FIELD, lpGalleryCategoryId);
            if (classId.HasValue)
                conds.Add(LessonPlan.CLASS_REF_FIELD, classId);
            var dbQuery = SelectLessonPlan(conds, callerId);

            dbQuery.Sql.AppendFormat(" and {0} like @{0}", Announcement.TITLE_FIELD);
            dbQuery.Parameters.Add(Announcement.TITLE_FIELD, string.Format(FILTER_FORMAT, titleFilter));
            return ReadMany<LessonPlan>(dbQuery);
        }

        protected DbQuery SelectLessonPlan(QueryCondition condition, int? callerId = null)
        {
            var dbQuery = new DbQuery();
            var classTeacherSql = "cast(0 as bit)";
            if (callerId.HasValue)
            {
                classTeacherSql = string.Format(@"(select count(*) from [{0}] where [{0}].[{1}] = {2}  and [{0}].[{3}] = [{4}].[{5}])",
                              "ClassTeacher", ClassTeacher.PERSON_REF_FIELD, callerId, ClassTeacher.CLASS_REF_FIELD,
                              LessonPlan.VW_LESSON_PLAN_NAME, LessonPlan.CLASS_REF_FIELD);

                classTeacherSql = $"cast(case when {nameof(LessonPlan.GalleryOwnerRef)} = {callerId} or {classTeacherSql} > 0 then 1 else 0 end as bit)";
            }

            var selectSet = $"{LessonPlan.VW_LESSON_PLAN_NAME}.*, {classTeacherSql} as IsOwner";
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, selectSet, LessonPlan.VW_LESSON_PLAN_NAME);
            condition.BuildSqlWhere(dbQuery, LessonPlan.VW_LESSON_PLAN_NAME);
            return dbQuery;
        }

        protected virtual DbQuery FilterLessonPlanByCallerId(DbQuery dbQuery, int callerId)
        {
            return dbQuery;
        }

        protected virtual AnnouncementQueryResult InternalGetAnnouncements(string procedureName, LessonPlansQuery query, IDictionary<string, object> additionalParams)
        {
            if(additionalParams == null)
                additionalParams = new Dictionary<string, object>();
            
            additionalParams.Add("schoolYearId", query.ClassId.HasValue ? (int?)null : schoolYearId);
            additionalParams.Add("classId", query.ClassId);

            var res = InternalGetAnnouncements<LessonPlansQuery>(procedureName, query, additionalParams);

            return res;
        }
        public AnnouncementQueryResult GetLessonPlansOrderedByDate(LessonPlansQuery query)
        {
            return InternalGetAnnouncements("spGetLessonPlansOrderedByDate", query, null);
        }
        public AnnouncementQueryResult GetLesonPlansOrderedByTitle(LessonPlansQuery query)
        {
            var ps = new Dictionary<string, object>
            {
                ["fromTitle"] = query.FromTitle,
                ["toTitle"] = query.ToTitle
            };
            return InternalGetAnnouncements("spGetLessonPlansOrderedByTitle", query, ps);
        }
        public AnnouncementQueryResult GetLesonPlansOrderedByClassName(LessonPlansQuery query)
        {
            var ps = new Dictionary<string, object>
            {
                ["fromClassName"] = query.FromClassName,
                ["toClassName"] = query.ToClassName
            };
            return InternalGetAnnouncements("spGetLessonPlansOrderedByClassName", query, ps);
        }


        public LessonPlan GetLastDraft(int personId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.STATE_FIELD, AnnouncementState.Draft},
                    {LessonPlan.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
                };
            var dbQuery = SelectLessonPlan(conds);
            var callerIdParam = "callerId";
            dbQuery.Sql.AppendFormat($" and ClassRef in (select ClassRef from ClassTeacher where ClassTeacher.PersonRef =@{callerIdParam})");
            dbQuery.Parameters.Add(callerIdParam, personId);
            Orm.OrderBy(dbQuery, LessonPlan.VW_LESSON_PLAN_NAME, Announcement.CREATED_FIELD, Orm.OrderType.Desc);
            return ReadOneOrNull<LessonPlan>(dbQuery);
        }

        public override bool CanAddStandard(int announcementId)
        {
            var dbQuery = new DbQuery();
            var classStandardTName = nameof(ClassStandard);
            var classTName = nameof(Class);
            dbQuery.Sql.AppendFormat(Orm.SELECT_COLUMN_FORMAT, Announcement.ID_FIELD, LessonPlan.VW_LESSON_PLAN_NAME)
                   .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, classTName, Class.ID_FIELD, LessonPlan.VW_LESSON_PLAN_NAME, LessonPlan.CLASS_REF_FIELD)
                   .AppendFormat(Orm.SIMPLE_JOIN_FORMAT, classStandardTName, ClassStandard.CLASS_REF_FIELD, classTName, Class.ID_FIELD)
                   .Append(" or ")
                   .Append($"[{classStandardTName}].{ClassStandard.CLASS_REF_FIELD} = [{classTName}].{Class.COURSE_REF_FIELD}");
            
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
                    {nameof(LessonPlan.InGallery), true }
                };
            if (excludedLessonPlanId.HasValue)
                conds.Add(Announcement.ID_FIELD, excludedLessonPlanId, ConditionRelation.NotEqual);
            var dbQuery = Orm.SimpleSelect(LessonPlan.VW_LESSON_PLAN_NAME, conds);
            return Exists(dbQuery);
        }

        /// <summary>
        /// Copies lesson plans and announcement standards.
        /// </summary>
        /// <returns>Key is SOURCE announcement id. Value is NEW announcement id.</returns>
        public IDictionary<int, int> CopyLessonPlansToClass(IList<int> lessonPlanIds, int toClassId, DateTime startDate,
            DateTime created)
        {
            lessonPlanIds = lessonPlanIds ?? new List<int>();

            var @params = new Dictionary<string, object>
            {
                ["lessonPlanIds"] = lessonPlanIds.Count == 0 ? new List<int>() : lessonPlanIds,
                ["toClassId"] = toClassId,
                ["startDate"] = startDate,
                ["created"] = created
            };

            IList<AnnouncementCopyResult> result;

            using (var reader = ExecuteStoredProcedureReader("spCopyLessonPlansToClass", @params))
            {
                result = reader.ReadList<AnnouncementCopyResult>();
            }

            return result.ToDictionary(x => x.FromAnnouncementId, y => y.ToAnnouncementId);
        }

        public void AdjustDates(IList<int> ids, DateTime startDate, int classId)
        {
            if (ids == null || ids.Count == 0)
                return;

            var @params = new Dictionary<string, object>
            {
                ["ids"] = ids,
                ["startDate"] = startDate,
                ["classId"] = classId
            };

            ExecuteStoredProcedure("spAdjustLessonPlanDates", @params);
        }
    }


    

    public class LessonPlansQuery : AnnouncementsQuery
    {
        public int? ClassId { get; set; }
        public int? TeacherId { get; set; }
        public int? StudentId { get; set; }
        public string FromClassName { get; set; }
        public string ToClassName { get; set; }
    }
}
