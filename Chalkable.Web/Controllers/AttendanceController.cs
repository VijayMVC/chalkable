using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AttendanceController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher", Preference.API_DESCR_ATTENDANCE_SET_ATTENDANCE, true, CallType.Get, new[] { AppPermissionType.Attendance })]
        public ActionResult SetAttendance(Guid classschoolpersonid, Guid classgeneralperiodid, int type, Guid? attendanceReasonId, DateTime date)
        {
            SchoolLocator.AttendanceService.SetClassAttendance(classschoolpersonid, classgeneralperiodid, date, (AttendanceTypeEnum)type, attendanceReasonId);
            return Json(true);
        }

        //TODO : think how to rewrite for better performence 
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult SetAttendanceForList(GuidList classPersonIds, GuidList classPeriodIds, IntList attendanceTypes, GuidList attReasons, DateTime date)
        {
            for (int i = 0; i < classPersonIds.Count; ++i)
            {
                var classSchoolPersonId = classPersonIds[i];
                var attendanceType = attendanceTypes[i];
                var reason = attReasons != null ? (attReasons[i] != Guid.Empty ? attReasons[i] : (Guid?)null) : null;
                var periodId = classPeriodIds[i];
                SchoolLocator.AttendanceService.SetClassAttendance(classSchoolPersonId, periodId, date, (AttendanceTypeEnum)attendanceType, reason);
            }
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher", Preference.API_DESCR_ATTENDANCE_SET_ATTENDANCE_FOR_CLASS, true, CallType.Post, new[] { AppPermissionType.Attendance })]
        public ActionResult SetAttendanceForClass(int type, Guid? attendanceReason, Guid classGeneralPeriodId, DateTime date)
        {
            SchoolLocator.AttendanceService.SetAttendanceForClass(classGeneralPeriodId, date, (AttendanceTypeEnum)type, attendanceReason);
            return Json(true);
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_ATTENDANCE_LIST_CLASS_ATTENDANCE, true, CallType.Get, new[] { AppPermissionType.Schedule, AppPermissionType.Class })]
        public ActionResult ClassList(DateTime? date, Guid classId)
        {
            date = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(date.Value);
            if (mp == null)
                throw new NoMarkingPeriodException();
           
            var attendanceReason = SchoolLocator.AttendanceReasonService.List();
            var teacherId = SchoolLocator.Context.Role == CoreRoles.TEACHER_ROLE ? SchoolLocator.Context.UserId : default(Guid?);
            var cps = SchoolLocator.ClassPeriodService.GetClassPeriods(date.Value, classId, null, null, teacherId);
            var cp = cps.OrderBy(x => x.Period.StartTime).LastOrDefault();
            var listClassAttendance = new List<ClassAttendanceViewData>();
            if (cp != null)
            {
                var query = new ClassAttendanceQuery
                    {
                        MarkingPeriodId = mp.Id,
                        ClassId = classId,
                        FromDate = date.Value,
                        ToDate = date.Value,
                        FromTime = cp.Period.StartTime,
                        ToTime = cp.Period.EndTime,
                        NeedAllData = true
                    };
                var attendances = SchoolLocator.AttendanceService.GetClassAttendanceComplex(query);
                listClassAttendance = ClassAttendanceViewData.Create(attendances, attendanceReason).ToList();
            }
            listClassAttendance.Sort((x, y) => string.CompareOrdinal(x.Student.LastName, y.Student.LastName));
            return Json(new PaginatedList<ClassAttendanceViewData>(listClassAttendance, 0, int.MaxValue));
        }



        //[AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        //public ActionResult SetDailyAttendance(Guid personId, DateTime? date, int? timeIn, int? timeOut)
        //{
        //    var dateTime = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
        //    var cDate = SchoolLocator.CalendarDateService.GetCalendarDateByDate(dateTime);
        //    if(!(cDate.IsSchoolDay && cDate.ScheduleSectionRef.HasValue))
        //        throw new ChalkableException("Today is not school day");
        //    var studentDailyAttendance = SchoolLocator.AttendanceService.SetDailyAttendance(dateTime, personId, timeIn ?? NowTimeInMinutes, timeOut);
        //    return Json(DailyAttendanceViewData.Create(studentDailyAttendance));
        //}
    }
}