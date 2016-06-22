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
    public class SupplementalAnnouncementDataAccess : BaseAnnouncementDataAccess<SupplementalAnnouncement>
    {
        protected int _schoolYearId;

        public SupplementalAnnouncementDataAccess(UnitOfWork unitOfWork, int schoolYearId) : base(unitOfWork)
        {
            _schoolYearId = schoolYearId;
        }

        public override IList<SupplementalAnnouncement> GetByIds(IList<int> keys)
        {
            var query = $@"Select * 
                           From  {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}
                           Where {Announcement.ID_FIELD} in (Select * From @keys)";
            var @params = new Dictionary<string, object>
            {
                ["keys"] = keys
            };

            var dbQuery = new DbQuery(query, @params);

            return ReadMany<SupplementalAnnouncement>(dbQuery);
        }

        public override IList<SupplementalAnnouncement> GetAnnouncements(QueryCondition condition, int callerId)
        {

            var conds = new AndQueryCondition { condition, { LessonPlan.SCHOOL_SCHOOLYEAR_REF_FIELD, _schoolYearId } };
            var dbQuery = SelectSupplementalAnnouncement(conds, callerId);
            dbQuery = FilterSupplementalByCallerId(dbQuery, callerId);
            return ReadMany<SupplementalAnnouncement>(dbQuery);

            //var condition = new AndQueryCondition {conds, {nameof(SupplementalAnnouncement.SchoolYearRef), SchoolYearId} };
            //var isOwnerScript = 
            //    $@"Select
            //            Cast(Case When Count(*) > 0 Then 1 Else 0 End As bit)
            //       From 
            //            {nameof(ClassTeacher)} 
            //       Join {nameof(SupplementalAnnouncement)} 
            //            On {nameof(SupplementalAnnouncement)}.{SupplementalAnnouncement.CLASS_REF_FIELD} = {nameof(ClassTeacher)}.{ClassTeacher.CLASS_REF_FIELD}
            //       Where
            //            {nameof(ClassTeacher)}.{ClassTeacher.PERSON_REF_FIELD} = @callerId";

            //var query = 
            //    $@"Select {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}.*, ({isOwnerScript}) as IsOwner
            //       From   {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}";

            //var @params = new Dictionary<string, object>
            //{
            //    ["callerId"] = callerId
            //};

            //var dbQuery = new DbQuery(query, @params);
            //condition.BuildSqlWhere(dbQuery, SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT);

            //return ReadMany<SupplementalAnnouncement>(dbQuery);
        }

        protected virtual DbQuery FilterSupplementalByCallerId(DbQuery dbQuery, int callerId)
        {
            return dbQuery;
        }

        ///////////////////////////////////////////

        public AnnouncementQueryResult GetSupplementalAnnouncementOrderedByDate(SupplementalAnnouncementQuery query)
        {
            return InternalGetAnnouncements("spGetSupplementalAnnouncementsOrderedByDate", query, null);
        }
        public AnnouncementQueryResult GetSupplementalAnnouncementOrderedByTitle(SupplementalAnnouncementQuery query)
        {
            var ps = new Dictionary<string, object>
            {
                ["fromTitle"] = query.FromTitle,
                ["toTitle"] = query.ToTitle
            };
            return InternalGetAnnouncements("spGetSupplementalAnnouncementsOrderedByTitle", query, ps);
        }
        public AnnouncementQueryResult GetSupplementalAnnouncementOrderedByClassName(SupplementalAnnouncementQuery query)
        {
            var ps = new Dictionary<string, object>
            {
                ["fromClassName"] = query.FromClassName,
                ["toClassName"] = query.ToClassName
            };
            return InternalGetAnnouncements("spGetSupplementalAnnouncementsOrderedByClassName", query, ps);
        }

        protected virtual AnnouncementQueryResult InternalGetAnnouncements(string procedureName, SupplementalAnnouncementQuery query, IDictionary<string, object> additionalParams)
        {
            if (additionalParams == null)
                additionalParams = new Dictionary<string, object>();
            
            additionalParams.Add("schoolYearId", _schoolYearId);
            additionalParams.Add("classId", query.ClassId);
            additionalParams.Add("teacherId", query.TeacherId);
            additionalParams.Add("studentId", query.StudentId);

            return InternalGetAnnouncements<SupplementalAnnouncementQuery>(procedureName, query, additionalParams);
        }

        ///////////////////////////////////////////

        public override SupplementalAnnouncement GetAnnouncement(int id, int callerId)
        {
            return GetAnnouncements(new AndQueryCondition {{nameof(SupplementalAnnouncement.Id), id}}, callerId)
                    .FirstOrDefault();
        }

        public override bool CanAddStandard(int announcementId)
        {
            var query = $@"Select {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}.Id 
                           From {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}
                                Join {nameof(Class)} 
                                    on {nameof(Class)}.{Class.ID_FIELD} = {SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}.{SupplementalAnnouncement.CLASS_REF_FIELD}
                                Join {nameof(ClassStandard)} 
                                    on    {nameof(ClassStandard)}.{ClassStandard.CLASS_REF_FIELD} = {nameof(Class)}.{Class.ID_FIELD}
                                       Or {nameof(ClassStandard)}.{ClassStandard.CLASS_REF_FIELD} = {nameof(Class)}.{Class.COURSE_REF_FIELD}";

            var conds = new AndQueryCondition
            {
                {Announcement.ID_FIELD, announcementId },
                {SupplementalAnnouncement.SCHOOL_SCHOOLYEAR_REF_FIELD, _schoolYearId }
            };

            var dbQuery = new DbQuery(query, new Dictionary<string, object>());

            conds.BuildSqlWhere(dbQuery, SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT);
            return Exists(dbQuery);
        }

        public override IList<AnnouncementDetails> GetDetailses(IList<int> ids, int callerId, int? roleId, bool onlyOwner = true)
        {
            var @params = new Dictionary<string, object>
            {
                ["announcementIds"] = ids,
                ["callerId"] = callerId,
                ["callerRole"] = roleId,
                ["schoolYearId"] = _schoolYearId
            };

            using (var reader = ExecuteStoredProcedureReader("spGetListOfSupplementalAnnouncementDetailsByIds", @params))
            {
                return BuildGetDetailsesResult(reader);
            }
        }

        protected override bool CanGetAllItems => false;

        protected override SupplementalAnnouncement ReadAnnouncementData(AnnouncementComplex announcement, SqlDataReader reader)
        {
            var res = reader.Read<SupplementalAnnouncement>();
            return res;
        }

        public AnnouncementDetails Create(int classId, DateTime created, DateTime expiresDate, int personId, int classAnnouncementTypeId)
        {
            var @params = new Dictionary<string, object>
            {
                ["created"] = created,
                ["expires"] = expiresDate,
                ["classId"] = classId,
                ["classAnnouncementTypeId"] = classAnnouncementTypeId,
                ["personId"] = personId,
                ["state"] = AnnouncementState.Draft
            };

            using (var reader = ExecuteStoredProcedureReader("spCreateSupplementalAnnouncement", @params))
            {
                return BuildGetDetailsResult(reader);
            }
        }

        public SupplementalAnnouncement GetLastDraft(int personId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.STATE_FIELD, AnnouncementState.Draft},
                    {SupplementalAnnouncement.SCHOOL_SCHOOLYEAR_REF_FIELD, _schoolYearId}
                };
            var dbQuery = SelectSupplementalAnnouncement(conds);
            var callerIdParam = "callerId";
            dbQuery.Sql.AppendFormat($" and ClassRef in (select ClassRef from ClassTeacher where ClassTeacher.PersonRef =@{callerIdParam})");
            dbQuery.Parameters.Add(callerIdParam, personId);
            Orm.OrderBy(dbQuery, SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT, Announcement.CREATED_FIELD, Orm.OrderType.Desc);
            return ReadOneOrNull<SupplementalAnnouncement>(dbQuery);
        }

        protected DbQuery SelectSupplementalAnnouncement(QueryCondition condition, int? callerId = null)
        {
            var dbQuery = new DbQuery();

            var ownerQ = new DbQuery();
            if (!callerId.HasValue)
            {
                ownerQ.Sql.Append("cast(0 as bit)");
            }
            else
            {
                ownerQ.Sql
                    .AppendFormat(Orm.SELECT_FORMAT, "cast(case when count(*) > 0 then 1 else 0 end as bit)",
                        nameof(ClassTeacher))
                    .Append(" Where ")
                    .Append($"[{nameof(ClassTeacher)}].[{nameof(ClassTeacher.PersonRef)}] = {callerId}")
                    .Append(" and ")
                    .Append($"[{nameof(ClassTeacher)}].[{nameof(ClassTeacher.ClassRef)}]").Append("=")
                    .Append($"[{SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}].[{nameof(SupplementalAnnouncement.ClassRef)}]");
            }
            
            var selectSet = $"{SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT}.*, ({ownerQ.Sql}) as IsOwner";
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, selectSet, SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT);
            condition.BuildSqlWhere(dbQuery, SupplementalAnnouncement.VW_SUPPLEMENTAL_ANNOUNCEMENT);
            return dbQuery;
        }

        public override void Update(SupplementalAnnouncement entity)
        {
            SimpleUpdate<Announcement>(entity);
            base.Update(entity);
        }
    }


    public class SupplementalAnnouncementForTeacherDataAccess : SupplementalAnnouncementDataAccess
    {
        public SupplementalAnnouncementForTeacherDataAccess(UnitOfWork unitOfWork, int schoolYearId) : base(unitOfWork, schoolYearId)
        {
        }

        protected override DbQuery FilterSupplementalByCallerId(DbQuery dbQuery, int callerId)
        {
            var callerIdParam = "callerId";

            var innerQ = new DbQuery();
            innerQ.Sql.AppendFormat(Orm.SELECT_FORMAT,
                nameof(ClassTeacher.ClassRef),
                nameof(ClassTeacher));
            var conds = new AndQueryCondition { { nameof(ClassTeacher.PersonRef), callerIdParam, callerId, ConditionRelation.Equal } };
            conds.BuildSqlWhere(innerQ, nameof(ClassTeacher));

            dbQuery.Sql.AppendFormat($" and {nameof(SupplementalAnnouncement.ClassRef)} in ({innerQ.Sql})");
            dbQuery.Parameters.Add(callerIdParam, callerId);
            return dbQuery;
        }
        protected override bool CanGetAllItems => false;

    }

    public class SupplementalAnnouncementForStudentDataAccess : SupplementalAnnouncementDataAccess
    {
        public SupplementalAnnouncementForStudentDataAccess(UnitOfWork unitOfWork, int schoolYearId) : base(unitOfWork, schoolYearId)
        {
        }

        protected override bool CanGetAllItems => false;

        protected override DbQuery FilterSupplementalByCallerId(DbQuery dbQuery, int callerId)
        {
            var callerIdParam = "callerId";

            var innerQ = new DbQuery();
            innerQ.Sql.AppendFormat(Orm.SELECT_FORMAT, 
                nameof(SupplementalAnnouncementRecipient.SupplementalAnnouncementRef),
                nameof(SupplementalAnnouncementRecipient));
            var conds = new AndQueryCondition {{nameof(SupplementalAnnouncementRecipient.StudentRef), callerIdParam, callerId, ConditionRelation.Equal}};
            conds.BuildSqlWhere(innerQ, nameof(SupplementalAnnouncementRecipient));

            dbQuery.Sql.Append($" and {nameof(SupplementalAnnouncement.Id)} in ({innerQ.Sql})");
            dbQuery.Parameters.Add(callerIdParam, callerId);

            dbQuery.Sql.AppendFormat($" and {SupplementalAnnouncement.VISIBLE_FOR_STUDENT_FIELD} = 1");
            return dbQuery;
        }
    }

    public class SupplementalAnnouncementForAdminDataAccess : SupplementalAnnouncementDataAccess
    {
        public SupplementalAnnouncementForAdminDataAccess(UnitOfWork unitOfWork, int schoolYearId, bool? ownedOnly) : base(unitOfWork, schoolYearId)
        {
            CanGetAllItems = !ownedOnly ?? true;
        }
        protected override DbQuery FilterSupplementalByCallerId(DbQuery dbQuery, int callerId)
        {
            return dbQuery;
        }
        protected override bool CanGetAllItems { get; }
    }



    public class SupplementalAnnouncementQuery : AnnouncementsQuery
    {
        public int? TeacherId { get; set; }
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public string FromClassName { get; set; }
        public string ToClassName { get; set; }
    }
}
