using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class ClassAnnouncementForTeacherDataAccess : ClassAnnouncementDataAccess
    {
        public ClassAnnouncementForTeacherDataAccess(UnitOfWork unitOfWork, int schoolYearId) : base(unitOfWork, schoolYearId)
        {
        }

        protected override DbQuery FilterClassAnnouncementByCaller(DbQuery dbQuery, int callerId)
        {
            dbQuery.Sql.Append(@" and (ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where ClassTeacher.PersonRef = @callerId))");
            dbQuery.Parameters.Add("callerId", callerId);
            return dbQuery;
        }

        protected override bool CanGetAllItems => false;
    }
}
