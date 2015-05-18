using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
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
        private const string DATE_TIME_FORMAT = "yyyy/MM/dd hh:mm:ss tt";

        public ActionResult HomeRedirect()
        {
            return Redirect(Settings.HomeRedirectUrl);
        }

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
            var serverTime = Context.NowSchoolTime.ToString(DATE_TIME_FORMAT);
            ViewData[ViewConstants.ROLE_NAME] = Context.Role.LoweredName;
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
            ViewData[ViewConstants.SERVER_TIME] = Context.NowSchoolTime.ToString(DATE_TIME_FORMAT);
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifySysAdmin(sysUser.Login, "", "", null, ip);

            ViewData[ViewConstants.ROLE_NAME] = CoreRoles.SUPER_ADMIN_ROLE.LoweredName;
            return View();
        }


        //[AuthorizationFilter("District")]
        //public ActionResult DistrictAdmin()
        //{
        //    var distictAdmin = MasterLocator.UserService.GetById(Context.UserId);
        //    ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
        //    ViewData[ViewConstants.DEMO_AZURE_PICTURE_URL] = PictureService.GeDemoPicturesRelativeAddress();
        //    ViewData[ViewConstants.SERVER_TIME] = Context.NowSchoolTime.ToString(DATE_TIME_FORMAT);
        //    ViewData[ViewConstants.SCHOOL_YEAR_SERVER_TIME] = Context.NowSchoolYearTime.ToString(DATE_TIME_FORMAT);
        //    ViewData[ViewConstants.ROLE_NAME] = CoreRoles.DISTRICT_ROLE.LoweredName;
        //    PrepareJsonData(UserViewData.Create(distictAdmin), ViewConstants.CURRENT_PERSON);
        //    //todo : add user tracking for districtadmin identify
        //    return View();
        //}

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
            ViewData[ViewConstants.SERVER_TIME] = Context.NowSchoolTime.ToString(DATE_TIME_FORMAT);
            ViewData[ViewConstants.SCHOOL_YEAR_SERVER_TIME] = Context.NowSchoolYearTime.ToString(DATE_TIME_FORMAT);
            ViewData[ViewConstants.NEEDS_TOUR] = false;
            ViewData[ViewConstants.ROLE_NAME] = Context.Role.LoweredName;
            ViewData[ViewConstants.CURRENT_USER_ROLE_ID] = Context.RoleId;
            ViewData[ViewConstants.STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
            ViewData[ViewConstants.TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
            ViewData[ViewConstants.ADMIN_GRADE_ROLE] = CoreRoles.ADMIN_GRADE_ROLE.Name;
            ViewData[ViewConstants.ADMIN_EDIT_ROLE] = CoreRoles.ADMIN_EDIT_ROLE.Name;
            ViewData[ViewConstants.ADMIN_VIEW_ROLE] = CoreRoles.ADMIN_VIEW_ROLE.Name;
            ViewData[ViewConstants.DISTRICT_ID] = Context.UserId.ToString();
            ViewData[ViewConstants.DEMO_PICTURE_DISTRICT_REF] = DEMO_PICTURE_DISTRICT_REF;
            ViewData[ViewConstants.IS_DEMO_DISTRICT] = true;
            if (Context.DistrictId.HasValue)
            {
                var district = DemoDistrictService.CreateDemoDistrict(Context.DistrictId.Value);
                var school = DemoSchoolService.CreateMasterSchool(Context.DistrictId.Value);
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
            PrepareTeacherJsonData();
            return View();
        }

        [AuthorizationFilter("Student")]
        public ActionResult Student()
        {
            PrepareStudentJsonData();
            return View();
        }

        private void PrepareCommonViewData(StartupData startupData)
        {
            //TODO: render data for demo school 
            if (Context.DistrictId.HasValue && Context.SchoolLocalId.HasValue)
            {
                var district = MasterLocator.DistrictService.GetByIdOrNull(Context.DistrictId.Value);
                if (Context.DeveloperId != null)
                    ViewData[ViewConstants.IS_DEV] = true;
                if (district.IsDemoDistrict)
                {
                    ViewData[ViewConstants.STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
                    ViewData[ViewConstants.TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_GRADE_ROLE] = CoreRoles.ADMIN_GRADE_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_EDIT_ROLE] = CoreRoles.ADMIN_EDIT_ROLE.Name;
                    ViewData[ViewConstants.ADMIN_VIEW_ROLE] = CoreRoles.ADMIN_VIEW_ROLE.Name;
                    
                    ViewData[ViewConstants.DEMO_PICTURE_DISTRICT_REF] = DEMO_PICTURE_DISTRICT_REF;
                    ViewData[ViewConstants.IS_DEMO_DISTRICT] = true;
                }
                ViewData[ViewConstants.LAST_SYNC_DATE] = district.LastSync.HasValue 
                    ? district.LastSync.Value.ToString("yyyy/MM/dd hh:mm:ss")
                    : "";
                ViewData[ViewConstants.DISTRICT_ID] = district.Id.ToString();
            }
            ViewData[ViewConstants.CURRENT_USER_ROLE_ID] = Context.RoleId;
            ViewData[ViewConstants.ROLE_NAME] = Context.Role.LoweredName;
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            ViewData[ViewConstants.DEMO_AZURE_PICTURE_URL] = PictureService.GeDemoPicturesRelativeAddress();
            ViewData[ViewConstants.CURR_SCHOOL_YEAR_ID] = GetCurrentSchoolYearId();
            ViewData[ViewConstants.VERSION] = CompilerHelper.Version;
            ViewData[ViewConstants.CROCODOC_API_URL] = PreferenceService.Get(Preference.CROCODOC_URL).Value;
            ViewData[ViewConstants.SERVER_TIME] = Context.NowSchoolTime.ToString(DATE_TIME_FORMAT);
            ViewData[ViewConstants.SCHOOL_YEAR_SERVER_TIME] = Context.NowSchoolYearTime.ToString(DATE_TIME_FORMAT);
            ViewData[ViewConstants.STUDY_CENTER_ENABLED] = Context.SCEnabled;
            PrepareJsonData(Context.Claims, ViewConstants.USER_CLAIMS);


            ViewData[ViewConstants.UNSHOWN_NOTIFICATIONS_COUNT] = startupData.UnshownNotificationsCount;

            var mps = startupData.MarkingPeriods;
            MarkingPeriod markingPeriod = mps.Where(x=>x.StartDate <= Context.NowSchoolYearTime).OrderBy(x=>x.StartDate).LastOrDefault();
            if (markingPeriod != null && SchoolLocator.Context.SchoolLocalId.HasValue)
            {
                PrepareJsonData(MarkingPeriodViewData.Create(markingPeriod), ViewConstants.MARKING_PERIOD);
                var gradingPeriod = startupData.GradingPeriod;
                if (gradingPeriod != null)
                    PrepareJsonData(ShortGradingPeriodViewData.Create(gradingPeriod), ViewConstants.GRADING_PERIOD);
            }
            PrepareJsonData(AlphaGradeViewData.Create(startupData.AlphaGrades), ViewConstants.ALPHA_GRADES);
            PrepareJsonData(AlternateScoreViewData.Create(startupData.AlternateScores), ViewConstants.ALTERNATE_SCORES);
            PrepareJsonData(MarkingPeriodViewData.Create(mps), ViewConstants.MARKING_PERIODS);
            var sy = SchoolLocator.SchoolYearService.GetCurrentSchoolYear();
            PrepareJsonData(SchoolYearViewData.Create(sy), ViewConstants.SCHOOL_YEAR);
           
        }
        

        private void PrepareStudentJsonData()
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var startupData = SchoolLocator.SchoolService.GetStartupData();

            PrepareCommonViewData(startupData);
            var person = startupData.Person;
            var personView = PersonInfoViewData.Create(person);
            ProcessFirstLogin(person);
            ProcessActive(person, personView);
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);
            var classes = startupData.Classes;
            PrepareJsonData(ClassViewData.Create(classes), ViewConstants.CLASSES);
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifyStudent(Context.Login, person.FirstName, person.LastName, Context.DistrictId.ToString(), "", person.FirstLoginDate, Context.DistrictTimeZone, ip);
        }           

        private void PrepareTeacherJsonData()
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var startupData = SchoolLocator.SchoolService.GetStartupData();

            PrepareCommonViewData(startupData);
            var person = startupData.Person;
            var personView = PersonInfoViewData.Create(person);
            ProcessFirstLogin(person);
            ProcessActive(person, personView);
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);


            if (!CanTeacherViewChalkable()) return;

            var classes = startupData.Classes;
            var classNames = classes.Select(x => x.Name).ToList();

            var schoolOption = startupData.SchoolOption;
            PrepareJsonData(SchoolOptionViewData.Create(schoolOption), ViewConstants.SCHOOL_OPTIONS);
            var classesList = classes.Select(ClassViewData.Create).ToList();
            PrepareJsonData(classesList, ViewConstants.CLASSES);
            PrepareClassesAdvancedData(startupData);
            PrepareJsonData(GradingCommentViewData.Create(startupData.GradingComments), ViewConstants.GRADING_COMMMENTS);
            PrepareJsonData(AttendanceReasonDetailsViewData.Create(startupData.AttendanceReasons), ViewConstants.ATTENDANCE_REASONS);
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifyTeacher(Context.Login, person.FirstName, person.LastName, Context.DistrictId.ToString(), 
                classNames, person.FirstLoginDate, Context.DistrictTimeZone, ip);
        }

        private void ProcessFirstLogin(PersonDetails person)
        {
            if (!person.FirstLoginDate.HasValue)
                SchoolLocator.PersonService.ProcessPersonFirstLogin(person.Id);
        }

        private void ProcessActive(PersonDetails person, PersonInfoViewData personView)
        {
            if (!person.Active)
            {
                ViewData[ViewConstants.REDIRECT_URL_KEY] = string.Format(UrlsConstants.SETUP_URL_FORMAT, Context.PersonId);
                var personEmail = SchoolLocator.PersonEmailService.GetPersonEmail(person.Id);
                personView.Email = personEmail == null ? null : personEmail.EmailAddress;
            }
        }
        
        private void PrepareClassesAdvancedData(StartupData startupData)
        {
            var classesAdvancedData = new List<object>();
            var allAlphaGrades = startupData.AlphaGrades;
            var classAnnouncementTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(startupData.Classes.Select(x => x.Id).ToList());
            foreach (var classDetails in startupData.Classes)
            {
                var typesByClasses = classAnnouncementTypes.Where(x => x.ClassRef == classDetails.Id).ToList();
                classesAdvancedData.Add(new
                {
                    ClassId = classDetails.Id,
                    TypesByClass = ClassAnnouncementTypeViewData.Create(typesByClasses),
                    AlphaGrades = classDetails.GradingScaleRef.HasValue
                                        ? startupData.AlphaGradesForClasses[classDetails.Id]
                                        : allAlphaGrades,
                    AlphaGradesForStandards = startupData.AlphaGradesForClassStandards[classDetails.Id]
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
