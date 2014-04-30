﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoDateStorage:BaseDemoStorage<int ,Date>
    {
        public DemoDateStorage(DemoStorage storage) : base(storage)
        {
        }

        public Date GetDateOrNull(DateQuery dateQuery)
        {
            return GetDatesFiltered(dateQuery).FirstOrDefault();
        }

        public DateTime GetDbDateTime()
        {
            return DateTime.Now;
        }

        public IList<Date> GetDates(DateQuery dateQuery)
        {
            return GetDatesFiltered(dateQuery);
        }

        public void Add(IList<Date> days)
        {
            foreach (var date in days)
            {
                Add(date);
            }
        }

        public void Delete(DateQuery query)
        {
            var dates = GetDatesFiltered(query);


            foreach (var date in dates)
            {
                var item = data.First(x => x.Value == date);
                data.Remove(item.Key);
            }
        }

        private List<Date> GetDatesFiltered(DateQuery query)
        {
            var dates = data.Select(x => x.Value);

            if (query.SchoolYearId.HasValue)
                dates = dates.Where(x => x.SchoolYearRef == query.SchoolYearId);
            if (query.FromDate.HasValue)
                dates = dates.Where(x => x.Day >= query.FromDate);
            if (query.ToDate.HasValue)
                dates = dates.Where(x => x.Day <= query.ToDate);
            if (query.SchoolDaysOnly)
                dates = dates.Where(x => x.IsSchoolDay);


            if (query.MarkingPeriodId.HasValue)
            {

                var mp = Storage.MarkingPeriodStorage.GetById(query.MarkingPeriodId.Value);
                dates = dates.Where(x => mp.StartDate <= x.Day && mp.EndDate >= x.Day);
            }

            if (query.DayType.HasValue)
                dates = dates.Where(x => x.DayType.Id == query.DayType);

            return dates.ToList();
        }

        public void Add(Date days)
        {
            data.Add(GetNextFreeId(), days);
        }

        public void Update(IList<Date> dates)
        {
            foreach (var date in dates)
            {
                var item = data.First(x => x.Value == date);
                data[item.Key] = date;
            }
        }



        private void AddMonth(int year, int month)
        {
            var daysCount = DateTime.DaysInMonth(year, month);

            for (var i = 0; i < daysCount; ++i)
            {
                var typeRef = 0;

                var day = new DateTime(year, month, i + 1);

                if (day.DayOfWeek == DayOfWeek.Thursday || day.DayOfWeek == DayOfWeek.Tuesday)
                    typeRef = 20;
                if (day.DayOfWeek == DayOfWeek.Monday)
                    typeRef = 19;
                if (day.DayOfWeek == DayOfWeek.Wednesday || day.DayOfWeek == DayOfWeek.Friday)
                    typeRef = 21;
                data.Add(GetNextFreeId(), new Date
                {
                    Day = day,
                    SchoolYearRef = 1,
                    SchoolRef = 1,
                    IsSchoolDay = day.DayOfWeek != DayOfWeek.Thursday && day.DayOfWeek != DayOfWeek.Friday,
                    DayTypeRef = typeRef
                });
            }
        }

        public override void Setup()
        {
            var currentYear = DateTime.Now.Year;
            for(var i = 1; i <= 12; ++i)
                AddMonth(currentYear, i);
        }
    }
}
