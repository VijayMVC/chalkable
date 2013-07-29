using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers.CalendarControllers
{
    public class CalendarController : ChalkableController
    {
        protected void WeekCalendar(ref DateTime? date, out DateTime start, out DateTime end)
        {
            date = date ?? SchoolLocator.Context.NowSchoolTime;
            var cal = new GregorianCalendar();
            var today = date.Value;
            start = cal.AddDays(today, -((int)today.DayOfWeek));
            end = cal.AddDays(start, 6);
        }

        protected void MonthCalendar(ref DateTime? date, out DateTime start, out DateTime end)
        {
            date = date ?? SchoolLocator.Context.NowSchoolTime;
            start = new DateTime(date.Value.Year, date.Value.Month, 1);
            end = start.AddMonths(1);
            while (start.DayOfWeek != DayOfWeek.Sunday)
                start = start.AddDays(-1);
            while (end.DayOfWeek != DayOfWeek.Saturday)
                end = end.AddDays(1);
        }

        protected IList<MonthCalendarViewData> PrepareMonthCalendar(DateTime start, DateTime end, DateTime currentDate,
                  Func<DateTime, bool, MonthCalendarViewData> createAction)
        {
            IList<MonthCalendarViewData> res = new List<MonthCalendarViewData>();
            do
            {
                res.Add(createAction(start, start.Month == currentDate.Month));
                start = start.AddDays(1);
            } while (start <= end);
            return res;
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult AssignDate(DateTime date, Guid sectionId)
        {
            var cdDate = SchoolLocator.CalendarDateService.GetCalendarDateByDate(date);
            if (SchoolLocator.CalendarDateService.CanAssignDate(cdDate.Id, sectionId))
            {
                SchoolLocator.CalendarDateService.AssignDate(cdDate.Id, sectionId);
                return Json(true);
            }
            return Json(false);
        }
    }
}