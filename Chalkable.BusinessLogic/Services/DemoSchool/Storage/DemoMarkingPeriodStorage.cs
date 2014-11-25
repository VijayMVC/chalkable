using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{

    public class DemoMarkingPeriodStorage : BaseDemoIntStorage<MarkingPeriod>
    {
        public DemoMarkingPeriodStorage(DemoStorage storage) : base(storage, x => x.Id)
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
            return data.Where(x => x.Value.StartDate <= date && x.Value.EndDate >= date).Select(x => x.Value).First();
        }

        public bool IsOverlaped(int id, DateTime startDate, DateTime endDate, int? i)
        {
            return false;
        }

        public bool Exists(IList<int> markingPeriodIds)
        {
            return data.Any(x => markingPeriodIds.Contains(x.Key));
        }

        public void DeleteMarkingPeriods(IList<int> markingPeriodIds)
        {
            foreach (var mp in markingPeriodIds)
            {
                Delete(mp);
            }
        }

        public void ChangeWeekDays(IList<int> markingPeriodIds, int weekDays)
        {
            throw new NotImplementedException();
        }

        public MarkingPeriod GetNextInYear(int markingPeriodId)
        {
            var mp = GetById(markingPeriodId);

            return
                data.Where(x => x.Value.SchoolYearRef == mp.SchoolYearRef && x.Value.StartDate > mp.StartDate)
                    .Select(x => x.Value)
                    .First();
        }
    }   
}
