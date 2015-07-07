using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForStudentDataAccess : ClassAnnouncementDataAccess
    {
        public AnnouncementForStudentDataAccess(UnitOfWork unitOfWork, int schoolId) : base(unitOfWork, schoolId)
        {
        }
        
        public override ClassAnnouncement GetLastDraft(int personId)
        {
            throw new System.NotImplementedException();
        }

        protected override DbQuery SeletClassAnnouncements(string tableName, int callerId)
        {
            var dbQuery = new DbQuery();
            var selectSet = string.Format("{0}.*, cast(0 as bit) as IsOwner", tableName);
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, selectSet, tableName);
            return dbQuery;
        }

        protected override DbQuery FilterClassAnnouncementByCaller(DbQuery dbQuery, int callerId)
        {
            dbQuery.Sql.Append(@" and (ClassRef in (select ClassPerson.ClassRef from ClassPerson where ClassPerson.PersonRef = @callerId))");
            dbQuery.Sql.AppendFormat(" and {0} = 1 ", LessonPlan.VISIBLE_FOR_STUDENT_FIELD);
            dbQuery.Parameters.Add("callerId", callerId);
            return dbQuery;
        }
    }
}
