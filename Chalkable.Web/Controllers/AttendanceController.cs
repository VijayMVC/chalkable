using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AttendancesViewData;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AttendanceController : ChalkableController
    {
        //[AuthorizationFilter("AdminGrade, AdminEdit, Teacher", Preference.API_DESCR_ATTENDANCE_SET_ATTENDANCE, true, CallType.Get, new[] { AppPermissionType.Attendance })]
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult SetAttendance(SetClassAttendanceViewData data)
        {
            SchoolLocator.AttendanceService.SetClassAttendances(data.Date, data.ClassId, data.Items.Select(x=>new ClassAttendance
                {
                    AttendanceReasonRef = x.AttendanceReasonId,
                    ClassRef = data.ClassId,
                    Date = data.Date,
                    PersonRef = x.PersonId,
                    Level = x.Level
                }).ToList());
            MasterLocator.UserTrackingService.SetAttendance(Context.Login, data.ClassId);
            return Json(true);
        }
        //TODO: implement this method later
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult NotTakenAttendanceClasses(DateTime? date)
        {
            date = (date ?? SchoolLocator.Context.NowSchoolYearTime);
            var notTakenAttendanceClasses = SchoolLocator.AttendanceService.GetNotTakenAttendanceClasses(date.Value);
            return Json(ClassViewData.Create(notTakenAttendanceClasses));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher", Preference.API_DESCR_ATTENDANCE_SET_ATTENDANCE_FOR_CLASS, true, CallType.Post, new[] { AppPermissionType.Attendance })]
        public ActionResult SetAttendanceForClass(string level, int? attendanceReasonId, int classId, DateTime date)
        {
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(date, true);
            if (mp == null)
            {
                throw new NoMarkingPeriodException("No marking period scheduled on this date");
            }

            var persons = SchoolLocator.StudentService.GetClassStudents(classId, mp.Id);
            SchoolLocator.AttendanceService.SetClassAttendances(date, classId, persons.Select(x => new ClassAttendance
            {
                AttendanceReasonRef = attendanceReasonId,
                ClassRef = classId,
                Date = date,
                PersonRef = x.Id,
                Level = level
            }).ToList());
            return Json(true);
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_ATTENDANCE_LIST_CLASS_ATTENDANCE, true, CallType.Get, new[] { AppPermissionType.Schedule, AppPermissionType.Class })]
        public ActionResult ClassList(DateTime? date, int classId)
        {
            date = (date ?? SchoolLocator.Context.NowSchoolYearTime).Date;
            return Json(new PaginatedList<ClassAttendanceViewData>(ClassAttendanceList(date.Value, classId), 0, int.MaxValue));
        }
        private IList<ClassAttendanceViewData> ClassAttendanceList(DateTime date, int classId)
        {
            var listClassAttendance = new List<ClassAttendanceViewData>();
            var attendances = SchoolLocator.AttendanceService.GetClassAttendances(date, classId);
            if (attendances != null)
            {
                IList<AttendanceReason> attendanceReason = SchoolLocator.AttendanceReasonService.List();
                listClassAttendance = ClassAttendanceViewData.Create(attendances, attendanceReason).ToList();
                listClassAttendance.Sort((x, y) => string.CompareOrdinal(x.Student.LastName, y.Student.LastName));
            }
            return listClassAttendance;
        } 
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetAttendanceForStudent(DateTime? datetime, int studentId)
        {
            return FakeJson("~/fakeData/getAttendanceForStudent.json");
        }

        [AuthorizationFilter("Teacher", Preference.API_DESCR_ATTENDANCE_SUMMARY, true, CallType.Get, new[] { AppPermissionType.Attendance })]
        public ActionResult AttendanceSummary(DateTime? date)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var schoolYearId = GetCurrentSchoolYearId();
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(schoolYearId, date ?? Context.NowSchoolYearTime.Date);
            var attendanceSummary = SchoolLocator.AttendanceService.GetAttendanceSummary(Context.PersonId.Value, gradingPeriod);
            return Json(TeacherAttendanceSummaryViewData.Create(attendanceSummary));
        }

        [AuthorizationFilter("Teacher", Preference.API_DESCR_ATTENDANCE_SEATING_CHART, true, CallType.Get, new[] { AppPermissionType.Attendance })]
        public ActionResult SeatingChart(DateTime? date, int classId)
        {
            return Json(GetSeatingChart(date, classId));
        }

        private AttendanceSeatingChartViewData GetSeatingChart(DateTime? date, int classId) {
            var d = (date ?? Context.NowSchoolYearTime).Date;
            var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(d, true);
            if (markingPeriod != null)
            {
                var seatingChart = SchoolLocator.AttendanceService.GetSeatingChart(classId, markingPeriod.Id);
                if (seatingChart != null)
                {
                    var attendances = ClassAttendanceList(d, classId);
                    var students = SchoolLocator.StudentService.GetClassStudents(classId, markingPeriod.Id, true);
                    return AttendanceSeatingChartViewData.Create(seatingChart, attendances, students);
                }    
            }
            
            return null;
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult PostSeatingChart(DateTime? date, SeatingChartInfo seatingChartInfo, Boolean needInfo)
        {
            var d = (date ?? Context.NowSchoolYearTime).Date;
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(d, true);
            SchoolLocator.AttendanceService.UpdateSeatingChart(seatingChartInfo.ClassId, mp.Id, seatingChartInfo);
            if (needInfo)
                return Json(GetSeatingChart(date, seatingChartInfo.ClassId));
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult AdminAttendanceSummary(bool renderNow, bool renderDay, bool renderMp, IntList gradeLevelsIds, 
            DateTime? nowDateTime, Guid? fromMarkingPeriodId, Guid? toMarkingPeriodId, DateTime? startDate, DateTime? endDate)
        {
            return FakeJson("~/fakeData/adminAttendanceSummary.json");
        }
    }
}