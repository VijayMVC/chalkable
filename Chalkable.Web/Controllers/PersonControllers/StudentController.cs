﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    [RequireHttps, TraceControllerFilter]
    public class StudentController : PersonController
    {

        //TODO: stduent grade rank ... get last grades 
        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_SUMMARY, true, CallType.Get, new[] { AppPermissionType.User, AppPermissionType.Attendance, AppPermissionType.Discipline, AppPermissionType.Grade })]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Summary(int schoolPersonId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            if (SchoolLocator.Context.PersonId != schoolPersonId &&
                SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
            {
                var student = SchoolLocator.PersonService.GetPerson(schoolPersonId);
                return Json(ShortPersonViewData.Create(student));
            }
            var studentSummaryInfo = SchoolLocator.PersonService.GetStudentSummaryInfo(schoolPersonId);
            var classes = SchoolLocator.ClassService.GetClasses(Context.SchoolYearId, null, schoolPersonId).ToList();
            var classPersons = SchoolLocator.ClassService.GetClassPersons(schoolPersonId, true);
            classes = classes.Where(x => classPersons.Any(y => y.ClassRef == x.Id)).ToList();
            var classPeriods = SchoolLocator.ClassPeriodService.GetClassPeriods(Context.NowSchoolYearTime, null, null, studentSummaryInfo.StudentInfo.Id, null);

            var sortedClassRefs = classPeriods.OrderBy(cp => cp.Period.StartTime).Select(cp => cp.ClassRef).Distinct().ToList();
            var classList = sortedClassRefs.Select(sortedClassRef => classes.FirstOrDefault(cls => cls.Id == sortedClassRef)).Where(c => c != null).ToList();
            classList.AddRange(classes.Where(cls => !sortedClassRefs.Contains(cls.Id)));

            var currentClassPeriod = classPeriods.FirstOrDefault(x => x.Period.StartTime <= NowTimeInMinutes && x.Period.EndTime >= NowTimeInMinutes);
            Room currentRoom = null;
            ClassDetails currentClass = null;
            if (currentClassPeriod != null)
            {
                currentClass = classes.FirstOrDefault(x => x.Id == currentClassPeriod.ClassRef);
                if (currentClass != null && currentClass.RoomRef.HasValue)
                    currentRoom = SchoolLocator.RoomService.GetRoomById(currentClass.RoomRef.Value);

            }
            var stHealsConditions = SchoolLocator.PersonService.GetStudentHealthConditions(schoolPersonId);
            var res = StudentSummaryViewData.Create(studentSummaryInfo, currentRoom, currentClass, classList);
            res.HealthConditions = StudentHealthConditionViewData.Create(stHealsConditions);
            return Json(res);

        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_INFO, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult Info(int personId)
        {
            var res = (StudentInfoViewData)GetInfo(personId, StudentInfoViewData.Create);
            var stHealsConditions = SchoolLocator.PersonService.GetStudentHealthConditions(personId);
            res.HealthConditions = StudentHealthConditionViewData.Create(stHealsConditions);
            //parents functionality are not implemanted yet
            //var studentParents = SchoolLocator.StudentParentService.GetParents(personId);
            //res.Parents = StudentParentViewData.Create(studentParents);
            return Json(res, 6);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_GET_STUDENTS, true, CallType.Get, new[] { AppPermissionType.User, })]
        public ActionResult GetStudents(string filter, bool? myStudentsOnly, int? start, int? count, int? classId, bool? byLastName)
        {
            //TODO: I think we need use particular methods like get teacher students and get class students
            var roleName = CoreRoles.STUDENT_ROLE.Name;
            int? teacherId = null;
            if (myStudentsOnly == true && CoreRoles.TEACHER_ROLE == SchoolLocator.Context.Role)
                teacherId = SchoolLocator.Context.PersonId;
            var res = PersonLogic.GetPersons(SchoolLocator, start, count, byLastName, filter, roleName, classId, null, teacherId);
            return Json(res);
        }

        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_GRADING_STAT, true, CallType.Get, new[] {AppPermissionType.User, AppPermissionType.Grade})]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
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