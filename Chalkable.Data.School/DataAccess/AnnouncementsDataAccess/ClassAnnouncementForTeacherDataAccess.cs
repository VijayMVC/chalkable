using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class ClassAnnouncementForTeacherDataAccess : ClassAnnouncementDataAccess
    {
        public ClassAnnouncementForTeacherDataAccess(UnitOfWork unitOfWork, int schoolYearId) : base(unitOfWork, schoolYearId)
        {
        }
        protected override DbQuery SeletClassAnnouncements(string tableName, int callerId)
        {
            var dbQuery = new DbQuery();
            var classTeacherSql = string.Format(@"(select cast(case when count(*) > 0 then 1 else 0 end as bit)
                                                     from [{0}] where [{0}].[{1}] = {2}  and [{0}].[{3}] = [{4}].[{5}])",
                                          "ClassTeacher", ClassTeacher.PERSON_REF_FIELD, callerId, ClassTeacher.CLASS_REF_FIELD,
                                          tableName, ClassAnnouncement.CLASS_REF_FIELD);
            var selectSet = string.Format("{0}.*, {1} as IsOwner", tableName, classTeacherSql);
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, selectSet, tableName);
            return dbQuery;
        }

        protected override DbQuery FilterClassAnnouncementByCaller(DbQuery dbQuery, int callerId)
        {
            dbQuery.Sql.Append(@" and (ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where ClassTeacher.PersonRef = @callerId))");
            dbQuery.Parameters.Add("callerId", callerId);
            return dbQuery;
        }
        
        public override ClassAnnouncement GetLastDraft(int personId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.STATE_FIELD, AnnouncementState.Draft},
                    {ClassAnnouncement.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
                };
            var dbQuery = SeletClassAnnouncements(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, personId);
            conds.BuildSqlWhere(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME);
            FilterClassAnnouncementByCaller(dbQuery, personId);
            Orm.OrderBy(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, Announcement.CREATED_FIELD, Orm.OrderType.Desc);
            return ReadOneOrNull<ClassAnnouncement>(dbQuery);
        }

    }
}
