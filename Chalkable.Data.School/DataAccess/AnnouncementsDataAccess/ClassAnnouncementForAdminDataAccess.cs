using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class ClassAnnouncementForAdminDataAccess : ClassAnnouncementDataAccess
    {
        public ClassAnnouncementForAdminDataAccess(UnitOfWork unitOfWork, int schoolYearId)
            : base(unitOfWork, schoolYearId)
        {
        }

        protected override DbQuery FilterClassAnnouncementByCaller(DbQuery dbQuery, int callerId)
        {
            return dbQuery;
        }

        protected override bool CanGetAllItems => true;
    }
}
