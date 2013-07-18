using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers.CalendarControllers
{
    public class AttendanceCalendarController : CalendarController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult MonthForClass(Guid classId, DateTime? date)
        {
            DateTime start, end;
            MonthCalendar(ref date, out start, out end);
            var attQuery = new ClassAttendanceQuery
                {
                    ClassId = classId,
                    SchoolYearId = GetCurrentSchoolYearId(),
                    Type = AttendanceTypeEnum.Absent | AttendanceTypeEnum.Excused | AttendanceTypeEnum.Late,
                    FromDate = start,
                    ToDate = end
                };
            var attendance = SchoolLocator.AttendanceService.GetClassAttendanceComplex(attQuery);
            var res = PrepareMonthCalendar(start, end, date.Value
                , (time, b) => AttendanceForClassCalendarItemViewData.Create(time, b, classId, attendance));
            return Json(res, 6);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult MonthForPerson(Guid personId, DateTime? date)
        {
            DateTime start, end;
            MonthCalendar(ref date, out start, out end);
            var attQuery = new ClassAttendanceQuery
            {
                SchoolYearId = GetCurrentSchoolYearId(),
                FromDate = start,
                ToDate = end,
                StudentId = personId
            };
            var attendances = SchoolLocator.AttendanceService.GetClassAttendanceComplex(attQuery);
            var res = PrepareMonthCalendar(start, end, date.Value
                , (time, b) => AttendanceForStudentCalendarViewData.Create(time, b, personId, attendances));
            return Json(res, 6);
        }
    }
}