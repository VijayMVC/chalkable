using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AttendancesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AttendanceController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher", Preference.API_DESCR_ATTENDANCE_SET_ATTENDANCE, true, CallType.Get, new[] { AppPermissionType.Attendance })]
        public ActionResult SetAttendance(Guid classPersonid, Guid classPeriodId, int type, Guid? attendanceReasonId, DateTime date)
        {
            SchoolLocator.AttendanceService.SetClassAttendance(classPersonid, classPeriodId, date, (AttendanceTypeEnum)type, attendanceReasonId);
            return Json(true);
        }

        //TODO : think how to rewrite for better performence 
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult SetAttendanceForList(GuidList classPersonIds, GuidList classPeriodIds, IntList attendanceTypes, GuidList attReasons, DateTime date)
        {
            for (int i = 0; i < classPersonIds.Count; ++i)
            {
                var classPersonId = classPersonIds[i];
                var attendanceType = attendanceTypes[i];
                var reason = attReasons != null ? (attReasons[i] != Guid.Empty ? attReasons[i] : (Guid?)null) : null;
                var periodId = classPeriodIds[i];
                SchoolLocator.AttendanceService.SetClassAttendance(classPersonId, periodId, date, (AttendanceTypeEnum)attendanceType, reason);
            }
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher", Preference.API_DESCR_ATTENDANCE_SET_ATTENDANCE_FOR_CLASS, true, CallType.Post, new[] { AppPermissionType.Attendance })]
        public ActionResult SetAttendanceForClass(int type, Guid? attendanceReason, Guid classPeriodId, DateTime date)
        {
            SchoolLocator.AttendanceService.SetAttendanceForClass(classPeriodId, date, (AttendanceTypeEnum)type, attendanceReason);
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
                var attendances = SchoolLocator.AttendanceService.GetClassAttendanceDetails(query);
                listClassAttendance = ClassAttendanceViewData.Create(attendances, attendanceReason).ToList();
            }
            listClassAttendance.Sort((x, y) => string.CompareOrdinal(x.Student.LastName, y.Student.LastName));
            return Json(new PaginatedList<ClassAttendanceViewData>(listClassAttendance, 0, int.MaxValue));
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher, Checkin")]
        public ActionResult SetDailyAttendance(Guid personId, DateTime? date, int? timeIn, int? timeOut)
        {
            var dateTime = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            var cDate = SchoolLocator.CalendarDateService.GetCalendarDateByDate(dateTime);
            if (!(cDate.IsSchoolDay && cDate.ScheduleSectionRef.HasValue))
                throw new ChalkableException("Today is not school day");
            var studentDailyAttendance = SchoolLocator.AttendanceService.SetDailyAttendance(dateTime, personId, timeIn ?? NowTimeInMinutes, timeOut);
            var person = SchoolLocator.PersonService.GetPerson(personId);
            return Json(DailyAttendanceViewData.Create(studentDailyAttendance, person));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher, Checkin")]
        public ActionResult SwipeCard(Guid personId, Guid classPeriodId)
        {
            var now = SchoolLocator.Context.NowSchoolTime;
            var cDate = SchoolLocator.CalendarDateService.GetCalendarDateByDate(now.Date);
            if (!(cDate.IsSchoolDay && cDate.ScheduleSectionRef.HasValue))
                throw new ChalkableException("Today is not school day");

            var att = SchoolLocator.AttendanceService.SwipeCard(personId, now, classPeriodId);
            var dailyAttendace = SchoolLocator.AttendanceService.GetDailyAttendance(now.Date, personId);
            return Json(DailyAttendanceViewData.Create(dailyAttendace, att.Student));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetAttendanceForStudent(DateTime? datetime, Guid studentId)
        {
            if (!SchoolLocator.Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            var date = (datetime ?? SchoolLocator.Context.NowSchoolTime).Date;
            var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(date);
            var query = new ClassAttendanceQuery
                {
                    MarkingPeriodId = markingPeriod.Id,
                    FromDate = date,
                    ToDate = date,
                    StudentId = studentId,
                    NeedAllData = true
                };
            var attendances = SchoolLocator.AttendanceService.GetClassAttendanceDetails(query);
            var dailAttendance = SchoolLocator.AttendanceService.GetDailyAttendance(date, studentId);
            var student = attendances.Count > 0 ? attendances.First().Student : SchoolLocator.PersonService.GetPerson(studentId);
            return Json(StudentAttendanceViewData.Create(student, attendances, dailAttendance));
        }

        [AuthorizationFilter("Teacher", Preference.API_DESCR_ATTENDANCE_SUMMARY, true, CallType.Get, new[] { AppPermissionType.Attendance })]
        public ActionResult AttendanceSummary(DateTime? date)
        {
            date = date ?? SchoolLocator.Context.NowSchoolTime;
            var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(date.Value);
            if (markingPeriod == null)
                throw new NoMarkingPeriodException();
            var trouble = new List<Person>();
            var well = new List<Person>();
            var query = new ClassAttendanceQuery
            {
                MarkingPeriodId = markingPeriod.Id,
                FromDate = date,
                ToDate = date,
                TeacherId = SchoolLocator.Context.UserId,
                Type = AttendanceTypeEnum.Absent | AttendanceTypeEnum.Excused | AttendanceTypeEnum.Late | AttendanceTypeEnum.Present
            };
            var studentsList = SchoolLocator.PersonService.GetPersons().Where(x=>x.RoleRef == CoreRoles.STUDENT_ROLE.Id).ToList();


            var all = SchoolLocator.AttendanceService.GetClassAttendanceDetails(query);
            foreach (var student in studentsList)
            {
                var attendances = all.Where(x => x.Student.Id == student.Id).ToList();
                var stat = attendances.Where(t => t.Type == AttendanceTypeEnum.Absent || t.Type == AttendanceTypeEnum.Late).ToList();
                if (stat.Count >= 5)
                    trouble.Add(student);
                else if (stat.Count <= 1)
                    well.Add(student);
            }
            
            return Json(AttendanceSummaryViewData.Create(trouble, well, all, markingPeriod), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult AdminAttendanceSummary(bool renderNow, bool renderDay, bool renderMp, GuidList gradeLevelsIds, 
            DateTime? nowDateTime, Guid? fromMarkingPeriodId, Guid? toMarkingPeriodId, DateTime? startDate, DateTime? endDate)
        {
            var date = (nowDateTime ?? Context.NowSchoolTime).Date;
            var res = new AdminAttendanceSummaryViewData();
            
            var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(date.Date);
            renderNow = renderNow && markingPeriod != null;
            IList<Person> allStudents = SchoolLocator.PersonService.GetPaginatedPersons(new PersonQuery {RoleId = CoreRoles.STUDENT_ROLE.Id});

            if (renderNow)
                res.NowAttendanceData = GetNowAttendanceData(gradeLevelsIds, allStudents);
            if (renderDay)
                res.AttendanceByDayData = GetAdminAttendanceByDate(gradeLevelsIds, markingPeriod, date, allStudents);
            if (renderMp)
                res.AttendanceByMpData = GetAdminAttendanceByMp(gradeLevelsIds, fromMarkingPeriodId, toMarkingPeriodId, startDate, endDate, allStudents);
            return Json(res, 8);
        }
        
        private NowAttendanceViewData GetNowAttendanceData(GuidList gradeLevelsIds, IList<Person> students)
        {
            var date = Context.NowSchoolTime.Date;
            var period = SchoolLocator.PeriodService.GetPeriod(NowTimeInMinutes, date);
            var sy = SchoolLocator.SchoolYearService.GetCurrentSchoolYear();
            if (period != null)
            {
               var absentCountFromPeriodInYear = SchoolLocator.AttendanceService.GetStudentCountAbsentFromPeriod(sy.StartDate, sy.EndDate
                   , gradeLevelsIds, period.Order, period.Order);

               var usuallyAbsent = absentCountFromPeriodInYear.Average(x => x.StudentCount);
               var absentNow = SchoolLocator.AttendanceService.GetStudentsAbsentFromPeriod(date, gradeLevelsIds, period.Order);
               var studentAbsentNow = students.Where(x => absentNow.Any(y => y.PersonId == x.Id)).ToList();
               var dicTotalAbsentPerSt = SchoolLocator.AttendanceService.CalcAttendanceTotalForStudents(
                        studentAbsentNow.Select(x => x.Id).ToList(), sy.Id, null, sy.StartDate, sy.EndDate, AttendanceTypeEnum.Absent);
                
                //TODO: get data for chart
                return NowAttendanceViewData.Create(studentAbsentNow, dicTotalAbsentPerSt, (int) usuallyAbsent);
            }
            return new NowAttendanceViewData();
        }
        private AttendanceByDayViewData GetAdminAttendanceByDate(GuidList gradeLevelsIds, MarkingPeriod markingPeriod, DateTime date, IList<Person> students)
        {

            throw new NotImplementedException();
        }
        private AttendanceByMpViewData GetAdminAttendanceByMp(GuidList gradeLevelsIds, Guid? fromMarkingPeriodId, Guid? toMarkingPeriodId, 
            DateTime? startDate, DateTime? endDate, IList<Person> students)
        {
            throw new NotImplementedException();
        }
    }
}