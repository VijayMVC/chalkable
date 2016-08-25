using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
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
        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult SetAttendance(SetClassAttendanceViewData data)
        {
            SchoolLocator.AttendanceService.SetClassAttendances(data.Date, data.ClassId, data.Items.Select(x => new StudentClassAttendance
                        {
                            AttendanceReasonId = x.AttendanceReasonId,
                            ClassId = data.ClassId,
                            Date = data.Date,
                            StudentId = x.PersonId,
                            Level = x.Level,
                            IsDailyAttendancePeriod = x.IsDailyAttendancePeriod
                        }).ToList());
            MasterLocator.UserTrackingService.SetAttendance(Context.Login, data.ClassId);
            return Json(true);
        }
        //TODO: implement this method later
        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public async Task<ActionResult> NotTakenAttendanceClasses(DateTime? date)
        {
            date = (date ?? SchoolLocator.Context.NowSchoolYearTime);
            var notTakenAttendanceClasses = await SchoolLocator.AttendanceService.GetNotTakenAttendanceClasses(date.Value);
            return Json(ClassViewData.Create(notTakenAttendanceClasses));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher", true, new[] { AppPermissionType.Attendance })]
        public ActionResult SetAttendanceForClass(string level, int? attendanceReasonId, int classId, DateTime date, bool isDailyAttendancePeriod)
        {
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(date, true);
            if (mp == null)
            {
                throw new NoMarkingPeriodException("No marking period scheduled on this date");
            }

            var persons = SchoolLocator.StudentService.GetClassStudents(classId, mp.Id);
            SchoolLocator.AttendanceService.SetClassAttendances(date, classId, persons.Select(x => new StudentClassAttendance
            {
                AttendanceReasonId = attendanceReasonId,
                ClassId = classId,
                Date = date,
                StudentId = x.Id,
                Level = level,
                IsDailyAttendancePeriod = isDailyAttendancePeriod,
            }).ToList());
            return Json(true);
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher", true, new[] { AppPermissionType.Schedule, AppPermissionType.Class })]
        public ActionResult ClassList(DateTime? date, int classId)
        {
            date = (date ?? SchoolLocator.Context.NowSchoolYearTime).Date;
            return Json(new PaginatedList<StudentClassAttendanceOldViewData>(ClassAttendanceList(date.Value, classId), 0, int.MaxValue));
        }

        //TODO: add this to as api methods later
        /*public ActionResult ClassListNew(DateTime? date, int classId)
        {
            date = (date ?? SchoolLocator.Context.NowSchoolYearTime).Date;
            var attendance = SchoolLocator.AttendanceService.GetClassAttendance(date.Value, classId);
            if (attendance != null)
            {
                IList<AttendanceReason> attendanceReason = SchoolLocator.AttendanceReasonService.List();
                var res = ClassAttendanceViewData.Create(attendance, attendanceReason);
                res.StudentAttendances = res.StudentAttendances.OrderBy(x => x.Student.LastName).ToList();
                return Json(res);
            }
            return Json(ClassAttendanceViewData.Create(classId, date.Value));
        }
*/

        private IList<StudentClassAttendanceOldViewData> ClassAttendanceList(DateTime date, int classId)
        {
            var listClassAttendance = new List<StudentClassAttendanceOldViewData>();
            var attendance = SchoolLocator.AttendanceService.GetClassAttendance(date, classId);
            if (attendance != null)
            {
                IList<AttendanceReason> attendanceReason = SchoolLocator.AttendanceReasonService.List();
                listClassAttendance = StudentClassAttendanceOldViewData.Create(attendance, attendanceReason).ToList();
                listClassAttendance = listClassAttendance.OrderBy(x => x.Student.LastName).ThenBy(x => x.Student.FirstName).ToList();
            }
            return listClassAttendance;
        } 
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult GetAttendanceForStudent(DateTime? datetime, int studentId)
        {
            return FakeJson("~/fakeData/getAttendanceForStudent.json");
        }

        [AuthorizationFilter("Teacher", true, new[] { AppPermissionType.Attendance })]
        public async Task<ActionResult> AttendanceSummary(DateTime? date)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var schoolYearId = GetCurrentSchoolYearId();
            var d = date ?? Context.NowSchoolYearTime.Date;
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(schoolYearId, d);
            var attendanceSummary = await SchoolLocator.AttendanceService.GetAttendanceSummary(Context.PersonId.Value, gradingPeriod);
            return Json(TeacherAttendanceSummaryViewData.Create(attendanceSummary, d));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher", true, new[] { AppPermissionType.Attendance })]
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

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult PostSeatingChart(DateTime? date, SeatingChartInfo seatingChartInfo, bool needInfo)
        {
            var d = (date ?? Context.NowSchoolYearTime).Date;
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(d, true);
            SchoolLocator.AttendanceService.UpdateSeatingChart(seatingChartInfo.ClassId, mp.Id, seatingChartInfo);
            if (needInfo)
                return Json(GetSeatingChart(date, seatingChartInfo.ClassId));
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult AdminAttendanceSummary(bool renderNow, bool renderDay, bool renderMp, IntList gradeLevelsIds, 
            DateTime? nowDateTime, Guid? fromMarkingPeriodId, Guid? toMarkingPeriodId, DateTime? startDate, DateTime? endDate)
        {
            return FakeJson("~/fakeData/adminAttendanceSummary.json");
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult AttendanceMonthesList()
        {
            var syId = GetCurrentSchoolYearId();
            var res = SchoolLocator.AttendanceMonthService.GetAttendanceMonths(syId);
            return Json(AttendanceMonthViewData.Create(res));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Attendance })]
        public ActionResult StudentAttendance(int studentId, DateTime? date)
        {
            date = date ?? Context.NowSchoolTime;
            var attendanceReasons = SchoolLocator.AttendanceReasonService.GetAll();
            var studentAttendances = SchoolLocator.AttendanceService.GetStudentAttendancesByDateRange(studentId, date.Value, date.Value).FirstOrDefault();
            return Json(StudentDateAttendanceViewData.Create(studentAttendances, attendanceReasons), 6);
        }
    }
}