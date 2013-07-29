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
    [RequireHttps, TraceControllerFilter]
    public class AttendanceCalendarController : CalendarController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult MonthForClass(Guid classId, DateTime? date)
        {
            var type = AttendanceTypeEnum.Absent | AttendanceTypeEnum.Excused | AttendanceTypeEnum.Late;
            var attQuery = new ClassAttendanceQuery {ClassId = classId, Type = type};
            return AttendancesForMonth(date, attQuery, (dateTime, isCurrentMonth, atts) =>
               AttendanceForClassCalendarViewData.Create(dateTime, isCurrentMonth, classId, atts));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult MonthForPerson(Guid personId, DateTime? date)
        {
            var attQuery = new ClassAttendanceQuery { StudentId = personId};
            return AttendancesForMonth(date, attQuery, (dateTime, isCurrentMonth, atts) =>
                AttendanceForStudentCalendarViewData.Create(dateTime, isCurrentMonth, personId, atts));
        }


        private ActionResult AttendancesForMonth(DateTime? date, ClassAttendanceQuery query,
                Func<DateTime, bool, IList<ClassAttendanceDetails>, MonthCalendarViewData> createAction)
        {
            DateTime start, end;
            MonthCalendar(ref date, out start, out end);
            query.FromDate = start;
            query.ToDate = end;
            query.SchoolYearId = GetCurrentSchoolYearId();
            var attendances = SchoolLocator.AttendanceService.GetClassAttendanceComplex(query);
            var res = PrepareMonthCalendar(start, end, date.Value, (time, b) => createAction(time, b, attendances));
            return Json(res, 6);
        }
    }
}