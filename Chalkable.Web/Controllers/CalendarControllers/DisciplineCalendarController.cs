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
    public class DiscilineCalendarController : CalendarController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult MonthForStudent(Guid studentId, DateTime? date)
        {
            DateTime start, end;
            MonthCalendar(ref date, out start, out end);
            var currentSchoolYearId = GetCurrentSchoolYearId();
            var classes = SchoolLocator.ClassService.GetClasses(currentSchoolYearId, null, null);
            var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(currentSchoolYearId, studentId, start, end);
            var res = PrepareMonthCalendar(start, end, date.Value, (time, b) => DisciplineMonthCalendarViewData.Create(time, b, classes,disciplines));
            return Json(res, 6);
        }
    }
}