using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoMarkingPeriodStorage:BaseDemoStorage<int, MarkingPeriod>
    {
        public DemoMarkingPeriodStorage(DemoStorage storage) : base(storage)
        {
        }

        public MarkingPeriod GetLast(DateTime tillDate)
        {
            return data.First(x => x.Value.StartDate <= tillDate).Value;
        }

        public IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId)
        {
            return data.Where(x => x.Value.SchoolYearRef == schoolYearId).Select(x => x.Value).ToList();
        }

        public MarkingPeriod GetMarkingPeriod(DateTime date)
        {
            throw new NotImplementedException();
        }

        public void Add(MarkingPeriod mp)
        {
            if (!data.ContainsKey(mp.Id))
                data[mp.Id] = mp;
        }

        public bool IsOverlaped(int id, DateTime startDate, DateTime endDate, int? i)
        {
            throw new NotImplementedException();
        }

        public bool Exists(IList<int> markingPeriodIds)
        {
            throw new NotImplementedException();
        }

        public void DeleteMarkingPeriods(IList<int> markingPeriodIds)
        {
            throw new NotImplementedException();
        }

        public void Update(MarkingPeriod mp)
        {
            if (data.ContainsKey(mp.Id))
                data[mp.Id] = mp;
        }

        public void ChangeWeekDays(IList<int> markingPeriodIds, int weekDays)
        {
            throw new NotImplementedException();
        }

        public MarkingPeriod GetNextInYear(int markingPeriodId)
        {
            throw new NotImplementedException();
        }

        public IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods)
        {
            foreach (var mp in markingPeriods)
            {
                Add(mp);
            }
            return markingPeriods;
        }

        public IList<MarkingPeriod> Update(IList<MarkingPeriod> markingPeriods)
        {
            foreach (var mp in markingPeriods)
            {
                Update(mp);
            }
            return markingPeriods;
        }
    }
}
