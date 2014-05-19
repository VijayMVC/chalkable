using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
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

        public IList<Period> RegeneratePeriods(IList<Guid> markingPeriodIds, int? startTime, int? length, int? lengthBetweenPeriods, int? periodCount)
        {
            throw new NotImplementedException();
        }

        public override void Setup()
        {
            Add(new Period
            {
                StartTime = 615,
                EndTime = 659,
                Order = 1,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {
                StartTime = 663,
                EndTime = 707,
                Order = 2,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {	
                StartTime = 710,
                EndTime = 740,
                Order = 3,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {	
                StartTime = 744,
                EndTime = 783,
                Order = 4,
                SchoolRef = 1,
                SchoolYearRef = 1
            });
            	
            Add(new Period
            {
                StartTime = 787,
                EndTime = 826,
                Order = 5,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {	
                StartTime = 830,
                EndTime = 869,
                Order = 6,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {	
                StartTime = 873,
                EndTime = 912,
                Order = 7,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {	
                StartTime = 916,
                EndTime = 955,
                Order = 8,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {
                StartTime = 959,
                EndTime = 1000,
                Order = 9,
                SchoolRef = 1,
                SchoolYearRef = 1
            });
        }
    }
}
