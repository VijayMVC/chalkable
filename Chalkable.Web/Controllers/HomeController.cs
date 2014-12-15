using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Common;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.AttendancesViewData;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;
using Chalkable.Web.Models.SchoolsViewData;
using Chalkable.Web.Tools;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class HomeController : ChalkableController
    {

        private const string DEMO_PICTURE_DISTRICT_REF = "99a2b309-b2f2-451d-a733-55ffb9548245";

        public ActionResult Index()
        {
            return View();
        }
      
        public ActionResult Terms()
        {
            return View();
        }


        //[AuthorizationFilter("AppTester")]
        public ActionResult AppTester()
        {
            var appTester = new User
            {
                Id = Guid.NewGuid(),
                Login = "apptester@chalkable.com",
                FullName = "App Tester"
            };
            //var appTester = MasterLocator.UserService.GetById(Context.UserId);
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            ViewData[ViewConstants.DEMO_AZURE_PICTURE_URL] = PictureService.GeDemoPicturesRelativeAddress();
            PrepareJsonData(AppTesterViewData.Create(appTester), ViewConstants.CURRENT_PERSON);
            var serverTime = Context.NowSchoolTime.ToString("yyyy/MM/dd hh:mm:ss tt");
            ViewData[ViewConstants.SERVER_TIME] = serverTime;
            //var ip = RequestHelpers.GetClientIpAddress(Request);
            //MasterLocator.UserTrackingService.IdentifySysAdmin(sysUser.Login, "", "", null, ip);
            return View();
        }

        public static string PasswordMd5(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            var b64 = Convert.ToBase64String(hash);
            return b64;
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult SysAdmin()
        {
            var sysUser = MasterLocator.UserService.GetById(Context.UserId);
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            ViewData[ViewConstants.DEMO_AZURE_PICTURE_URL] = PictureService.GeDemoPicturesRelativeAddress();
            PrepareJsonData(SysAdminViewData.Create(sysUser), ViewConstants.CURRENT_PERSON);
            var serverTime = Context.NowSchoolTime.ToString("yyyy/MM/dd hh:mm:ss tt");
            ViewData[ViewConstants.SERVER_TIME] = serverTime;
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifySysAdmin(sysUser.Login, "", "", null, ip);

            ViewData[ViewConstants.ROLE_NAME] = CoreRoles.SUPER_ADMIN_ROLE.LoweredName;
            return View();
        }

        [AuthorizationFilter("Developer")]
        public ActionResult Developer(Guid? currentApplicationId, bool? isPwdReset)
        {
            if (isPwdReset.HasValue && isPwdReset.Value)
                ViewData[ViewConstants.REDIRECT_URL_KEY] = UrlsConstants.DEV_RESET_PASSWORD_URL;

            var developer = MasterLocator.DeveloperService.GetById(MasterLocator.Context.UserId);
            PrepareJsonData(DeveloperViewData.Create(developer), ViewConstants.CURRENT_PERSON);
            var applications = MasterLocator.ApplicationService.GetApplications(0, int.MaxValue, false);
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            ViewData[ViewConstants.DEMO_AZURE_PICTURE_URL] = PictureService.GeDemoPicturesRelativeAddress();
            var serverTime = Context.NowSchoolTime.ToString("yyyy/MM/dd hh:mm:ss tt");
            ViewData[ViewConstants.SERVER_TIME] = serverTime;
            ViewData[ViewConstants.SCHOOL_YEAR_SERVER_TIME] = Context.NowSchoolYearTime.ToString("yyyy/MM/dd hh:mm:ss tt");
            ViewData[ViewConstants.NEEDS_TOUR] = false;
            ViewData[ViewConstants.ROLE_NAME] = Context.Role.LoweredName;
            ViewData[ViewConstants.CURRENT_USER_ROLE_ID] = Context.RoleId;
            ViewData[ViewConstants.STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
            ViewData[ViewConstants.TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
            ViewData[ViewConstants.ADMIN_GRADE_ROLE] = CoreRoles.ADMIN_GRADE_ROLE.Name;
            ViewData[ViewConstants.ADMIN_EDIT_ROLE] = CoreRoles.ADMIN_EDIT_ROLE.Name;
            ViewData[ViewConstants.ADMIN_VIEW_ROLE] = CoreRoles.ADMIN_VIEW_ROLE.Name;
            ViewData[ViewConstants.DEMO_PREFIX_KEY] = Context.UserId.ToString();
            ViewData[ViewConstants.DEMO_PICTURE_DISTRICT_REF] = DEMO_PICTURE_DISTRICT_REF;

            if (Context.DistrictId.HasValue)
            {
                var district = DemoDistrictStorage.CreateDemoDistrict(Context.DistrictId.Value);
                var school = DemoMasterSchoolStorage.CreateMasterSchool(Context.DistrictId.Value);
                school.District = district;
                PrepareJsonData(ShortSchoolViewData.Create(school), ViewConstants.SCHOOL);
            }


            PrepareJsonData(BaseApplicationViewData.Create(applications), ViewConstants.APPLICATIONS);
            if (applications.Count > 0)
            {
                var app = currentApplicationId.HasValue ? applications.First(x => x.Id == currentApplicationId) : applications.Last();
                app = MasterLocator.ApplicationService.GetApplicationById(app.Id);
                var res = ApplicationController.PrepareAppInfo(MasterLocator, app, true, true);
                PrepareJsonData(res, ViewConstants.DEFAULT_APPLICATION, 6);
            }
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifyDeveloper(developer.Email, developer.DisplayName, DateTime.UtcNow, "UTC", ip);
            return View();
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Teacher()
        {
            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            PrepareTeacherJsonData(mp);
            return View();
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult Admin()
        {
            PrepareAdminJsonData();
            
            return View();
        }

        [AuthorizationFilter("Student")]
        public ActionResult Student()
        {
            PrepareStudentJsonData();
            return View();
        }

        private void PrepareCommonViewData(MarkingPeriod markingPeriod = null)
        {
            //TODO: render data for demo school 
            if (Context.DistrictId.HasValue && Context.SchoolLocalId.HasValue)
            {
                var district = MasterLocator.DistrictService.GetByIdOrNull(Context.DistrictId.Value);
                var school = MasterLocator.SchoolService.GetById(Context.DistrictId.Value, Context.SchoolLocalId.Value);
                school.District = district;
                PrepareJsonData(ShortSchoolViewData.Create(school), ViewConstants.SCHOOL);
                if (Context.DeveloperId != null)
                    ViewData[ViewConstants.IS_DEV] = true;
                if (district.IsDemoDistrict)
                {
                    ViewData[ViewConstants.STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
                    ViewData[ViewConstants.TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_GRADE_ROLE] = CoreRoles.ADMIN_GRADE_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_EDIT_ROLE] = CoreRoles.ADMIN_EDIT_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_VIEW_ROLE] = CoreRoles.ADMIN_VIEW_ROLE.Name;
                    ViewData[ViewConstants.DEMO_PREFIX_KEY] = district.Id.ToString();
                    ViewData[ViewConstants.DEMO_PICTURE_DISTRICT_REF] = DEMO_PICTURE_DISTRICT_REF;
                }
                ViewData[ViewConstants.LAST_SYNC_DATE] = district.LastSync.HasValue 
                    ? district.LastSync.Value.ToString("yyyy/MM/dd hh:mm:ss")
                    : "";

            }
            ViewData[ViewConstants.CURRENT_USER_ROLE_ID] = Context.RoleId;
            ViewData[ViewConstants.ROLE_NAME] = Context.Role.LoweredName;

            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            ViewData[ViewConstants.DEMO_AZURE_PICTURE_URL] = PictureService.GeDemoPicturesRelativeAddress();
            ViewData[ViewConstants.CURR_SCHOOL_YEAR_ID] = GetCurrentSchoolYearId();
            ViewData[ViewConstants.VERSION] = CompilerHelper.Version;
            ViewData[ViewConstants.CROCODOC_API_URL] = PreferenceService.Get(Preference.CROCODOC_URL).Value;
            var serverTime = Context.NowSchoolTime.ToString("yyyy/MM/dd hh:mm:ss tt");
            ViewData[ViewConstants.SERVER_TIME] = serverTime;
            ViewData[ViewConstants.SCHOOL_YEAR_SERVER_TIME] = Context.NowSchoolYearTime.ToString("yyyy/MM/dd hh:mm:ss tt");
            PrepareJsonData(Context.Claims, ViewConstants.USER_CLAIMS);

            //PrepareJsonData(AttendanceReasonViewData.Create(SchoolLocator.AttendanceReasonService.List()), ViewConstants.ATTENDANCE_REASONS);

            ViewData[ViewConstants.UNSHOWN_NOTIFICATIONS_COUNT] = SchoolLocator.NotificationService.GetUnshownNotifications().Count;
            if (markingPeriod != null && SchoolLocator.Context.SchoolLocalId.HasValue)
            {
                PrepareJsonData(MarkingPeriodViewData.Create(markingPeriod), ViewConstants.MARKING_PERIOD);
                var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(markingPeriod.SchoolYearRef, Context.NowSchoolYearTime.Date);
                if (gradingPeriod != null)
                    PrepareJsonData(ShortGradingPeriodViewData.Create(gradingPeriod), ViewConstants.GRADING_PERIOD);

                var nextmp = SchoolLocator.MarkingPeriodService.GetNextMarkingPeriodInYear(markingPeriod.Id);
                if (nextmp != null)
                    PrepareJsonData(MarkingPeriodViewData.Create(nextmp), ViewConstants.NEXT_MARKING_PERIOD);
            }
            PrepareJsonData(AlphaGradeViewData.Create(SchoolLocator.AlphaGradeService.GetAlphaGrades()), ViewConstants.ALPHA_GRADES);
            PrepareJsonData(AlternateScoreViewData.Create(SchoolLocator.AlternateScoreService.GetAlternateScores()), ViewConstants.ALTERNATE_SCORES);
            PrepareJsonData(MarkingPeriodViewData.Create(SchoolLocator.MarkingPeriodService.GetMarkingPeriods(Context.SchoolYearId)), ViewConstants.MARKING_PERIODS);
        }
        

        private void PrepareStudentJsonData()
        {
            var person = SchoolLocator.PersonService.GetPersonDetails(Context.PersonId.Value);
            var personView = PersonInfoViewData.Create(person);
            personView.DisplayName = person.FullName();

            if (!person.FirstLoginDate.HasValue)
            {
                SchoolLocator.PersonService.ProcessPersonFirstLogin(person.Id);
            }
            if (!person.Active)
            {
                ViewData[ViewConstants.REDIRECT_URL_KEY] = string.Format(UrlsConstants.SETUP_URL_FORMAT, Context.PersonId);
                var personEmail = SchoolLocator.PersonEmailService.GetPersonEmail(person.Id);
                personView.Email = personEmail == null ? null : personEmail.EmailAddress;
            }
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);
            
            //todo: move this logic to getClass stored procedure later
            var classPersons = SchoolLocator.ClassService.GetClassPersons(person.Id, true);
            var classes = SchoolLocator.ClassService.GetClassesSortedByPeriod().Where(c => classPersons.Any(cp => cp.ClassRef == c.Id)).ToList();
            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
           
            PrepareJsonData(ClassViewData.Create(classes), ViewConstants.CLASSES);
            PrepareCommonViewData(mp);

            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifyStudent(person.Email, person.FirstName, person.LastName, Context.DistrictId.ToString(), "", person.FirstLoginDate, Context.DistrictTimeZone, ip);
        }           

        private void PrepareAdminJsonData()
        {
            Trace.Assert(Context.PersonId.HasValue);
            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            var person = SchoolLocator.PersonService.GetPersonDetails(Context.PersonId.Value);
            var personView = PersonViewData.Create(person);
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);
            var gradeLevels = SchoolLocator.GradeLevelService.GetGradeLevels();
            PrepareJsonData(GradeLevelViewData.Create(gradeLevels), ViewConstants.GRADE_LEVELS);
            PrepareJsonData(AttendanceReasonDetailsViewData.Create(SchoolLocator.AttendanceReasonService.List()), ViewConstants.ATTENDANCE_REASONS);
            PrepareCommonViewData(mp);
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifyAdmin(person.Email, person.FirstName, person.LastName, Context.DistrictId.ToString(), person.FirstLoginDate, Context.DistrictTimeZone, "Admin", ip);
        }

        private void PrepareTeacherJsonData(MarkingPeriod mp)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            PrepareCommonViewData(mp);
            var person = SchoolLocator.PersonService.GetPersonDetails(Context.PersonId.Value);
            var personView = PersonInfoViewData.Create(person);
            if (!person.FirstLoginDate.HasValue)
            {
                SchoolLocator.PersonService.ProcessPersonFirstLogin(person.Id);
            }
            if (!person.Active)
            {
                ViewData[ViewConstants.REDIRECT_URL_KEY] = string.Format(UrlsConstants.SETUP_URL_FORMAT, Context.PersonId);
                var personEmail = SchoolLocator.PersonEmailService.GetPersonEmail(person.Id);
                personView.Email = personEmail == null ? null : personEmail.EmailAddress;
            }
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);


            if (!CanTeacherViewChalkable()) return;

            var classes = SchoolLocator.ClassService.GetClassesSortedByPeriod();
            var gradeLevels = classes.Select(x => x.GradeLevel.Name).Distinct().ToList();
            var classNames = classes.Select(x => x.Name).ToList();

            var schoolOption = SchoolLocator.SchoolService.GetSchoolOption();
            PrepareJsonData(SchoolOptionViewData.Create(schoolOption), ViewConstants.SCHOOL_OPTIONS);
            var classesList = classes.Select(ClassViewData.Create).ToList();
            PrepareJsonData(classesList, ViewConstants.CLASSES);
            PrepareClassesAdvancedData(classes);
            PrepareJsonData(GradingCommentViewData.Create(SchoolLocator.GradingCommentService.GetGradingComments()), ViewConstants.GRADING_COMMMENTS);
            PrepareJsonData(AttendanceReasonDetailsViewData.Create(SchoolLocator.AttendanceReasonService.List()), ViewConstants.ATTENDANCE_REASONS);
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifyTeacher(Context.Login, person.FirstName, person.LastName, Context.DistrictId.ToString(), 
                gradeLevels, classNames, person.FirstLoginDate, Context.DistrictTimeZone, ip);
        }
        
        private void PrepareClassesAdvancedData(IEnumerable<ClassDetails> classDetailsList)
        {
            var classesAdvancedData = new List<object>();
            var allAlphaGrades = SchoolLocator.AlphaGradeService.GetAlphaGrades();
            var classAnnouncementTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classDetailsList.Select(x => x.Id).ToList());
            foreach (var classDetails in classDetailsList)
            {

                int classId = classDetails.Id;
                var typesByClasses = classAnnouncementTypes.Where(x => x.ClassRef == classId).ToList();
                classesAdvancedData.Add(new
                {
                    ClassId = classId,
                    TypesByClass = ClassAnnouncementTypeViewData.Create(typesByClasses),
                    AlphaGrades = classDetails.GradingScaleRef.HasValue
                                        ? SchoolLocator.AlphaGradeService.GetAlphaGradesForClass(classDetails.Id)
                                        : allAlphaGrades,
                    AlphaGradesForStandards = SchoolLocator.AlphaGradeService.GetAlphaGradesForClassStandards(classId)
                });
            }
            PrepareJsonData(classesAdvancedData, ViewConstants.CLASSES_ADV_DATA);
        }

        private bool CanTeacherViewChalkable()
        {
            return ClaimInfo.HasPermission(Context.Claims, new List<string> {ClaimInfo.VIEW_CLASSROOM})
                   || ClaimInfo.HasPermission(Context.Claims, new List<string> {ClaimInfo.VIEW_CLASSROOM_ADMIN});
        }
    }
}
