using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoDayTypeStorage : BaseDemoIntStorage<DayType>
    {
        public DemoDayTypeStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public IList<DayType> GetDateTypes(int schoolYearId, int? fromNumber = null, int? toNumber = null)
        {
            var dayTypes = data.Select(x => x.Value).Where(x => x.SchoolYearRef == schoolYearId);

            if (fromNumber.HasValue)
                dayTypes = dayTypes.Where(x => x.Number >= fromNumber);
            if (toNumber.HasValue)
                dayTypes = dayTypes.Where(x => x.Number <= toNumber);

            return dayTypes.ToList();
        }

        public bool Exists(int schoolYearId)
        {
            return data.Count(x => x.Value.SchoolYearRef == schoolYearId) > 0;
        }
    }
}
