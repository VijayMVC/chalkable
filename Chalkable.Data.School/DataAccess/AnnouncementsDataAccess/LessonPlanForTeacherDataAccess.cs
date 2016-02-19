using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class LessonPlanForTeacherDataAccess : LessonPlanDataAccess
    {
        public LessonPlanForTeacherDataAccess(UnitOfWork unitOfWork, int schoolYearId)
            : base(unitOfWork, schoolYearId)
        {
        }

        protected override DbQuery FilterLessonPlanByCallerId(DbQuery dbQuery, int callerId)
        {
            var callerIdParam = "callerId";
            dbQuery.Sql.AppendFormat($" and ClassRef in (select ClassRef from ClassTeacher where ClassTeacher.PersonRef =@{callerIdParam})");
            dbQuery.Parameters.Add(callerIdParam, callerId);
            return dbQuery;
        }
        protected override bool CanGetAllItems => false;

    }
}
