using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.MixPanel;
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

        [AuthorizationFilter("SysAdmin")]
        public ActionResult SysAdmin()
        {
            var sysUser = MasterLocator.UserService.GetById(Context.UserId);
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            ViewData[ViewConstants.DEMO_AZURE_PICTURE_URL] = PictureService.GeDemoPicturesRelativeAddress();
            PrepareJsonData(SysAdminViewData.Create(sysUser), ViewConstants.CURRENT_PERSON);
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MixPanelService.IdentifySysAdmin(sysUser.Login, "", "", null, ip);
            return View();
        }

        [AuthorizationFilter("Developer")]
        public ActionResult Developer(Guid? currentApplicationId)
        {
            var developer = MasterLocator.DeveloperService.GetDeveloperById(MasterLocator.Context.UserId);
            PrepareJsonData(DeveloperViewData.Create(developer), ViewConstants.CURRENT_PERSON);
            var applications = MasterLocator.ApplicationService.GetApplications(0, int.MaxValue, false);
            if (applications.Count == 0)
            {
                ViewData[ViewConstants.REDIRECT_URL_KEY] = UrlsConstants.DEV_APP_INFO_URL;
            }
            ViewData[ViewConstants.AZURE_PICTURE_URL] = PictureService.GetPicturesRelativeAddress();
            ViewData[ViewConstants.DEMO_AZURE_PICTURE_URL] = PictureService.GeDemoPicturesRelativeAddress();
            var serverTime = Context.NowSchoolTime.ToString("yyyy/MM/dd hh:mm:ss");
            ViewData[ViewConstants.SERVER_TIME] = serverTime;
            ViewData[ViewConstants.SCHOOL_YEAR_SERVER_TIME] = Context.NowSchoolYearTime.ToString("yyyy/MM/dd hh:mm:ss");
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
            MixPanelService.IdentifyDeveloper(developer.Email, developer.DisplayName, DateTime.UtcNow, "UTC", ip);
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
            if (Context.DistrictId.HasValue && Context.SchoolId.HasValue)
            {
                var district = MasterLocator.DistrictService.GetByIdOrNull(Context.DistrictId.Value);
                var school = MasterLocator.SchoolService.GetById(Context.SchoolId.Value);
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
            var serverTime = Context.NowSchoolTime.ToString("yyyy/MM/dd hh:mm:ss");
            ViewData[ViewConstants.SERVER_TIME] = serverTime;
            ViewData[ViewConstants.SCHOOL_YEAR_SERVER_TIME] = Context.NowSchoolYearTime.ToString("yyyy/MM/dd hh:mm:ss");
            PrepareJsonData(Context.Claims, ViewConstants.USER_CLAIMS);

            //PrepareJsonData(AttendanceReasonViewData.Create(SchoolLocator.AttendanceReasonService.List()), ViewConstants.ATTENDANCE_REASONS);

            ViewData[ViewConstants.UNSHOWN_NOTIFICATIONS_COUNT] = SchoolLocator.NotificationService.GetUnshownNotifications().Count;
            if (markingPeriod != null && SchoolLocator.Context.SchoolId.HasValue)
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
        }
        
        private void PrepareStudentJsonData()
        {
            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            var person = SchoolLocator.PersonService.GetPerson(Context.UserLocalId.Value);
            var personView = PersonViewData.Create(person);
            personView.DisplayName = person.FullName;
            if (!person.FirstLoginDate.HasValue)
            {
                ViewData[ViewConstants.REDIRECT_URL_KEY] = string.Format(UrlsConstants.SETUP_URL_FORMAT, Context.UserLocalId);
                var personEmail = SchoolLocator.PersonEmailService.GetPersonEmail(person.Id);
                personView.Email = personEmail == null ? null : personEmail.EmailAddress;
            }
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);
            
            //todo: move this logic to getClass stored procedure later
            var classPersons = SchoolLocator.ClassService.GetClassPersons(person.Id, true);
            var classes = SchoolLocator.ClassService.GetClasses(mp.SchoolYearRef, null, Context.UserLocalId)
                                       .Where(c => classPersons.Any(cp => cp.ClassRef == c.Id)).ToList();
            
            PrepareJsonData(ClassViewData.Create(classes), ViewConstants.CLASSES);
            PrepareCommonViewData(mp);

            var ip = RequestHelpers.GetClientIpAddress(Request);
            MixPanelService.IdentifyStudent(person.Email, person.FirstName, person.LastName, Context.SchoolId.ToString(), "", person.FirstLoginDate, Context.SchoolTimeZoneId, ip);
        }           

        private void PrepareAdminJsonData()
        {
            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            var person = SchoolLocator.PersonService.GetPerson(Context.UserLocalId.Value);
            var personView = PersonViewData.Create(person);
            personView.DisplayName = person.ShortSalutationName;
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);
            var gradeLevels = SchoolLocator.GradeLevelService.GetGradeLevels();
            PrepareJsonData(GradeLevelViewData.Create(gradeLevels), ViewConstants.GRADE_LEVELS);
            PrepareJsonData(AttendanceReasonDetailsViewData.Create(SchoolLocator.AttendanceReasonService.List()), ViewConstants.ATTENDANCE_REASONS);
            PrepareCommonViewData(mp);
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MixPanelService.IdentifyAdmin(person.Email, person.FirstName, person.LastName, Context.SchoolId.ToString(), person.FirstLoginDate, Context.SchoolTimeZoneId, "Admin", ip);
        }

        private void PrepareTeacherJsonData(MarkingPeriod mp)
        {
            if (!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();

            PrepareCommonViewData(mp);

            var person = SchoolLocator.PersonService.GetPerson(Context.UserLocalId.Value);
            var personView = PersonViewData.Create(person);
            personView.DisplayName = person.ShortSalutationName;           
            if (!person.FirstLoginDate.HasValue)
            {
                ViewData[ViewConstants.REDIRECT_URL_KEY] = string.Format(UrlsConstants.SETUP_URL_FORMAT, Context.UserLocalId);
                var personEmail = SchoolLocator.PersonEmailService.GetPersonEmail(person.Id);
                personView.Email = personEmail == null ? null : personEmail.EmailAddress;
            }
            PrepareJsonData(personView, ViewConstants.CURRENT_PERSON);


            if (!CanTeacherViewChalkable()) return;
            
            var classes = SchoolLocator.ClassService.GetClasses(mp.SchoolYearRef, null, Context.UserLocalId.Value);
            var gradeLevels = classes.Select(x => x.GradeLevel.Name).Distinct().ToList();
            var classNames = classes.Select(x => x.Name).ToList();

            var schoolOption = SchoolLocator.SchoolService.GetSchoolOption();
            PrepareJsonData(SchoolOptionViewData.Create(schoolOption), ViewConstants.SCHOOL_OPTIONS);
            var executionResult = classes.Select(ClassViewData.Create).ToList();
            PrepareJsonData(executionResult, ViewConstants.CLASSES);
            PrepareClassesAdvancedData(classes, mp);
            PrepareJsonData(GradingCommentViewData.Create(SchoolLocator.GradingCommentService.GetGradingComments()), ViewConstants.GRADING_COMMMENTS);
            PrepareJsonData(AttendanceReasonDetailsViewData.Create(SchoolLocator.AttendanceReasonService.List()), ViewConstants.ATTENDANCE_REASONS);
            var ip = RequestHelpers.GetClientIpAddress(Request);
            MixPanelService.IdentifyTeacher(Context.Login, person.FirstName, person.LastName, Context.SchoolId.ToString(), 
                gradeLevels, classNames, person.FirstLoginDate, Context.SchoolTimeZoneId, ip);
        }
        
        private void PrepareClassesAdvancedData(IEnumerable<ClassDetails> classDetailses, MarkingPeriod mp)
        {
            var classesAdvancedData = new List<object>();
            classDetailses = classDetailses.Where(x => x.MarkingPeriodClasses.Any(y => y.MarkingPeriodRef == mp.Id)).ToList();
            var classesMaskDic = ClassController.BuildClassesUsageMask(SchoolLocator, mp.Id, SchoolLocator.Context.SchoolTimeZoneId);
            var allAlphaGrades = SchoolLocator.AlphaGradeService.GetAlphaGrades();
            var classAnnouncementTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classDetailses.Select(x => x.Id).ToList());
            foreach (var classDetails in classDetailses)
            {

                int classId = classDetails.Id;
                var typesByClasses = classAnnouncementTypes.Where(x => x.ClassRef == classId).ToList();
                classesAdvancedData.Add(new
                {
                    ClassId = classId,
                    Mask = classesMaskDic.ContainsKey(classId) ? classesMaskDic[classId] : new List<int>(),
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
