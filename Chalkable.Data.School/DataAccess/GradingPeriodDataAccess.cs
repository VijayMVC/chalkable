using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradingPeriodDataAccess : DataAccessBase<GradingPeriod, int>
    {
        public GradingPeriodDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
