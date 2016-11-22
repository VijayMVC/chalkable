using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoDateStorage : BaseDemoIntStorage<Date>
    {
        public DemoDateStorage()
            : base(null, true)
        {
        }
    }

    public class DemoCalendarDateService : DemoSchoolServiceBase, ICalendarDateService
    {
        private DemoDateStorage DateStorage { get; set; }
        public DemoCalendarDateService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            DateStorage = new DemoDateStorage();
        }

        private List<Date> GetDatesFiltered(DateQuery query)
        {
            var dates = DateStorage.GetData().Select(x => x.Value);

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

                var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(query.MarkingPeriodId.Value);
                dates = dates.Where(x => mp.StartDate <= x.Day && mp.EndDate >= x.Day);
            }

            if (query.DayType.HasValue)
                dates = dates.Where(x => x.DayTypeRef == query.DayType);

            return dates.ToList();
        }

        public DateTime GetDbDateTime()
        {
            return DateTime.Now;
        }
        
        public IList<Date> GetLastDays(int schoolYearId, bool schoolDaysOnly, DateTime? fromDate, DateTime? tillDate, int count = Int32.MaxValue)
        {
            return GetDatesFiltered(new DateQuery
            {
                SchoolYearId = schoolYearId,
                FromDate = fromDate,
                ToDate = tillDate,
                Count = count,
                SchoolDaysOnly = schoolDaysOnly
            });
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



                DateStorage.Add(new Date
                {
                    Day = day,
                    SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                    IsSchoolDay = day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday,
                    DayTypeRef = typeRef,
                    BellScheduleRef = DemoSchoolConstants.BellScheduleId
                });
            }
        }

        public void AddDates()
        {
            var currentYear = DateTime.Now.Year;
            for (var i = 1; i <= 12; ++i)
                AddMonth(currentYear, i);
        }

        public void Add(IList<Date> days)
        {
            DateStorage.Add(days);
        }

        public void Edit(IList<Date> dates)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<Date> dates)
        {
            throw new NotImplementedException();
        }

        public void PrepareToDelete(IList<Date> dates)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Date> GetDates(DateQuery dateQuery)
        {
            return GetDatesFiltered(dateQuery);
        }
    }
}
