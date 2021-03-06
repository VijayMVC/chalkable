﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
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
            date = date ?? context.NowSchoolYearTime;
            var cal = new GregorianCalendar();
            var today = date.Value;
            start = cal.AddDays(today, -((int)today.DayOfWeek)).Date;
            end = cal.AddDays(start, 6).Date;
        }


        protected void MonthCalendar(ref DateTime? date, out DateTime start, out DateTime end)
        {
            date = date ?? SchoolLocator.Context.NowSchoolYearTime;
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

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Schedule})]
        public ActionResult IsSchoolDay(DateTime? dateTime)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var day = (dateTime ?? DateTime.Now).Date;
            var schoodDays = SchoolLocator.CalendarDateService.GetLastDays(Context.SchoolYearId.Value, true, day, day);
            return Json(schoodDays.Count > 0);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] {AppPermissionType.Schedule})]
        public ActionResult SchoolDaysCount(DateTime fromDate, DateTime toDate)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var schoodDays = SchoolLocator.CalendarDateService.GetLastDays(Context.SchoolYearId.Value, true, fromDate, toDate);
            return Json(schoodDays.Count);
        }
    }
}