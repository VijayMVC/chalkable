using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model.Announcements;

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
            dbQuery.Sql.Append(" and ")
                .Append("(")
                .Append($" {LessonPlan.CLASS_REF_FIELD} is null ")
                .Append(" or ")
                .AppendFormat(
                    $" {LessonPlan.CLASS_REF_FIELD} in (select ClassRef from ClassTeacher where ClassTeacher.PersonRef =@{callerIdParam})")
                .Append(")");
            dbQuery.Parameters.Add(callerIdParam, callerId);
            return dbQuery;
        }
        protected override bool CanGetAllItems => false;

    }
}
