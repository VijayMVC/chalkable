using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradingScaleRangeDataAccess : DataAccessBase<GradingScaleRange, int>
    {
        public GradingScaleRangeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<GradingScaleRange> gradingScaleRanges)
        {
            SimpleDelete(gradingScaleRanges);
        }
    }
}
