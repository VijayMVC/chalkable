﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoMarkingPeriodStorage : BaseDemoStorage<int, MarkingPeriod>
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
            return data.Where(x => x.Value.StartDate >= date && date <= x.Value.EndDate).Select(x => x.Value).First();
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
            return data.Any(x => markingPeriodIds.Contains(x.Key));
        }

        public void DeleteMarkingPeriods(IList<int> markingPeriodIds)
        {
            foreach (var mp in markingPeriodIds)
            {
                Delete(mp);
            }
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
            var mp = GetById(markingPeriodId);

            return
                data.Where(x => x.Value.SchoolYearRef == mp.SchoolYearRef && x.Value.StartDate > mp.StartDate)
                    .Select(x => x.Value)
                    .First();
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

        public void Setup()
        {
            Add(new MarkingPeriod
            {
                Id = 1,
                Name = "Semester 2",
                Description = "",
                StartDate = new DateTime(2014, 1, 21),
                EndDate = new DateTime(2014, 5, 30),
                SchoolRef = 1,
                SchoolYearRef = 12,
                WeekDays = 62
            });

            Add(new MarkingPeriod
            {
                Id = 2,
                Name = "Semester 3",
                Description = "",
                StartDate = new DateTime(2014, 6, 30),
                EndDate = new DateTime(2014, 10, 30),
                SchoolRef = 1,
                SchoolYearRef = 12,
                WeekDays = 62
            });
        }
}
}
