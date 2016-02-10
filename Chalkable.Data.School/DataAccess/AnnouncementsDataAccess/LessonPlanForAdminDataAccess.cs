using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class LessonPlanForAdminDataAccess : LessonPlanDataAccess
    {
        public LessonPlanForAdminDataAccess(UnitOfWork unitOfWork, int schoolYearId, bool? ownedOnly = null)
            : base(unitOfWork, schoolYearId)
        {
            CanGetAllItems = !ownedOnly ?? true;
        }

        protected override DbQuery FilterLessonPlanByCallerId(DbQuery dbQuery, int callerId)
        {
            return dbQuery;
        }

        protected override bool CanGetAllItems { get; }
    }

}
