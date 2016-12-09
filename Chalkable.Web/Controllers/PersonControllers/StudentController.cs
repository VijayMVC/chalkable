using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
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

        private const string CONTENT_DISPOSITION = "Content-Disposition";
        private const string HEADER_FORMAT = "inline; filename={0}";

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> Summary(int schoolPersonId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            if (SchoolLocator.Context.PersonId != schoolPersonId &&
                SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
            {
                var student = SchoolLocator.PersonService.GetPerson(schoolPersonId);
                return Json(ShortPersonViewData.Create(student));
            }

            var studentHealths = SchoolLocator.StudentService.GetStudentHealthConditions(schoolPersonId);
            
            var currentSchoolYear = SchoolLocator.SchoolYearService.GetCurrentSchoolYearByStudent(schoolPersonId);

            var healthFormsTask = SchoolLocator.StudentService.GetStudentHealthForms(schoolPersonId, currentSchoolYear.Id);
            var studentSummaryInfoTask = SchoolLocator.StudentService.GetStudentSummaryInfo(schoolPersonId, currentSchoolYear.Id);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(schoolPersonId);

            var classes = SchoolLocator.ClassService.GetStudentClasses(currentSchoolYear.Id, schoolPersonId).ToList();
            var classPersons = SchoolLocator.ClassService.GetClassPersons(schoolPersonId, true);
            classes = classes.Where(x => classPersons.Any(y => y.ClassRef == x.Id)).ToList();
            var schedule = SchoolLocator.ClassPeriodService.GetSchedule(null, schoolPersonId, null,
                Context.NowSchoolYearTime.Date, Context.NowSchoolYearTime.Date);
            var classIdsSortedBySchedule = schedule.OrderBy(si => si.PeriodOrder).Select(si => si.ClassId).Distinct().ToList();

            var classList = classIdsSortedBySchedule.Where(x=>x.HasValue && classes.Any(y=>y.Id == x.Value))
                .Select(sortedClassRef => classes.First(cls => cls.Id == sortedClassRef)).ToList();
            classList.AddRange(classes.Where(cls => !classIdsSortedBySchedule.Contains(cls.Id)));

            Room currentRoom = null;
            ClassDetails currentClass = null;
            var studentSummaryInfo = await studentSummaryInfoTask;
            if (studentSummaryInfo.CurrentSectionId.HasValue)
            {
                currentClass = classes.FirstOrDefault(x => x.Id == studentSummaryInfo.CurrentSectionId.Value);
                if (currentClass?.RoomRef != null)
                    currentRoom = SchoolLocator.RoomService.GetRoomById(currentClass.RoomRef.Value);
            }
            var res = StudentSummaryViewData.Create(studentSummaryInfo, currentRoom, currentClass, classList, customAlerts, await studentHealths, await healthFormsTask, BaseSecurity.IsStudent(Context));
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> StudentAlertDetails(int studentId, int schoolYearId)
        {
            var healths = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);
            var student = SchoolLocator.StudentService.GetById(studentId, schoolYearId);
            return Json(StudentAlertDetailsViewData.Create(student, await healths, customAlerts));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]
        public async Task<ActionResult> Info(int personId)
        {
            var currentSchoolYear = SchoolLocator.SchoolYearService.GetCurrentSchoolYearByStudent(personId);
            var syId = currentSchoolYear.Id;
            var today = Context.NowSchoolTime;


            var studentSummaryInfoTask = SchoolLocator.StudentService.GetStudentSummaryInfo(personId, syId);
            var stHealsConditionsTask = SchoolLocator.StudentService.GetStudentHealthConditions(personId);
            var healthFormsTask = SchoolLocator.StudentService.GetStudentHealthForms(personId, syId);
            var verifyHealthFormsAccess = SchoolLocator.StudentService.CanVerifyHealthForm(personId, syId);

            var studentDetailsInfo = SchoolLocator.StudentService.GetStudentDetailsInfo(personId, syId);
            
            var studentClasses = SchoolLocator.ClassService.GetStudentClasses(syId, personId);
            Room currentRoom = null;
            ClassDetails currentClass = null;
            var studentSummaryInfo = await studentSummaryInfoTask;
            if (studentSummaryInfo.CurrentSectionId.HasValue)
            {
                currentClass = studentClasses.FirstOrDefault(x => x.Id == studentSummaryInfo.CurrentSectionId.Value);
                if (currentClass?.RoomRef != null)
                    currentRoom = SchoolLocator.RoomService.GetRoomById(currentClass.RoomRef.Value);
            }
            var res = GetInfo(personId, currentSchoolYear.SchoolRef, personInfo => StudentInfoViewData.Create(personInfo, studentDetailsInfo,
                studentSummaryInfo, studentClasses, currentClass, currentRoom, syId));

            var studentContacts = SchoolLocator.ContactService.GetStudentContactDetails(personId);
            res.StudentContacts = StudentContactViewData.Create(studentContacts);

            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(personId);
            res.StudentCustomAlertDetails = StudentCustomAlertDetailViewData.Create(customAlerts);
            
            res.HealthConditions = StudentHealthConditionViewData.Create(await stHealsConditionsTask);
            res.HealthForms = StudentHealthFormViewData.Create(await healthFormsTask);
            res.IsAbleVerifyHealthForms = await verifyHealthFormsAccess;
            res.HasNotVerifiedHealthForm = res.HealthForms.Count > 0 &&
                                           res.HealthForms.Any(x => !x.VerifiedDate.HasValue);

            var homeroom = SchoolLocator.RoomService.GetStudentHomeroomOrNull(personId, syId);
            if(homeroom != null)
                res.Homeroom = HomeroomViewData.Create(homeroom);

            return Json(res);
        }
        

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public async Task<ActionResult> VerifyStudentHealthForm(int studentId, int healthFormId)
        {
            var currentSchoolYear = SchoolLocator.SchoolYearService.GetCurrentSchoolYearByStudent(studentId);
            await SchoolLocator.StudentService.VerifyStudentHealthForm(studentId, currentSchoolYear.Id, healthFormId);
            var res = await SchoolLocator.StudentService.GetStudentHealthForms(studentId, currentSchoolYear.Id);
            return Json(StudentHealthFormViewData.Create(res));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult DownloadHealthFormDocument(int studentId, int healthFormId)
        {
            var file = SchoolLocator.StudentService.DownloadStudentHealthFormDocument(studentId, healthFormId);
            Response.AddHeader(CONTENT_DISPOSITION, string.Format(HEADER_FORMAT, "StudentHealthForm.pdf"));
            return File(file, MimeHelper.GetContentTypeByExtension("pdf"));
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> Schedule(int personId)
        {
            var studentHealths = SchoolLocator.StudentService.GetStudentHealthConditions(personId);
            var syId = GetCurrentSchoolYearId();
            var healthFormsTask = SchoolLocator.StudentService.GetStudentHealthForms(personId, syId);
            var student = SchoolLocator.StudentService.GetById(personId, GetCurrentSchoolYearId());
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(personId);
            var res = PrepareScheduleData(StudentProfileViewData.Create(student, customAlerts, await studentHealths, await healthFormsTask));
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]
        public ActionResult GetStudents(string filter, bool? myStudentsOnly, int? start, int? count, int? classId, int? schoolId, int? gradeLevel, int? programId, bool? byLastName, int? markingPeriodId, bool? enrolledOnly)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);

            enrolledOnly = enrolledOnly ?? false;

            int? classMatesToId = null;
            int? teacherId = null;

            if (CoreRoles.TEACHER_ROLE == SchoolLocator.Context.Role)
                schoolId = Context.SchoolLocalId;

            if (CoreRoles.STUDENT_ROLE == SchoolLocator.Context.Role)
                classMatesToId = Context.PersonId;
            else
                if (myStudentsOnly == true)
                    if (Context.Claims.HasPermission(ClaimInfo.VIEW_STUDENT) || Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_STUDENTS))
                            teacherId = SchoolLocator.Context.PersonId;
                    else
                        throw new ChalkableException($"User has no required ({ClaimInfo.VIEW_STUDENT} or {ClaimInfo.VIEW_CLASSROOM_STUDENTS}) permission for watch \"My Students\"");
                else
                    if (!Context.Claims.HasPermission(ClaimInfo.VIEW_STUDENT))
                        throw new ChalkableException($"User has no required ({ClaimInfo.VIEW_STUDENT}) permission for watch \"All Students\"");
            
            var res = SchoolLocator.StudentService.SearchStudents(Context.SchoolYearId.Value, classId, schoolId, gradeLevel, programId, teacherId, classMatesToId, filter, 
                byLastName != true, start ?? 0, count ?? 10, markingPeriodId, enrolledOnly.Value);
            return Json(res.Transform(StudentSchoolsInfoViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> Explorer(int personId)
        {
            var syId = GetCurrentSchoolYearId();
            var studentExplorerInfo = SchoolLocator.StudentService.GetStudentExplorerInfo(personId, syId);
            var healthFormsTask = SchoolLocator.StudentService.GetStudentHealthForms(personId, syId);
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(personId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(personId);

            MasterLocator.UserTrackingService.UsedStandardsExplorer(Context.Login, "student explorer");
            var res = StudentExplorerViewData.Create(await studentExplorerInfo, await stHealsConditions, customAlerts, Context.Claims, await healthFormsTask);
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> AttendanceSummary(int studentId, int? gradingPeriodId)
        {
            var syId = GetCurrentSchoolYearId();
            var healthFormsTask = SchoolLocator.StudentService.GetStudentHealthForms(studentId, syId);
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syId);
            var gp = gradingPeriodId.HasValue
                         ? SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId.Value)
                         : SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syId, Context.NowSchoolYearTime.Date);
            var studentSummary = SchoolLocator.AttendanceService.GetStudentAttendanceSummary(studentId, gradingPeriodId);

            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);
            var res = StudentAttendanceSummaryViewData.Create(studentSummary, gp, gradingPeriods, customAlerts, await stHealsConditions, await healthFormsTask);
            
            return Json(res);
        }
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> Apps(int studentId, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 12;

            var syId = GetCurrentSchoolYearId();
            var healthFormsTask = SchoolLocator.StudentService.GetStudentHealthForms(studentId, syId);
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);

            var student = SchoolLocator.StudentService.GetById(studentId, syId);
            var apps = MasterLocator.ApplicationService.GetApplications(start.Value, count.Value, true)
                .Where(x => MasterLocator.ApplicationService.HasMyApps(x))
                .Select(BaseApplicationViewData.Create).ToList();
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);

            return Json(StudentAppsViewData.Create(student, apps, customAlerts, await stHealsConditions, await healthFormsTask));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> DisciplineSummary(int studentId, int? gradingPeriodId)
        {
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);

            var syId = GetCurrentSchoolYearId();
            var healthFormsTask = SchoolLocator.StudentService.GetStudentHealthForms(studentId, syId);
            var student = SchoolLocator.StudentService.GetById(studentId, syId);
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syId);
            var gp = gradingPeriodId.HasValue
                         ? SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId.Value)
                         : SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syId, Context.NowSchoolYearTime.Date);
            var infractionSummaries = SchoolLocator.DisciplineService.GetStudentInfractionSummary(studentId, gradingPeriodId);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);

            var res = StudentDisciplineSummaryViewData.Create(student, infractionSummaries, gp, gradingPeriods, customAlerts, await stHealsConditions, await healthFormsTask);
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> GradingSummary(int studentId)
        {
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);

            var syId = GetCurrentSchoolYearId();
            var healthFormsTask = SchoolLocator.StudentService.GetStudentHealthForms(studentId, syId);
            var student = SchoolLocator.StudentService.GetById(studentId, syId);
            var studentAveragesTask = SchoolLocator.GradingStatisticService.GetStudentAveragesByStudentId(syId, studentId);

            var enrolledClassIds = SchoolLocator.ClassService.GetClassPersons(studentId, null, true, null).Select(x => x.ClassRef);

            var studentAverages = await studentAveragesTask;
            var classIds = studentAverages.Select(x => x.ClassId).ToList();
            var classes = SchoolLocator.ClassService.GetByIds(classIds);

            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syId);
            var gp = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syId, Context.NowSchoolYearTime.Date);
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);
            var res = StudentProfileGradingSummaryViewData.Create(student, studentAverages, gp, gradingPeriods, classes, enrolledClassIds, customAlerts, await stHealsConditions, await healthFormsTask);
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> GradingDetails(int studentId, int gradingPeriodId)
        {
            var syId = GetCurrentSchoolYearId();
            var gradingDetailsTask = SchoolLocator.GradingStatisticService.GetStudentGradingDetails(syId, studentId, gradingPeriodId);
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(studentId);
            var healthFormsTask = SchoolLocator.StudentService.GetStudentHealthForms(studentId, syId);
            
            var customAlerts = SchoolLocator.StudentCustomAlertDetailService.GetList(studentId);
            

            var res = StudentProfileGradingDetailViewData.Create(await gradingDetailsTask, customAlerts, 
                await stHealsConditions, Context.Claims, await healthFormsTask);
            return Json(res);
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public async Task<ActionResult> Panorama(int studentId, StudentProfilePanoramaSetting settings)
        {
            if (!Context.Claims.HasPermission(ClaimInfo.VIEW_PANORAMA))
                throw new ChalkableSecurityException("You are not allowed to view class panorama");

            if (settings.AcadYears == null)
                settings = SchoolLocator.PanoramaSettingsService.Get<StudentProfilePanoramaSetting>(null);

            if (settings.AcadYears.Count == 0)
                throw new ChalkableException("Academic years is required parameter");

            var studentPanorama = await SchoolLocator.StudentService.Panorama(studentId, settings.AcadYears, settings.StandardizedTestFilters);
            var standardizedTests = SchoolLocator.StandardizedTestService.GetListOfStandardizedTestDetails();
            
            return Json(StudentPanoramaViewData.Create(studentId, studentPanorama, settings, standardizedTests));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult SavePanoramaSettings(StudentProfilePanoramaSetting setting)
        {
            if (!Context.Claims.HasPermission(ClaimInfo.VIEW_PANORAMA))
                throw new ChalkableSecurityException("You are not allowed to change panorama settings");

            if (setting.AcadYears == null || setting.AcadYears.Count == 0)
                throw new ChalkableException("Academic years is required parameter");

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