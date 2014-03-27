﻿using System.Collections.Generic;
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

        public Period GetPeriodOrNull(int time, int id)
        {
            throw new System.NotImplementedException();
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
    }
}
