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
using Chalkable.Data.School.DataAccess;
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

        //TODO : think how to rewrite for better performence 
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult SetAttendanceForList(int personId, IntList classIds, IList<string> levels, StringList attReasons, DateTime date)
        {
            /*for (int i = 0; i < classPersonIds.Count; ++i)
            {
                var classPersonId = classPersonIds[i];
                var attendanceType = attendanceTypes[i];
                Guid reason;
                bool hasReason = Guid.TryParse(attReasons[i], out reason);
                var periodId = classPeriodIds[i];
                SchoolLocator.AttendanceService.SetClassAttendance(classPersonId, periodId, date
                    , (AttendanceTypeEnum)attendanceType, hasReason  && reason != Guid.Empty ? reason : (Guid?)null);
            }*/
            return Json(true);
        }

        //TODO: implement this method later
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult NotTakenAttendanceClasses(DateTime? date)
        {
            //date = (date ?? SchoolLocator.Context.NowSchoolYearTime);
            //var notTakenAttendanceClasses = SchoolLocator.AttendanceService.GetNotTakenAttendanceClasses(date.Value);
            //return Json(ClassViewData.Create(notTakenAttendanceClasses));
            return Json(new List<ClassViewData>());
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher", Preference.API_DESCR_ATTENDANCE_SET_ATTENDANCE_FOR_CLASS, true, CallType.Post, new[] { AppPermissionType.Attendance })]
        public ActionResult SetAttendanceForClass(string level, int? attendanceReasonId, int classId, DateTime date)
        {
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(date, true);
            if (mp == null)
            {
                throw new ChalkableException("No marking period scheduled on this date");
            }

            IList<Person> persons = SchoolLocator.PersonService.GetClassStudents(classId, mp.Id);
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


        public ActionResult AttendanceTest()
        {
            if (HttpContext.Cache["CONTEXT"] == null)
            {
                var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
                var c = serviceLocator.UserService.Login("user1195_6cf8e8ab-2cae-4d17-8b7c-59bc3b1134fe@chalkable.com",
                                                         "Qwerty1@");
                HttpContext.Cache["CONTEXT"] = c;
            }
            var context = HttpContext.Cache["CONTEXT"] as UserContext;
            InitServiceLocators(context);
            return ClassList(new DateTime(2013, 11, 18), 635);
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher, Checkin")]
        public ActionResult SetDailyAttendance(Guid personId, DateTime? date, int? timeIn, int? timeOut)
        {

            throw new NotImplementedException();
            //var dateTime = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            //var cDate = SchoolLocator.CalendarDateService.GetCalendarDateByDate(dateTime);
            //if (!(cDate.IsSchoolDay && cDate.ScheduleSectionRef.HasValue))
            //    throw new ChalkableException("Today is not school day");
            //var studentDailyAttendance = SchoolLocator.AttendanceService.SetDailyAttendance(dateTime, personId, timeIn ?? NowTimeInMinutes, timeOut);
            //var person = SchoolLocator.PersonService.GetPerson(personId);
            //return Json(DailyAttendanceViewData.Create(studentDailyAttendance, person));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher, Checkin")]
        public ActionResult SwipeCard(Guid personId, Guid classPeriodId)
        {
            var now = SchoolLocator.Context.NowSchoolYearTime;
            var cDate = SchoolLocator.CalendarDateService.GetCalendarDateByDate(now.Date);
            if (!(cDate.IsSchoolDay && cDate.DayTypeRef.HasValue))
                throw new ChalkableException("Today is not school day");

            var att = SchoolLocator.AttendanceService.SwipeCard(personId, now, classPeriodId);
            var dailyAttendace = SchoolLocator.AttendanceService.GetDailyAttendance(now.Date, personId);
            return Json(DailyAttendanceViewData.Create(dailyAttendace, att.Student));
        }


        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ATTENDANCE_STUDENT_ATTENDANCE_SUMMARY, true, CallType.Get, new[] { AppPermissionType.User, AppPermissionType.Attendance })]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult StudentAttendanceSummary(Guid personId, Guid markingPeriodId)
        {
            throw new NotImplementedException();
            //if (!Context.SchoolId.HasValue)
            //    throw new UnassignedUserException();
            //var currentDateTime = Context.NowSchoolTime;
            //var student = SchoolLocator.PersonService.GetPersonDetails(personId);
            //var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            //var attendances = SchoolLocator.AttendanceService.GetClassAttendanceDetails(new ClassAttendanceQuery
            //    {
            //        MarkingPeriodId = mp.Id,
            //        StudentId = student.Id,
            //        FromDate = mp.StartDate,
            //        ToDate = currentDateTime
            //    });
            //var res = StudentAttendanceDetailedViewData.Create(student, attendances, mp);
            //return Json(res, 6);
        }



        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetAttendanceForStudent(DateTime? datetime, int studentId)
        {
            return FakeJson("~/fakeData/getAttendanceForStudent.json");
            //if (!SchoolLocator.Context.SchoolId.HasValue)
            //    throw new UnassignedUserException();
            //var date = (datetime ?? SchoolLocator.Context.NowSchoolTime).Date;
            //var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(date);
            //var query = new ClassAttendanceQuery
            //    {
            //        MarkingPeriodId = markingPeriod.Id,
            //        FromDate = date,
            //        ToDate = date,
            //        StudentId = studentId,
            //        NeedAllData = true
            //    };
            //var attendances = SchoolLocator.AttendanceService.GetClassAttendanceDetails(query);
            //var dailAttendance = SchoolLocator.AttendanceService.GetDailyAttendance(date, studentId);
            //var student = attendances.Count > 0 ? attendances.First().Student : SchoolLocator.PersonService.GetPerson(studentId);
            //return Json(StudentAttendanceViewData.Create(student, attendances, dailAttendance));
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
            if(markingPeriod == null)
                throw new NoMarkingPeriodException();
            var seatingChart = SchoolLocator.AttendanceService.GetSeatingChart(classId, markingPeriod.Id);
            if (seatingChart != null)
            {
                var attendances = ClassAttendanceList(d, classId);
                var students = SchoolLocator.PersonService.GetClassStudents(classId, markingPeriod.Id, true);
                return AttendanceSeatingChartViewData.Create(seatingChart, attendances, students);               
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
            //var date = (nowDateTime ?? Context.NowSchoolTime).Date;
            //var res = new AdminAttendanceSummaryViewData();
            
            //var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(date.Date);
            //renderNow = renderNow && markingPeriod != null;
            //IList<Person> allStudents = SchoolLocator.PersonService.GetPaginatedPersons(new PersonQuery {RoleId = CoreRoles.STUDENT_ROLE.Id});

            //if (renderNow)
            //    res.NowAttendanceData = GetNowAttendanceData(gradeLevelsIds, allStudents);
            //if (renderDay)
            //    res.AttendanceByDayData = GetAdminAttendanceByDate(gradeLevelsIds, markingPeriod, date, allStudents);
            //if (renderMp)
            //    res.AttendanceByMpData = GetAdminAttendanceByMp(gradeLevelsIds, fromMarkingPeriodId, toMarkingPeriodId, startDate, endDate, allStudents);
            //return Json(res, 8);
        }
        
        private NowAttendanceViewData GetNowAttendanceData(GuidList gradeLevelsIds, IList<Person> students)
        {
            throw new NotImplementedException();
            //var date = Context.NowSchoolTime.Date;
            //var period = SchoolLocator.PeriodService.GetPeriod(NowTimeInMinutes, date);
            //var sy = SchoolLocator.SchoolYearService.GetCurrentSchoolYear();
            //if (period != null)
            //{
            //   var absentCountFromPeriodInYear = SchoolLocator.AttendanceService.GetStudentCountAbsentFromPeriod(sy.StartDate, sy.EndDate
            //       , gradeLevelsIds, period.Order, period.Order);

            //   var usuallyAbsent = absentCountFromPeriodInYear.Average(x => x.StudentCount);
            //   var absentNow = SchoolLocator.AttendanceService.GetStudentsAbsentFromPeriod(date, gradeLevelsIds, period.Order);
            //   var studentAbsentNow = students.Where(x => absentNow.Any(y => y.PersonId == x.Id)).ToList();
            //   var dicTotalAbsentPerSt = SchoolLocator.AttendanceService.CalcAttendanceTotalForStudents(
            //            studentAbsentNow.Select(x => x.Id).ToList(), sy.Id, null, sy.StartDate, sy.EndDate, AttendanceTypeEnum.Absent);
                
            //   var type = AttendanceTypeEnum.Absent | AttendanceTypeEnum.Excused | AttendanceTypeEnum.Late;
            //   var attStats = SchoolLocator.AttendanceService.CalcAttendanceTotalPerPeriod(date, date, 1, period.Order, type, gradeLevelsIds);
            //   var attStatsView = AttendanceStatsViewData.BuildStatsPerPeriod(attStats, period.Order);
            //   return NowAttendanceViewData.Create(studentAbsentNow, dicTotalAbsentPerSt, (int)usuallyAbsent, attStatsView);
            //}
            //return new NowAttendanceViewData();
        }

        private AttendanceByDayViewData GetAdminAttendanceByDate(GuidList gradeLevelsIds, MarkingPeriod markingPeriod, DateTime date, IList<Person> students)
        {
            throw new NotImplementedException();
            //if (markingPeriod != null)
            //{
            //    var stsIdsAbsentFromDay = SchoolLocator.AttendanceService.GetStudentsAbsentFromDay(date, gradeLevelsIds);
            //    var stsAttTotalPerType = SchoolLocator.AttendanceService.CalcAttendanceTotalPerTypeForStudents(students.Select(x=>x.Id).ToList()
            //        , null, markingPeriod.Id, date, date);

            //    var type = AttendanceTypeEnum.Present | AttendanceTypeEnum.NotAssigned;
            //    stsAttTotalPerType = stsAttTotalPerType.Where(x => x.Total > 0 && (x.AttendanceType & (type)) == 0).ToList();
            //    var days = SchoolLocator.CalendarDateService.GetLastDays(markingPeriod.SchoolYearRef, true, null, date, 7);

            //    type = AttendanceTypeEnum.Absent | AttendanceTypeEnum.Excused | AttendanceTypeEnum.Late;
            //    var attsStatsPerDate = SchoolLocator.AttendanceService.CalcAttendanceTotalPerDate(days.First().DateTime, days.Last().DateTime, type, gradeLevelsIds);
            //    var attsStatsView = AttendanceStatsViewData.BuildStatsPerDate(attsStatsPerDate, days, "ddd");
            //    return AttendanceByDayViewData.Create(students, stsAttTotalPerType, stsIdsAbsentFromDay, attsStatsView);
            //}
            //return new AttendanceByDayViewData();
        }
        private AttendanceByMpViewData GetAdminAttendanceByMp(GuidList gradeLevelsIds, Guid? fromMarkingPeriodId, Guid? toMarkingPeriodId, 
            DateTime? startDate, DateTime? endDate, IList<Person> students)
        {
            throw new NotImplementedException();
            //MarkingPeriod fromMp, toMp;
            //DateTime fromDate, toDate;
            //var currentDate = Context.NowSchoolTime.Date;
            //if (startDate.HasValue && endDate.HasValue)
            //{
            //    if (startDate.Value > endDate.Value)
            //        throw new ChalkableException(ChlkResources.ERR_INCCORRECT_REQUEST_DATE_PARAMS_START_DATE);

            //    fromMp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod(startDate);
            //    toMp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod(endDate);
            //    fromDate = startDate.Value;
            //    toDate = endDate.Value;
            //    if (startDate.Value > toMp.EndDate || endDate.Value < fromMp.StartDate)
            //        throw new NoMarkingPeriodException();
            //}
            //else if (fromMarkingPeriodId.HasValue && toMarkingPeriodId.HasValue)
            //{
            //    fromMp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodById(fromMarkingPeriodId.Value);
            //    toMp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodById(toMarkingPeriodId.Value);
            //    fromDate = fromMp.StartDate;
            //    toDate = toMp.EndDate;
            //}
            //else throw new ChalkableException(ChlkResources.ERR_INCCORRECT_REQUEST_DATE_PARAMS);

            //var currentSchoolYear = SchoolLocator.SchoolYearService.GetCurrentSchoolYear();
            //if (currentSchoolYear.Id != fromMp.SchoolYearRef || currentSchoolYear.Id != toMp.SchoolYearRef)
            //    throw new ChalkableException(ChlkResources.ERR_MARKING_PERIODS_DONT_BELONG_TO_CURRENT_SCHOOL_YEAR);

            //IList<DateTime> dates = null;
            //if (toDate.Date > currentDate.Date)
            //    toDate = currentDate;
            //if (fromDate.Date <= toDate.Date)
            //    dates = SchoolLocator.CalendarDateService.GetLastDays(currentSchoolYear.Id, true, fromDate.Date, toDate.Date).Select(x => x.DateTime).ToList();
            
            //if (dates != null && dates.Count > 0)
            //{
            //    var type = AttendanceTypeEnum.Absent | AttendanceTypeEnum.Late;
            //    var dicStsAttTotal = SchoolLocator.AttendanceService.CalcAttendanceTotalForStudents(students.Select(x => x.Id).ToList(),
            //                          currentSchoolYear.Id, null, fromDate, toDate, type);
            //    IList<Guid> absentLateStsIds = dicStsAttTotal.Where(x => x.Value > 0).OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
            //    var stsAbsentCountAvg = SchoolLocator.AttendanceService.GetStudentCountAbsentFromDay(fromDate, toDate, gradeLevelsIds)
            //                                                           .Average(x => x.Value);
                
            //    int maxPeriodOrder = 10; //TODO : get max periodOrder from db
            //    type = type | AttendanceTypeEnum.Excused;
            //    var attStatsPerPeriod = SchoolLocator.AttendanceService.CalcAttendanceTotalPerPeriod(fromDate, toDate, 1, maxPeriodOrder, type, gradeLevelsIds);
            //    var attStatsView = AttendanceStatsViewData.BuildStatsPerPeriod(attStatsPerPeriod, maxPeriodOrder);
            //    return AttendanceByMpViewData.Create(students, absentLateStsIds, (int)stsAbsentCountAvg, attStatsView);
            //}
            //return new AttendanceByMpViewData();
        }
    }
}