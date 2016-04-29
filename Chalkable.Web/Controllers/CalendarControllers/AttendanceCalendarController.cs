using System;
using System.Web.Mvc;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers.CalendarControllers
{
    public class AttendanceCalendarController : CalendarController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
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

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult StudentAttendances(int studentId, DateTime? date)
        {
            date = date ?? Context.NowSchoolYearTime;
            var studentAttendances = SchoolLocator.AttendanceService.GetStudentAttendancesByDateRange(studentId, date.Value, date.Value);
            return Json(studentAttendances, 6);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult MonthForClass(int classId, DateTime? date)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            DateTime start, end;
            MonthCalendar(ref date, out start, out end);
            var classAttendnances = SchoolLocator.AttendanceService.GetClassPeriodAttendances(classId, start, end);
            var res = PrepareMonthCalendar(start, end, date.Value, (time, b) => AttendanceForClassCalendarViewData.Create(time, b, classId, classAttendnances));
            return Json(res, 6);
        }
    }
}