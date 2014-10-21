using System;
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

        public Period GetPeriodOrNull(int time, int schoolYearId)
        {
            if (data.Count(x => x.Value.StartTime >= time && x.Value.EndTime <= time && x.Value.SchoolYearRef == schoolYearId) == 1)
                return
                    data.First(
                        x =>
                            x.Value.StartTime >= time && x.Value.EndTime <= time &&
                            x.Value.SchoolYearRef == schoolYearId).Value;
            return null;
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            return data.Where( x => x.Value.SchoolYearRef == schoolYearId).Select( x => x.Value).ToList();
        }
    }
}
