using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradingScaleDataAccess : DataAccessBase<GradingScale, int>
    {
        public GradingScaleDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<GradingScaleRange> GetClassGradingScaleRanges(int classId)
        {
            var @params = new Dictionary<string, object>
            {
                ["classId"] = classId
            };

            var res = ExecuteStoredProcedureList<GradingScaleRange>("spGetClassGradingScaleRanges", @params);
            return res.Count == 0 ? DefaultGradingScaleRange() : res;
        }


        private static IList<GradingScaleRange> DefaultGradingScaleRange()
        {
            return new List<GradingScaleRange>
            {
                new GradingScaleRange
                {
                    LowValue = 0,
                    HighValue = 59,
                    AveragingEquivalent = 50,
                    IsPassing = false
                },
                new GradingScaleRange
                {
                    LowValue = 59,
                    HighValue = 69,
                    AveragingEquivalent = 65,
                    IsPassing = true
                },
                new GradingScaleRange
                {
                    LowValue = 69,
                    HighValue = 79,
                    AveragingEquivalent = 75,
                    IsPassing = true
                },
                new GradingScaleRange
                {
                    LowValue = 79,
                    HighValue = 89,
                    AveragingEquivalent = 85,
                    IsPassing = true
                },
                new GradingScaleRange
                {
                    LowValue = 89,
                    HighValue = 100,
                    AveragingEquivalent = 95,
                    IsPassing = true
                }
            };
        } 
    }
}
