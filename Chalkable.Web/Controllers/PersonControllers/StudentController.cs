﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.AttendancesViewData;
using Chalkable.Web.Models.DisciplinesViewData;
using Chalkable.Web.Models.GradingViewData;
using Chalkable.Web.Models.PanoramaViewDatas;
using Chalkable.Web.Models.PersonViewDatas;
using Chalkable.Web.Models.Settings;

namespace Chalkable.Web.Controllers.PersonControllers
{
    [RequireHttps, TraceControllerFilter]
    public class StudentController : PersonController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Summary(int schoolPersonId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            if (SchoolLocator.Context.PersonId != schoolPersonId &&
                SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
            {
                var student = SchoolLocator.PersonService.GetPerson(schoolPersonId);
                return Json(ShortPersonViewData.Create(student));
            }
            var studentSummaryInfo = SchoolLocator.StudentService.GetStudentSummaryInfo(schoolPersonId);
            var classes = SchoolLocator.ClassService.GetStudentClasses(Context.SchoolYearId.Value, schoolPersonId).ToList();
            var classPersons = SchoolLocator.ClassService.GetClassPersons(schoolPersonId, true);
            classes = classes.Where(x => classPersons.Any(y => y.ClassRef == x.Id)).ToList();
            var schedule = SchoolLocator.ClassPeriodService.GetSchedule(null, studentSummaryInfo.StudentInfo.Id, null,
                Context.NowSchoolYearTime.Date, Context.NowSchoolYearTime.Date);
            var classIdsSortedBySchedule = schedule.OrderBy(si => si.PeriodOrder).Select(si => si.ClassId).Distinct().ToList();

            var classList = classIdsSortedBySchedule.Where(x=>x.HasValue && classes.Any(y=>y.Id == x.Value))
                .Select(sortedClassRef => classes.First(cls => cls.Id == sortedClassRef)).ToList();
            classList.AddRange(classes.Where(cls => !classIdsSortedBySchedule.Contains(cls.Id)));

            Room currentRoom = null;
            ClassDetails currentClass = null;
            if (studentSummaryInfo.CurrentSectionId.HasValue)
            {
                currentClass = classes.FirstOrDefault(x => x.Id == studentSummaryInfo.CurrentSectionId.Value);
                if (currentClass?.RoomRef != null)
                    currentRoom = SchoolLocator.RoomService.GetRoomById(currentClass.RoomRef.Value);
            }
            
            var studentHealths = SchoolLocator.StudentService.GetStudentHealthConditions(schoolPersonId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(schoolPersonId);
            
            var res = StudentSummaryViewData.Create(studentSummaryInfo, currentRoom, currentClass, classList, customAlerts, studentHealths, BaseSecurity.IsStudent(Context));
            return Json(res);
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]
        public ActionResult Info(int personId)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);

            var syId = GetCurrentSchoolYearId();
            var today = Context.NowSchoolTime;
            var studentDetailsInfo = SchoolLocator.StudentService.GetStudentDetailsInfo(personId, syId);
            var studentSummaryInfo = SchoolLocator.StudentService.GetStudentSummaryInfo(personId);
            var studentClasses = SchoolLocator.ClassService.GetStudentClasses(syId, personId);

            var res = GetInfo(personId, personInfo => StudentInfoViewData.Create(personInfo, studentDetailsInfo,
                studentSummaryInfo, studentClasses, syId, today));
            
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(personId);
            res.HealthConditions = StudentHealthConditionViewData.Create(stHealsConditions);

            var studentContacts = SchoolLocator.ContactService.GetStudentContactDetails(personId);
            res.StudentContacts = StudentContactViewData.Create(studentContacts);

            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(personId);
            res.StudentCustomAlertDetails = StudentCustomAlertDetailViewData.Create(customAlerts);

            var homeroom = SchoolLocator.RoomService.GetStudentHomeroomOrNull(personId, syId);
            if(homeroom != null)
                res.Homeroom = HomeroomViewData.Create(homeroom);

            return Json(res);
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Schedule(int personId)
        {
            var student = SchoolLocator.StudentService.GetById(personId, GetCurrentSchoolYearId());
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(personId);
            var studentHealths = SchoolLocator.StudentService.GetStudentHealthConditions(personId);
            var res = PrepareScheduleData(StudentProfileViewData.Create(student, customAlerts, studentHealths));
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]
        public ActionResult GetStudents(string filter, bool? myStudentsOnly, int? start, int? count, int? classId, bool? byLastName, int? markingPeriodId)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
       
            int? classMatesToId = null;
            int? teacherId = null;
            if (CoreRoles.STUDENT_ROLE == SchoolLocator.Context.Role)
                classMatesToId = Context.PersonId;
            else
            {
                if (myStudentsOnly == true)
                {
                    if (Context.Claims.HasPermission(ClaimInfo.VIEW_STUDENT) || Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_STUDENTS))
                    {
                        if (CoreRoles.TEACHER_ROLE == SchoolLocator.Context.Role)
                            teacherId = SchoolLocator.Context.PersonId;
                    }
                    else
                        throw new ChalkableException($"User has no required ({ClaimInfo.VIEW_STUDENT} or {ClaimInfo.VIEW_CLASSROOM_STUDENTS}) permission for watch \"My Students\"");
                }
                else
                    if (!Context.Claims.HasPermission(ClaimInfo.VIEW_STUDENT))
                        throw new ChalkableException($"User has no required ({ClaimInfo.VIEW_STUDENT}) permission for watch \"Whole Students\"");
            }
            var res = SchoolLocator.StudentService.SearchStudents(Context.SchoolYearId.Value, classId, teacherId, classMatesToId, filter, byLastName != true, start ?? 0, count ?? 10, markingPeriodId);
            return Json(res.Transform(StudentViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Explorer(int personId)
        {
            var syId = GetCurrentSchoolYearId();
            var studentExplorerInfo = SchoolLocator.StudentService.GetStudentExplorerInfo(personId, syId);
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(personId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(personId);

            MasterLocator.UserTrackingService.UsedStandardsExplorer(Context.Login, "student explorer");
            var res = StudentExplorerViewData.Create(studentExplorerInfo, stHealsConditions, customAlerts);
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult AttendanceSummary(int studentId, int? gradingPeriodId)
        {
            var syid = GetCurrentSchoolYearId();
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syid);
            var gp = gradingPeriodId.HasValue
                         ? SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId.Value)
                         : SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syid, Context.NowSchoolYearTime.Date);
            var studentSummary = SchoolLocator.AttendanceService.GetStudentAttendanceSummary(studentId, gradingPeriodId);

            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);
            var res = StudentAttendanceSummaryViewData.Create(studentSummary, gp, gradingPeriods, customAlerts, stHealsConditions);
            
            return Json(res);
        }
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Apps(int studentId, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 12;

            var syId = GetCurrentSchoolYearId();
            var student = SchoolLocator.StudentService.GetById(studentId, syId);
            var apps = MasterLocator.ApplicationService.GetApplications(start.Value, count.Value, true)
                .Where(x => MasterLocator.ApplicationService.HasMyApps(x))
                .Select(BaseApplicationViewData.Create).ToList();
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);
            return Json(StudentAppsViewData.Create(student, apps, customAlerts, stHealsConditions));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult DisciplineSummary(int studentId, int? gradingPeriodId)
        {
            var syId = GetCurrentSchoolYearId();
            var student = SchoolLocator.StudentService.GetById(studentId, syId);
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syId);
            var gp = gradingPeriodId.HasValue
                         ? SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId.Value)
                         : SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syId, Context.NowSchoolYearTime.Date);
            var infractionSummaries = SchoolLocator.DisciplineService.GetStudentInfractionSummary(studentId, gradingPeriodId);
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);
            var res = StudentDisciplineSummaryViewData.Create(student, infractionSummaries, gp, gradingPeriods, customAlerts, stHealsConditions);
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult GradingSummary(int studentId)
        {
            var syId = GetCurrentSchoolYearId();
            var student = SchoolLocator.StudentService.GetById(studentId, syId);
            var gradingSummary = SchoolLocator.GradingStatisticService.GetStudentGradingSummary(syId, studentId);

            var enrolledClassIds =
                SchoolLocator.ClassService.GetClassPersons(studentId, null, true, null).Select(x => x.ClassRef);

            var classes = 
                gradingSummary.StudentAverages.Select(x => SchoolLocator.ClassService.GetById(x.ClassId)).ToList();

            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syId);
            var gp = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syId, Context.NowSchoolYearTime.Date);
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);
            var res = StudentProfileGradingSummaryViewData.Create(student, gradingSummary, gp, gradingPeriods, classes, enrolledClassIds, customAlerts, stHealsConditions);
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult GradingDetails(int studentId, int gradingPeriodId)
        {
            var syId = GetCurrentSchoolYearId();
            var student = SchoolLocator.StudentService.GetById(studentId, syId);
            var gp = SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var gradingDetails = SchoolLocator.GradingStatisticService.GetStudentGradingDetails(syId, studentId, gp.Id);
            
            var activityIds = gradingDetails.StudentAnnouncements.Select(x => x.ActivityId).Distinct().ToList();
            var announcements = SchoolLocator.ClassAnnouncementService.GetByActivitiesIds(activityIds);

            var classIds = announcements.Select(x => x.ClassAnnouncementData.ClassRef).Distinct().ToList();

            var classAnnouncementTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classIds);
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);

            var res = StudentProfileGradingDetailViewData.Create(student, gradingDetails, gp, announcements, classAnnouncementTypes, customAlerts, stHealsConditions);
            return Json(res);
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult Panorama(int studentId, StudentProfilePanoramaSetting settings)
        {
            if (!Context.Claims.HasPermission(ClaimInfo.VIEW_PANORAMA))
                throw new ChalkableSecurityException("You are not allowed to view class panorama");

            if (settings.SchoolYearIds == null)
                settings = SchoolLocator.PanoramaSettingsService.Get<StudentProfilePanoramaSetting>(null);

            if (settings.SchoolYearIds.Count == 0)
                throw new ChalkableException("School years is required parameter");

            var studentPanorama = SchoolLocator.StudentService.Panorama(studentId, settings.SchoolYearIds, settings.StandardizedTestFilters);
            var standardizedTests = SchoolLocator.StandardizedTestService.GetListOfStandardizedTestDetails();

            return Json(StudentPanoramaViewData.Create(studentId, studentPanorama, settings, standardizedTests));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult SavePanoramaSettings(StudentProfilePanoramaSetting setting)
        {
            if (!Context.Claims.HasPermission(ClaimInfo.VIEW_PANORAMA))
                throw new ChalkableSecurityException("You are not allowed to change panorama settings");

            if (setting.SchoolYearIds == null || setting.SchoolYearIds.Count == 0)
                throw new ChalkableException("School years is required parameter");

            SchoolLocator.PanoramaSettingsService.Save(setting, null);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult RestorePanoramaSettings()
        {
            if (!Context.Claims.HasPermission(ClaimInfo.VIEW_PANORAMA))
                throw new ChalkableSecurityException("You are not allowed to change panorama settings");

            var settings = SchoolLocator.PanoramaSettingsService.Restore<StudentProfilePanoramaSetting>(null);
            return Json(PersonProfilePanoramaSettingViewData.Create(settings));
        }
    }
}