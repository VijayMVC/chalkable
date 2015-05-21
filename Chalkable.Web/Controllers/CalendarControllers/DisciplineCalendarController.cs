using System;
using System.Web.Mvc;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers.CalendarControllers
{
    [RequireHttps, TraceControllerFilter]
    public class DisciplineCalendarController : CalendarController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult MonthForStudent(int studentId, DateTime? date)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            DateTime start, end;
            MonthCalendar(ref date, out start, out end);
            var disciplines = SchoolLocator.DisciplineService.GetDisciplineByDateRange(studentId, start, end);
            var res = PrepareMonthCalendar(start, end, date.Value, (time, b) => DisciplineMonthCalendarViewData.Create(time, b, Context.PersonId.Value, disciplines));
            return Json(res, 6);
        }
    }
}