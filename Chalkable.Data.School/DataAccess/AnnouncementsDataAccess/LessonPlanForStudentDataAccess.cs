using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class LessonPlanForStudentDataAccess : LessonPlanDataAccess
    {
        public LessonPlanForStudentDataAccess(UnitOfWork unitOfWork, int schoolYearId)
            : base(unitOfWork, schoolYearId)
        {
        }

        protected override bool CanGetAllItems => false;

        protected override DbQuery FilterLessonPlanByCallerId(DbQuery dbQuery, int callerId)
        {
            var callerIdParam = "callerId";
            dbQuery.Sql.Append($" and ClassRef in (select ClassRef from ClassPerson where ClassPerson.PersonRef =@{callerIdParam})");
            dbQuery.Sql.AppendFormat($" and {LessonPlan.VISIBLE_FOR_STUDENT_FIELD} = 1");
            dbQuery.Parameters.Add(callerIdParam, callerId);
            return dbQuery;
        }
    }
}
