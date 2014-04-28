using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradingScaleRangeDataAccess : DataAccessBase<GradingScaleRange, int>
    {
        public GradingScaleRangeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IDictionary<int, int> gsAgIds)
        {
            SimpleDelete(gsAgIds.Select(kv => new GradingScaleRange {GradingScaleRef = kv.Key, AlphaGradeRef = kv.Value}).ToList());
        }
    }
}
