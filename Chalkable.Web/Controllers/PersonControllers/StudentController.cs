using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
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

        //TODO: stduent grade rank ... get last grades 
        [RequireRequestValue("schoolPersonId")]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_SUMMARY, true, CallType.Get, new[] { AppPermissionType.User, AppPermissionType.Attendance, AppPermissionType.Discipline, AppPermissionType.Grade })]
        public ActionResult Summary(int schoolPersonId)
        {
            throw new NotImplementedException();
            //if (!Context.SchoolId.HasValue)
            //    throw new UnassignedUserException();
            //var currentDateTime = Context.NowSchoolTime;
            //var student = SchoolLocator.PersonService.GetPerson(schoolPersonId);
            //if (student.StudentInfo == null || student.StudentInfo.GradeLevel == null)
            //    throw new UnassignedUserException(ChlkResources.ERR_STUDENT_IS_NOT_ASSIGNED_TO_GRADE_LEVEL);
            //if (SchoolLocator.Context.UserLocalId != schoolPersonId && SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
            //    return Json(ShortPersonViewData.Create(student));
            //var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(currentDateTime.Date);
            //AttendanceTypeEnum? currentAttendanceType = null;
            //IList<AnnouncementsClassPeriodViewData> announcementPeriod = new List<AnnouncementsClassPeriodViewData>();
            //int maxPeriodNumber = 9;
            //var mp = markingPeriod ?? SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            //var attendanceTotal = SchoolLocator.AttendanceService.CalcAttendanceTotalPerTypeForStudent(student.Id, null, mp.Id, null, currentDateTime.Date);
            //var disciplineTotal = SchoolLocator.DisciplineService.CalcDisciplineTypeTotalForStudent(student.Id, null, mp.Id, null, currentDateTime.Date);
            
            ////var allStudentsRankStats = SchoolLocator.GradingStatisticService.get(mp.SchoolYearRef, gradeLevelRef, null, null);
            ////var currentStudentRankStats = allStudentsRankStats.First(x => x.StudentId == student.SchoolPersonId);

            //var lastGrades =  SchoolLocator.StudentAnnouncementService.GetLastGrades(student.Id, null, 5);
            //var mapper = SchoolLocator.GradingStyleService.GetMapper();
            //var classes = SchoolLocator.ClassService.GetClasses(null, mp.Id, student.Id);

            //var studentsRanks = SchoolLocator.GradingStatisticService.GetStudentGradingRanks(mp.SchoolYearRef, null, student.StudentInfo.GradeLevelRef, null);
            //Room currentRoom = null;
            //ClassDetails currentClass = null;
            //if (markingPeriod != null)
            //{
            //    var rooms = SchoolLocator.RoomService.GetRooms();
            //    var periods = SchoolLocator.PeriodService.GetPeriods(markingPeriod.Id, null);
            //    maxPeriodNumber = periods.Count > 0 ? periods.Max(x => x.Order) : maxPeriodNumber;
            //    var announcements = SchoolLocator.AnnouncementService.GetAnnouncements(currentDateTime.Date, currentDateTime);
            //    var classPeriods = SchoolLocator.ClassPeriodService.GetClassPeriods(currentDateTime, null, null, student.Id, null);
            //    announcementPeriod = AnnouncementsClassPeriodViewData.Create(announcements, classPeriods, classes, rooms, currentDateTime.Date);

            //    var currentClassPeriod = classPeriods.FirstOrDefault(x => x.Period.StartTime <= NowTimeInMinutes && x.Period.EndTime >= NowTimeInMinutes);
            //    if (currentClassPeriod != null)
            //    {
            //        var currentAttendance = SchoolLocator.AttendanceService.GetClassAttendanceDetails(student.Id, 
            //            currentClassPeriod.Id, currentDateTime.Date);
            //        if (currentAttendance != null)
            //            currentAttendanceType = currentAttendance.Type;
            //        currentClass = classes.First(x => x.Id == currentClassPeriod.ClassRef);
            //        currentRoom = rooms.First(x => x.Id == currentClassPeriod.RoomRef);
            //    }
            //}
           
            //var res = StudentSummaryViewData.Create(student, currentRoom, currentClass, classes, disciplineTotal,
            //                attendanceTotal, currentAttendanceType, announcementPeriod, maxPeriodNumber, lastGrades, mapper, studentsRanks);
            //return Json(res, 7);
        }
        
        
        [RequireRequestValue("personId")]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_INFO, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult Info(int personId)
        {
            var res = (StudentInfoViewData)GetInfo(personId, StudentInfoViewData.Create);
            var studentParents = SchoolLocator.StudentParentService.GetParents(personId);
            res.Parents = StudentParentViewData.Create(studentParents);
            return Json(res, 6);
        }

        //ToDo This is only info copy 
        [RequireRequestValue("personId")]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_INFO, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult Schedule(int personId)
        {
            var res = (StudentInfoViewData)GetInfo(personId, StudentInfoViewData.Create);
            return Json(res, 6);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_GET_STUDENTS, true, CallType.Get, new[] { AppPermissionType.User, })]
        public ActionResult GetStudents(string filter, bool? myStudentsOnly, int? start, int? count, int? classId, bool? byLastName)
        {
            var roleName = CoreRoles.STUDENT_ROLE.Name;
            int? teacherId = null;
            if (myStudentsOnly == true && CoreRoles.TEACHER_ROLE == SchoolLocator.Context.Role)
                teacherId = SchoolLocator.Context.UserLocalId;
            var res = PersonLogic.GetPersons(SchoolLocator, start, count, byLastName, filter, roleName, classId, null, teacherId);
            return Json(res);
        }

       
        [RequireRequestValue("personId")]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_GRADING_STAT, true, CallType.Get, new[] {AppPermissionType.User, AppPermissionType.Grade})]
        public ActionResult Grading(int personId, int markingPeriodId)
        {
            throw new NotImplementedException();
            //var student = SchoolLocator.PersonService.GetPerson(personId);
            //if (!BaseSecurity.IsAdminTeacherOrExactStudent(MasterLocator.UserService.GetByLogin(student.Email), Context))
            //    throw new ChalkableSecurityException(ChlkResources.ERR_VIEW_INFO_INVALID_RIGHTS);
            //var gardingStats = SchoolLocator.GradingStatisticService.GetFullGradingStats(markingPeriodId, student.Id);
            //var gradingMapper = SchoolLocator.GradingStyleService.GetMapper();
            //var res = StudentGradingViewData.Create(student, gardingStats, gradingMapper);
            //return Json(res);
        }
    }
}