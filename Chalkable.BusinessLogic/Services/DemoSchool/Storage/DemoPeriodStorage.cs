using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPeriodStorage:BaseDemoStorage<int, Period>
    {
        public DemoPeriodStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(Period period)
        {
            if (!data.ContainsKey(period.Id))
                data[period.Id] = period;
        }

        public void Update(Period period)
        {
            if (data.ContainsKey(period.Id))
                data[period.Id] = period;
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

        public void Add(IList<Period> periods)
        {
            foreach (var period in periods)
            {
                Add(period);
            }
        }

        public IList<Period> Update(IList<Period> periods)
        {
            foreach (var period in periods)
            {
                Update(period);
            }
            return periods;
        }

        public IList<Period> RegeneratePeriods(IList<Guid> markingPeriodIds, int? startTime, int? length, int? lengthBetweenPeriods, int? periodCount)
        {
            throw new NotImplementedException();
        }

        public override void Setup()
        {
            Add(new Period
            {
                Id = GetNextFreeId(),
                StartTime = 615,
                EndTime = 659,
                Order = 1,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {
                Id = GetNextFreeId(),
                StartTime = 663,
                EndTime = 707,
                Order = 2,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {	
                Id = GetNextFreeId(),
                StartTime = 710,
                EndTime = 740,
                Order = 3,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {	
                Id = GetNextFreeId(),
                StartTime = 744,
                EndTime = 783,
                Order = 4,
                SchoolRef = 1,
                SchoolYearRef = 1
            });
            	
            Add(new Period
            {
                Id = GetNextFreeId(),
                StartTime = 787,
                EndTime = 826,
                Order = 5,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {	
                Id = GetNextFreeId(),
                StartTime = 830,
                EndTime = 869,
                Order = 6,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {	
                Id = GetNextFreeId(),
                StartTime = 873,
                EndTime = 912,
                Order = 7,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {	
                Id = GetNextFreeId(),
                StartTime = 916,
                EndTime = 955,
                Order = 8,
                SchoolRef = 1,
                SchoolYearRef = 1
            });

            Add(new Period
            {
                Id = GetNextFreeId(),
                StartTime = 959,
                EndTime = 1000,
                Order = 9,
                SchoolRef = 1,
                SchoolYearRef = 1
            });
        }
    }
}
