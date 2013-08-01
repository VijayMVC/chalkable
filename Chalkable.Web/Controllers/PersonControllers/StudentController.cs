using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    [RequireHttps, TraceControllerFilter]
    public class StudentController : PersonController
    {

        //[RequireRequestValue("schoolPersonId")]
        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_SUMMARY, true, CallType.Get, new[] { AppPermissionType.User, AppPermissionType.Attendance, AppPermissionType.Discipline, AppPermissionType.Grade })]
        //public ActionResult Summary(Guid schoolPersonId)
        //{
        //    if (!Context.SchoolId.HasValue)
        //        throw new UnassignedUserException();
        //    var currentDateTime = Context.NowSchoolTime;
        //    var student = SchoolLocator.PersonService.GetPerson(schoolPersonId);
        //    if (student.StudentInfo == null || student.StudentInfo.GradeLevel == null)
        //        throw new UnassignedUserException(ChlkResources.ERR_STUDENT_IS_NOT_ASSIGNED_TO_GRADE_LEVEL);
        //    if (SchoolLocator.Context.UserId != schoolPersonId && SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
        //        return Json(ShortPersonViewData.Create(student));
        //    var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(currentDateTime.Date);
        //    ClassPeriod currentClassPeriod = null;
        //    ClassAttendanceDetails currentAttendance = null;
        //    IList<AnnouncementsClassPeriodViewData> announcementPeriod = new List<AnnouncementsClassPeriodViewData>();
        //    int maxPeriodNumber = 9;
        //    var mp = markingPeriod ?? SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
        //    Guid gradeLevelRef = student.StudentInfo.GradeLevelRef;

        //    var attendances = SchoolLocator.AttendanceService.GetClassAttendanceComplex(null, mp.Id, null, student.Id, null, null, mp.StartDate, currentDateTime.Date);
        //    var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineComplexByDateRange(mp.SchoolYearRef, schoolPersonId, mp.StartDate, currentDateTime.Date);
        //    var allStudentsRankStats = SchoolLocator.GradingStatisticService.get(mp.SchoolYearRef, gradeLevelRef, null, null);
        //    var currentStudentRankStats = allStudentsRankStats.First(x => x.StudentId == student.SchoolPersonId);

        //    var lastGrades = SchoolLocator.StudentAnnouncementService.LastGrades(schoolPersonId, 5);
        //    var classes = SchoolLocator.ClassService.GetClasses(null, mp.Id, student.SchoolPersonId, 0);
        //    if (markingPeriod != null)
        //    {
        //        var periods = SchoolLocator.PeriodService.GetPeriods(markingPeriod.Id, null);
        //        maxPeriodNumber = periods.Count > 0 ? periods.Max(x => x.Order) : maxPeriodNumber;
        //        var announcements = SchoolLocator.AnnouncementService.GetAnnouncements(currentDateTime.Date, currentDateTime);
        //        var classPeriods = SchoolLocator.ClassPeriodService.GetClassPeriods(currentDateTime, null, null, student.Id, null);
        //        announcementPeriod = AnnouncementsClassPeriodViewData.Create(classPeriods, periods, announcements, classes, currentDateTime.Date);

        //        currentClassPeriod = classPeriods.FirstOrDefault(x => periods.Any(y => y.Id == x.GeneralPeriodRef && y.StartTime <= NowTimeInMinutes && y.EndTime >= NowTimeInMinutes));
        //        if (currentClassPeriod != null)
        //            currentAttendance = attendances.FirstOrDefault(x => x.ClassGeneralPeriodId == currentClassPeriod.Id && x.Date == currentDateTime.Date);
        //    }

        //    double gradeAttendanceAvg = SchoolLocator.AttendanceService.CalcGradeAttendanceAvg(gradeLevelRef);
        //    double gradeDisciplineAvg = SchoolLocator.DisciplineService.CalcGradeDisciplineAvg(gradeLevelRef);
        //    var res = StudentSummaryViewData.Create(student, currentClassPeriod, attendances, gradeAttendanceAvg, disciplines, gradeDisciplineAvg,
        //                                            lastGrades, currentStudentRankStats, allStudentsRankStats, announcementPeriod, currentAttendance,
        //                                            classes, SchoolLocator.GradingStyleService.GetMapper(), maxPeriodNumber);
        //    return Json(res, 7);
        //}
        
        
        [RequireRequestValue("personId")]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_INFO, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult Info(Guid personId)
        {
            var res = (StudentInfoViewData)GetInfo(personId, StudentInfoViewData.Create);
            var studentParents = SchoolLocator.StudentParentService.GetParents(personId);
            res.Parents = StudentParentViewData.Create(studentParents);
            return Json(res, 6);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_GET_STUDENTS, true, CallType.Get, new[] { AppPermissionType.User, })]
        public ActionResult GetStudents(string filter, bool? myStudentsOnly, int? start, int? count, Guid? classId, int? sortType)
        {
            var roleName = CoreRoles.STUDENT_ROLE.Name;
            Guid? teacherId = null;
            if (myStudentsOnly == true && CoreRoles.TEACHER_ROLE == SchoolLocator.Context.Role)
                teacherId = SchoolLocator.Context.UserId;
            var res = PersonLogic.GetPersons(SchoolLocator, start, count, sortType, filter, roleName, classId, null, teacherId);
            return Json(res);
        }
    }
}