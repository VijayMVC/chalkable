using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers.CalendarControllers
{
    [RequireHttps, TraceControllerFilter]
    public class DisciplineCalendarController : CalendarController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult MonthForStudent(int studentId, DateTime? date)
        {
            return FakeJson("~/fakeData/disciplinesCalendar.json");
            //DateTime start, end;
            //MonthCalendar(ref date, out start, out end);
            //var currentSchoolYearId = GetCurrentSchoolYearId();
            //var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(currentSchoolYearId, studentId, start, end);
            //var res = PrepareMonthCalendar(start, end, date.Value, (time, b) => DisciplineMonthCalendarViewData.Create(time, b, Context.UserId, disciplines));
            //return Json(res, 6);
        }
    }
}