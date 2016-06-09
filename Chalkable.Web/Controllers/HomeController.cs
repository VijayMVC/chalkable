using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Common;
using Chalkable.Web.Extensions;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.AttendancesViewData;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.GradingViewData;
using Chalkable.Web.Models.PersonViewDatas;
using Chalkable.Web.Models.SchoolsViewData;
using Chalkable.Web.Tools;
using Microsoft.ReportingServices.Interfaces;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Messages;

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
            ViewData[ViewConstants.ASSESSMENT_APLICATION_ID] = MasterLocator.ApplicationService.GetAssessmentId();
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
            ViewData[ViewConstants.ASSESSMENT_APLICATION_ID] = MasterLocator.ApplicationService.GetAssessmentId();
            return View();
        }

        //TODO: refactor ... too many duplications with Teacher()
        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DistrictAdmin()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            var distictAdmin = SchoolLocator.PersonService.GetPersonDetails(Context.PersonId.Value);
            var district = PrepareCommonViewData();
            ViewData[ViewConstants.ROLE_NAME] = CoreRoles.DISTRICT_ADMIN_ROLE.LoweredName;
            PrepareJsonData(PersonViewData.Create(distictAdmin), ViewConstants.CURRENT_PERSON);
            var gradeLevel = SchoolLocator.GradeLevelService.GetGradeLevels();
            PrepareJsonData(GradeLevelViewData.Create(gradeLevel), ViewConstants.GRADE_LEVELS);
            PrepareJsonData(AttendanceReasonDetailsViewData.Create(SchoolLocator.AttendanceReasonService.GetAll()), ViewConstants.ATTENDANCE_REASONS);

            var sy = SchoolLocator.SchoolYearService.GetCurrentSchoolYear();
            PrepareJsonData(SchoolYearViewData.Create(sy), ViewConstants.SCHOOL_YEAR);

            var announcementAttributes = SchoolLocator.AnnouncementAttributeService.GetList(true);
            PrepareJsonData(AnnouncementAttributeViewData.Create(announcementAttributes), ViewConstants.ANNOUNCEMENT_ATTRIBUTES);
            
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(Context.SchoolYearId.Value);
            var currentGradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(Context.SchoolYearId.Value, Context.NowSchoolYearTime.Date);

            PrepareJsonData(GradingPeriodViewData.Create(gradingPeriods), ViewConstants.GRADING_PERIODS);
            PrepareJsonData(ShortGradingPeriodViewData.Create(currentGradingPeriod), ViewConstants.GRADING_PERIOD);

            var mps = SchoolLocator.MarkingPeriodService.GetMarkingPeriods(sy.Id);
            PrepareJsonData(MarkingPeriodViewData.Create(mps), ViewConstants.MARKING_PERIODS);

            var schoolOption = SchoolLocator.SchoolService.GetSchoolOption();
            PrepareJsonData(SchoolOptionViewData.Create(schoolOption), ViewConstants.SCHOOL_OPTIONS);

            var alternateScore = SchoolLocator.AlternateScoreService.GetAlternateScores();
            PrepareJsonData(AlternateScoreViewData.Create(alternateScore), ViewConstants.ALTERNATE_SCORES);

            var gradingComments = SchoolLocator.GradingCommentService.GetGradingComments();
            PrepareJsonData(GradingCommentViewData.Create(gradingComments), ViewConstants.GRADING_COMMMENTS);
            
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifyDistrictAdmin(distictAdmin.Email, "", "", 
                district.Name, null, Context.DistrictTimeZone, Context.Role.Name, ip, Context.SCEnabled);
            return View();
        }

        [AuthorizationFilter("Developer")]
        public ActionResult Developer(Guid? currentApplicationId, bool? isPwdReset)
        {
            if (isPwdReset.HasValue && isPwdReset.Value)
                ViewData[ViewConstants.REDIRECT_URL_KEY] = UrlsConstants.DEV_RESET_PASSWORD_URL;

            var developer = MasterLocator.DeveloperService.GetById(MasterLocator.Context.UserId);
            PrepareJsonData(DeveloperViewData.Create(developer), ViewConstants.CURRENT_PERSON);
            var applications = MasterLocator.ApplicationService.GetApplicationsWithLive(developer.Id, null, null);
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            ViewData[ViewConstants.DEMO_AZURE_PICTURE_URL] = PictureService.GeDemoPicturesRelativeAddress();
            ViewData[ViewConstants.SERVER_TIME] = Context.NowSchoolTime.ToString(DATE_TIME_FORMAT);
            ViewData[ViewConstants.SCHOOL_YEAR_SERVER_TIME] = Context.NowSchoolYearTime.ToString(DATE_TIME_FORMAT);
            ViewData[ViewConstants.NEEDS_TOUR] = false;
            ViewData[ViewConstants.ROLE_NAME] = Context.Role.LoweredName;
            ViewData[ViewConstants.CURRENT_USER_ROLE_ID] = Context.RoleId;
            ViewData[ViewConstants.STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
            ViewData[ViewConstants.TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
            ViewData[ViewConstants.DISTRICT_ADMIN_ROLE] = CoreRoles.DISTRICT_ADMIN_ROLE.Name;
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

        [HttpGet]
        public string UiLibraryUrl()
        {
            Response.ContentType = "text/css";
            return $"@import url(\"{Url.StaticContent("/Content/ui-library.css")}\")";
        }

        private District PrepareCommonViewData()
        {
            District district = null;
            if (Context.DistrictId.HasValue && Context.SchoolLocalId.HasValue)
            {
                district = MasterLocator.DistrictService.GetByIdOrNull(Context.DistrictId.Value);
                if (Context.DeveloperId != null)
                    ViewData[ViewConstants.IS_DEV] = true;
                if (district.IsDemoDistrict)
                {
                    ViewData[ViewConstants.STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
                    ViewData[ViewConstants.TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
                    ViewData[ViewConstants.DISTRICT_ADMIN_ROLE] = CoreRoles.DISTRICT_ADMIN_ROLE.Name;

                    ViewData[ViewConstants.DEMO_PICTURE_DISTRICT_REF] = DEMO_PICTURE_DISTRICT_REF;
                    ViewData[ViewConstants.IS_DEMO_DISTRICT] = true;
                }
                ViewData[ViewConstants.LAST_SYNC_DATE] = district.LastSync.HasValue
                    ? district.LastSync.Value.ToString("yyyy/MM/dd hh:mm:ss")
                    : "";
                ViewData[ViewConstants.DISTRICT_ID] = district.Id.ToString();

                var messagingSettings = MessagingSettingsViewData.Create(MasterLocator.SchoolService.GetDistrictMessaginSettings(Context.DistrictId.Value));
                PrepareJsonData(messagingSettings, ViewConstants.MESSAGING_SETTINGS);

                //TODO : maybe added this to startup data ... only needs for school persons 
                var school = SchoolLocator.SchoolService.GetSchool(Context.SchoolLocalId.Value);
                ViewData[ViewConstants.SCHOOL_NAME] = school.Name;
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
            ViewData[ViewConstants.ASSESSMENT_ENABLED] = Context.AssessmentEnabled;
            ViewData[ViewConstants.MESSAGING_DISABLED] = Context.MessagingDisabled;
            ViewData[ViewConstants.ASSESSMENT_APLICATION_ID] = MasterLocator.ApplicationService.GetAssessmentId();
            ViewData[ViewConstants.SIS_API_VERSION] = Context.SisApiVersion;

            var leParams = SchoolLocator.LeService.GetLEParams();

            PrepareJsonData(leParams, ViewConstants.LE_PARAMS);
            PrepareJsonData(PersonClaimViewData.Create(Context.Claims), ViewConstants.USER_CLAIMS);
            return district;
        }

        private District PrepareCommonViewDataForSchoolPerson(StartupData startupData)
        {
            //TODO: render data for demo school 
            var district = PrepareCommonViewData();
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
            PrepareJsonData(GradingPeriodViewData.Create(startupData.GradingPeriods), ViewConstants.GRADING_PERIODS);

            PrepareJsonData(MarkingPeriodViewData.Create(mps), ViewConstants.MARKING_PERIODS);
            var sy = SchoolLocator.SchoolYearService.GetCurrentSchoolYear();
            PrepareJsonData(SchoolYearViewData.Create(sy), ViewConstants.SCHOOL_YEAR);
            PrepareJsonData(SchoolLocator.SchoolYearService.GetYears(), ViewConstants.YEARS);

            return district;
        }
        

        private void PrepareStudentJsonData()
        {
            Trace.Assert(Context.PersonId.HasValue);
            var startupData = SchoolLocator.SchoolService.GetStartupData();

            var district = PrepareCommonViewDataForSchoolPerson(startupData);
            var person = startupData.Person;
            var personView = PersonInfoViewData.Create(person);
            ProcessFirstLogin(person);
            ProcessActive(person, personView);
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);
            var classes = startupData.Classes;
            PrepareJsonData(ClassViewData.Create(classes), ViewConstants.CLASSES);
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifyStudent(Context.Login, person.FirstName, person.LastName, 
                district.Name, "", person.FirstLoginDate, Context.DistrictTimeZone, ip, Context.SCEnabled);
        }           

        private void PrepareTeacherJsonData()
        {
            var startProcessingTime = DateTime.Now.TimeOfDay;
            
            var timeCallBuilder = new StringBuilder();
            Trace.Assert(Context.PersonId.HasValue);
            
            var startupData = ProcessMethodAndCallTime(()=>SchoolLocator.SchoolService.GetStartupData(), timeCallBuilder, "Retrieving StartUpData ");
            var district = ProcessMethodAndCallTime(()=> PrepareCommonViewDataForSchoolPerson(startupData), timeCallBuilder, "PrepareCommonSchoolPersonData");

            var person = startupData.Person;
            
            ProcessMethodAndCallTime(() => ProcessFirstLogin(person), timeCallBuilder, "ProcessFirstLogin");
            var personView = PersonInfoViewData.Create(person);

            ProcessMethodAndCallTime(() => ProcessActive(person, personView), timeCallBuilder, "ProcessActive");
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);


            if (!CanTeacherViewChalkable()) return;

            var classes = startupData.Classes;
            var classNames = classes.Select(x => x.Name).ToList();

            var schoolOption = startupData.SchoolOption;
            PrepareJsonData(SchoolOptionViewData.Create(schoolOption), ViewConstants.SCHOOL_OPTIONS);
            var classesList = classes.Select(ClassViewData.Create).ToList();
            PrepareJsonData(classesList, ViewConstants.CLASSES);

            ProcessMethodAndCallTime(() => PrepareClassesAdvancedData(startupData), timeCallBuilder, "Retrieving Activity Category from Inow");
            PrepareClassesAdvancedData(startupData);
            
            PrepareJsonData(GradingCommentViewData.Create(startupData.GradingComments), ViewConstants.GRADING_COMMMENTS);

            PrepareJsonData(AttendanceReasonDetailsViewData.Create(startupData.AttendanceReasons), ViewConstants.ATTENDANCE_REASONS);

            var announcementAttributes = ProcessMethodAndCallTime(()=>SchoolLocator.AnnouncementAttributeService.GetList(true), timeCallBuilder, "Retrieving AnnouncementAttribute");
            
            PrepareJsonData(AnnouncementAttributeViewData.Create(announcementAttributes), ViewConstants.ANNOUNCEMENT_ATTRIBUTES);

            var ip = RequestHelpers.GetClientIpAddress(Request);
            MasterLocator.UserTrackingService.IdentifyTeacher(Context.Login, person.FirstName, person.LastName, district.Name, 
                classNames, person.FirstLoginDate, Context.DistrictTimeZone, ip, Context.SCEnabled);

            var time = DateTime.Now.TimeOfDay - startProcessingTime;
            if (time.Seconds > 5)
            {
                var message = $"Timeout Error. Teacher.aspx performance time issue. Processing Time {time} \n";
                var ex = new ChalkableException(message + timeCallBuilder);
                SendErrorToRaygun(ex, "Teacher SisUserLogin Performance Issue ", SchoolLocator.Context);
            }
        }


        private T ProcessMethodAndCallTime<T>(Func<T> action, StringBuilder timeCallBuilder, string message)
        {
            var startTime = DateTime.Now.TimeOfDay;
            var res = action();
            timeCallBuilder.AppendLine($"{message} {DateTime.Now.TimeOfDay - startTime}");
            return res;
        }
        private void ProcessMethodAndCallTime(Action action, StringBuilder timeCallBuilder, string message)
        {
            var startTime = DateTime.Now.TimeOfDay;
            action();
            timeCallBuilder.AppendLine($"{message} {DateTime.Now.TimeOfDay - startTime}");
        }

        private static readonly RaygunClient RaygunClient = new RaygunClient();
        private void SendErrorToRaygun(Exception ex, string tag, UserContext context)
        {
            var tags = new List<string> { Settings.Domain, context.Role.LoweredName, tag };
            
            RaygunClient.ApplicationVersion = CompilerHelper.Version;
            RaygunClient.User = SchoolLocator.Context.Login;
            RaygunClient.UserInfo = new RaygunIdentifierMessage(context.DistrictId + ":" + context.PersonId)
            {
                Email = context.Login,
                UUID = context.UserId.ToString()
            };
            
            RaygunClient.SendInBackground(ex, tags);
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
                personView.Email = personEmail?.EmailAddress;
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

                var alphaGradesForStandars = startupData.AlphaGradesForClassStandards[classDetails.Id];
                if (alphaGradesForStandars.Count == 0 && Context.SchoolLocalId.HasValue)
                    alphaGradesForStandars = startupData.AlphaGradesForSchoolStandards
                        .Where( x => x.SchoolId == Context.SchoolLocalId.Value)
                        .Select(x => x.AlphaGrade).ToList();

                var classAdvanceData = new
                {
                    ClassId = classDetails.Id,
                    TypesByClass = ClassAnnouncementTypeViewData.Create(typesByClasses),
                    AlphaGrades =
                        classDetails.GradingScaleRef.HasValue
                            ? startupData.AlphaGradesForClasses[classDetails.Id]
                            : allAlphaGrades,
                    AlphaGradesForStandards = alphaGradesForStandars
                };
                classesAdvancedData.Add(classAdvanceData);

                
            }
            PrepareJsonData(classesAdvancedData, ViewConstants.CLASSES_ADV_DATA);
        }

        private bool CanTeacherViewChalkable()
        {
            return Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM)
                   || Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN);
        }

        
        [AuthorizationFilter("Teacher, Student")]
        public ActionResult LearningEarnings()
        {
            var res = SchoolLocator.LeService.BuildLESingOnUrl();
            return new RedirectResult(res);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult LECredits(int? classId)
        {   
            var url = SchoolLocator.LeService.BuildLECreditsUrl(classId);
            return Json(new {url});
        }
    }

}
