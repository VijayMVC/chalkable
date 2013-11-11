using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AttendanceReasonDataAccess : DataAccessBase<AttendanceReason, int>
    {
        public AttendanceReasonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

    public class AttendanceLevelReasonDataAccess : DataAccessBase<AttendacneLevelReason, int>
    {
        public AttendanceLevelReasonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
