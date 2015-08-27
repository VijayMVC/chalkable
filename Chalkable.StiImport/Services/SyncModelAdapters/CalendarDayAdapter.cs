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

        protected override void InsertInternal(IList<CalendarDay> entities)
        {
            var days = entities.ToList()
                .Select(x => new Date
                {
                    DayTypeRef = x.DayTypeID,
                    IsSchoolDay = x.InSchool,
                    SchoolYearRef = x.AcadSessionID,
                    BellScheduleRef = x.BellScheduleID,
                    Day = x.Date
                }).ToList();
            SchoolLocator.CalendarDateService.Add(days);
        }

        protected override void UpdateInternal(IList<CalendarDay> entities)
        {
            var ds = entities.Select(x => new Date
            {
                DayTypeRef = x.DayTypeID,
                IsSchoolDay = x.InSchool,
                BellScheduleRef = x.BellScheduleID,
                SchoolYearRef = x.AcadSessionID,
                Day = x.Date
            }).ToList();
            SchoolLocator.CalendarDateService.Edit(ds);
        }

        protected override void DeleteInternal(IList<CalendarDay> entities)
        {
            var dates = entities.Select(x => new Date
            {
                Day = x.Date,
                SchoolYearRef = x.AcadSessionID
            }).ToList();
            SchoolLocator.CalendarDateService.Delete(dates);
        }
    }
}