using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers.CalendarControllers
{
    public class CalendarController : ChalkableController
    {
        protected void WeekCalendar(ref DateTime? date, out DateTime start, out DateTime end)
        {
            WeekCalendar(ref date, out start, out end, Context);
        }

        protected static void WeekCalendar(ref DateTime? date, out DateTime start, out DateTime end, UserContext context)
        {
            date = date ?? context.NowSchoolTime;
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

        protected IList<DateTime> GetWeekDates(ref DateTime? date, out DateTime start, out DateTime end)
        {
            var cal = new GregorianCalendar();
            WeekCalendar(ref date, out start, out end);
            var res = new List<DateTime> {start};
            for (var i = 1; i < 7; i++)
            {
                res.Add(cal.AddDays(start, i));
            }
            return res;
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

        [AuthorizationFilter("Teacher")]
        public ActionResult TeacherClassWeek(DateTime? date, int classId)
        {
            DateTime start, end;
            WeekCalendar(ref date, out start, out end);

            var res = new List<TeacherClassWeekCalendarViewData>();
            var schoolYearId = GetCurrentSchoolYearId();
            var days = SchoolLocator.CalendarDateService.GetLastDays(schoolYearId, false, start, end);
            var classPeriods = SchoolLocator.ClassPeriodService.GetClassPeriods(schoolYearId, null, classId, null, null, null);

            for (var i = 0; i < days.Count; i++)
            {
                var d = days[i];
                if (d == null || !d.DayTypeRef.HasValue) continue;
                var currentClassPeriods = classPeriods.Where(x => x.DayTypeRef == d.DayTypeRef).ToList();
                res.Add(TeacherClassWeekCalendarViewData.Create(d.Day.Date, currentClassPeriods));
            }
            return Json(res);
        }
    }
}