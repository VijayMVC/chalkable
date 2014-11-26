using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPeriodStorage:BaseDemoIntStorage<Period>
    {
        public DemoPeriodStorage(DemoStorage storage) : base(storage, x => x.Id, true)
        {
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            return data.Where( x => x.Value.SchoolYearRef == schoolYearId).Select( x => x.Value).ToList();
        }
    }
}
