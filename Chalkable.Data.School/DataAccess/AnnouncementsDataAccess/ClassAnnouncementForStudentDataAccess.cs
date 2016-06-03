using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class ClassAnnouncementForStudentDataAccess : ClassAnnouncementDataAccess
    {
        public ClassAnnouncementForStudentDataAccess(UnitOfWork unitOfWork, int schoolYearId) : base(unitOfWork, schoolYearId)
        {
        }
        
        public override ClassAnnouncement GetLastDraft(int personId)
        {
            throw new ChalkableSecurityException("Student is not able to get drafts");
        }

        protected override DbQuery SelectClassAnnouncements(string tableName, int callerId)
        {
            var dbQuery = new DbQuery();
            var selectSet = $"{tableName}.*, cast(0 as bit) as IsOwner";
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

        protected override bool CanGetAllItems => false;
    }
}
