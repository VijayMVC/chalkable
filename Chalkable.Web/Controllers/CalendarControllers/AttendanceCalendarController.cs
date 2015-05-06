using System;
using System.Web.Mvc;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers.CalendarControllers
{
    public class AttendanceCalendarController : CalendarController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult MonthForStudent(int studentId, DateTime? date)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            DateTime start, end;
            MonthCalendar(ref date, out start, out end);
            var studentAttendances = SchoolLocator.AttendanceService.GetStudentAttendancesByDateRange(studentId, start, end);
            var attendanceReasons = SchoolLocator.AttendanceReasonService.GetAll();
            var res = PrepareMonthCalendar(start, end, date.Value, (time, b) => AttendanceForStudentCalendarViewData.Create(time, b, Context.PersonId.Value, studentAttendances, attendanceReasons));
            return Json(res, 6);
        }
    }
}