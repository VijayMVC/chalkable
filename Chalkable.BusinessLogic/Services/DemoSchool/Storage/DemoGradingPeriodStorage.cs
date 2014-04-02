using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradingPeriodStorage:BaseDemoStorage<int, GradingPeriod>
    {
        public DemoGradingPeriodStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<GradingPeriodDetails> GetGradingPeriodDetails(int schoolYearId, int? markingPeriodId)
        {
            var gradingPeriods =
                data.Where(x => x.Value.SchoolYearRef == schoolYearId && x.Value.MarkingPeriodRef == markingPeriodId)
                    .ToList();

            //convert to marking period details

            return null;
        }
    }
}
