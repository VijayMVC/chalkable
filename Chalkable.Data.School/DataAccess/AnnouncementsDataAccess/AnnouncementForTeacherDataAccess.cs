using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using System.Linq;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForTeacherDataAccess : ClassAnnouncementDataAccess
    {
        public AnnouncementForTeacherDataAccess(UnitOfWork unitOfWork, int schoolId) : base(unitOfWork, schoolId)
        {
        }
        private const string GET_TEACHER_ANNOUNCEMENTS = "spGetTeacherAnnouncements";
        private const string GRADED_ONLY_PARAM = "gradedOnly";
        private const string ALL_SCHOOL_ITEMS_PARAM = "allSchoolItems";
        private const string SIS_ACTIVITIES_IDS_PARAM = "sisActivitiesIds";
        private const string NOW_PARAM = "now";
        private const string OWNED_ONLY_PARAM = "ownedOnly";
        private const string MARKING_PERIOD_ID_PARAM = "markingPeriodId";
        private const string ROLE_ID_PARAM = "roleId";

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {ROLE_ID_PARAM, query.RoleId},
                    {MARKING_PERIOD_ID_PARAM, query.MarkingPeriodId},
                    {NOW_PARAM, query.Now},
                    {OWNED_ONLY_PARAM, query.OwnedOnly},
                    {GRADED_ONLY_PARAM, query.GradedOnly},
                    {ALL_SCHOOL_ITEMS_PARAM, query.AllSchoolItems},
                    {SIS_ACTIVITIES_IDS_PARAM, query.SisActivitiesIds != null ? query.SisActivitiesIds.Select(x => x.ToString()).JoinString(",") : null}
                };
            return GetAnnouncementsComplex(GET_TEACHER_ANNOUNCEMENTS, parameters, query);
        }

        public override Announcement GetAnnouncement(int id, int callerId)
        {
            var dbQuery = BuildSimpleAnnouncementQuery(callerId);
            (new AndQueryCondition
                {
                    { Announcement.ID_FIELD, id },
                    { Announcement.SCHOOL_REF_FIELD, schoolId },
                }).BuildSqlWhere(dbQuery, "Announcement");
            dbQuery.Sql.Append(@" and (Announcement.ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where ClassTeacher.PersonRef = @callerId))");
            dbQuery.Parameters.Add("callerId", callerId);
            return ReadOneOrNull<Announcement>(dbQuery);
        }


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

    }
}
