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
            return data.Where(x => x.Value.StartDate >= date && date <= x.Value.EndDate).Select(x => x.Value).First();
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

        public override void Setup()
        {
            var currentYear = DateTime.Now.Year;

            Add(new MarkingPeriod
            {
                Id = DemoSchoolConstants.FirstMarkingPeriodId,
                Name = "Semester 1",
                Description = "",
                StartDate = new DateTime(currentYear, 1, 21),
                EndDate = new DateTime(currentYear, 5, 30),
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                WeekDays = 62
            });

            Add(new MarkingPeriod
            {
                Id = DemoSchoolConstants.SecondMarkingPeriodId,
                Name = "Semester 2",
                Description = "",
                StartDate = new DateTime(currentYear, 6, 30),
                EndDate = new DateTime(currentYear, 10, 30),
                SchoolRef = DemoSchoolConstants.SchoolId,
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                WeekDays = 62
            });
        }
}
}
