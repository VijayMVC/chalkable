using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class CalendarDayAdapter : SyncModelAdapter<CalendarDay>
    {
        public CalendarDayAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Date Selector(CalendarDay x)
        {
            return new Date
            {
                DayTypeRef = x.DayTypeID,
                IsSchoolDay = x.InSchool,
                SchoolYearRef = x.AcadSessionID,
                BellScheduleRef = x.BellScheduleID,
                Day = x.Date
            };
        }

        protected override void InsertInternal(IList<CalendarDay> entities)
        {
            var days = entities.ToList().Select(Selector).ToList();
            ServiceLocatorSchool.CalendarDateService.Add(days);
        }

        protected override void UpdateInternal(IList<CalendarDay> entities)
        {
            var ds = entities.Select(Selector).ToList();
            ServiceLocatorSchool.CalendarDateService.Edit(ds);
        }

        protected override void DeleteInternal(IList<CalendarDay> entities)
        {
            var dates = entities.Select(x => new Date
            {
                Day = x.Date,
                SchoolYearRef = x.AcadSessionID
            }).ToList();
            ServiceLocatorSchool.CalendarDateService.Delete(dates);
        }

        protected override void PrepareToDeleteInternal(IList<CalendarDay> entities)
        {
            var dates = entities.Select(x => new Date
            {
                Day = x.Date,
                SchoolYearRef = x.AcadSessionID
            }).ToList();
            ServiceLocatorSchool.CalendarDateService.PrepareToDelete(dates);
        }
    }
}