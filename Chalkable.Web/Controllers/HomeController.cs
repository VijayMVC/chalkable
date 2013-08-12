using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;
using Chalkable.Web.Models.SchoolsViewData;
using Chalkable.Web.Tools;

namespace Chalkable.Web.Controllers
{
    public class HomeController : ChalkableController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LogOn(string userName, string password, bool remember)
        {
            var context = LogOn(remember, us => us.Login(userName, password));
            if(context != null)
                return Json(new { Success = true, data = new {Role = context.Role.LoweredName} }, JsonRequestBehavior.AllowGet);
            return Json(new { Success = false, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            var userName = ControllerContext.HttpContext.User.Identity.Name;
            ChalkableAuthentication.SignOut();
            return Json(new { success = true, data = new {success = true} }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Confirm(string key)
        {
            return Confirm(key, AfterConfirmAction);
        }
        private ActionResult AfterConfirmAction(UserContext context)
        {
            //TODO: create default Announcement for teacher
            SchoolLocator.PersonService.ActivatePerson(context.UserId);
            //TODO: mix panel 
            if (context.Role == CoreRoles.SUPER_ADMIN_ROLE)
                return Redirect<HomeController>(x => x.SysAdmin());
            if (context.Role == CoreRoles.TEACHER_ROLE)
                return Redirect<HomeController>(x => x.Teacher(true));
            return Redirect<HomeController>(c => c.Index());
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult SysAdmin()
        {
            var sysUser = MasterLocator.UserService.GetById(Context.UserId);
            PrepareJsonData(SysAdminViewData.Create(sysUser), CURRENT_PERSON);
            return View();
        }


        [AuthorizationFilter("Developer")]
        public ActionResult Developer(Guid? currentApplicationId)
        {
            var prefDemoSchool = MasterLocator.SchoolService.GetById(Context.SchoolId.Value).DemoPrefix;
            var developer = MasterLocator.DeveloperService.GetDeveloperById(MasterLocator.Context.UserId);
            ViewData[IS_DEV] = true;
            PrepareJsonData(DeveloperViewData.Create(developer), CURRENT_PERSON);
            var applications = MasterLocator.ApplicationService.GetApplications(0, int.MaxValue, false);
            if (applications.Count == 0)
            {
                ViewData[REDIRECT_URL_KEY] = DEV_APP_INFO_URL;
            }
            ViewData[NEEDS_TOUR] = false;
            PrepareCommonViewData(prefDemoSchool);
            PrepareJsonData(BaseApplicationViewData.Create(applications), APPLICATIONS);
            if (applications.Count > 0)
            {
                var app = currentApplicationId.HasValue ? applications.First(x => x.Id == currentApplicationId) : applications.Last();
                app = MasterLocator.ApplicationService.GetApplicationById(app.Id);
                var res = ApplicationController.PrepareAppInfo(MasterLocator, app, true, true);
                PrepareJsonData(res, DEFAULT_APPLICATION, 6);
            }
            //TODO: mix panel
            return View();
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Teacher(bool? redirectToSetup)
        {
            if (redirectToSetup.HasValue && redirectToSetup.Value)
                ViewData[REDIRECT_URL_KEY] = string.Format(SETUP_REDIRECT_URL_FORMAT, Context.UserId);

            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            PrepareTeacherJsonData(mp, false);
            return View();
        }

        
        private const string CLASSES = "Classes";
        private const string CLASSES_ADV_DATA = "ClassesAdvancedData";
        private const string CURRENT_PERSON = "CurrentPerson";
        private const string IS_DEV = "IsDeveloper";
        private const string MARKING_PERIOD = "MarkingPeriod";
        private const string NEXT_MARKING_PERIOD = "NextMarkingPeriod";
        private const string ATTENDANCE_REASONS = "AttendanceReasons";
        private const string SCHOOL = "School";
        private const string FINALIZED_CLASSES_IDS = "FinalizedClassesIds";
        private const string DEFAULT_APPLICATION = "DefaultApplication";
        private const string APPLICATIONS = "Applications";

        private const string STUDENT_ROLE = "StudentRole";
        private const string TEACHER_ROLE = "TeacherRole";
        private const string ADMIN_GRADE_ROLE = "AdminGradeRole";
        private const string ADMIN_EDIT_ROLE = "AdminEditRole";
        private const string ADMIN_VIEW_ROLE = "AdminViewRole";

        private const string CROCODOC_API_URL = "CrocodocApiUrl";
        private const string NEEDS_TOUR = "NeedsTour";
        private const string UNSHOWN_NOTIFICATIONS_COUNT = "UnshownNotificationsCount";
        
        private const string VERSION = "Version";
        private const string SETUP_REDIRECT_URL_FORMAT = "setup/hello/{0}";
        private const string CURR_SCHOOL_YEAR_ID = "CurrentSchoolYearId";
        private const string REDIRECT_URL_KEY = "RedirectUrl";
        private const string INVITE_URL = "setup/invite";
        private const string ROLENAME_KEY = "Rolename";
        private const string SCHEDULE_URL = "schools/schedule/{0}";
        private const string ATTENDANCE_CLASS_LIST_URL = "attendances/class-list/{0}";
        private const string DEV_APP_INFO_URL = "applications/devAppInfo/";


        private void PrepareCommonViewData(string prefixDemoSchool = null, MarkingPeriod markingPeriod = null)
        {
            //TODO: render data for demo school 
            if (Context.SchoolId.HasValue)
            {
                var school = MasterLocator.SchoolService.GetById(Context.SchoolId.Value);
                PrepareJsonData(ShortSchoolViewData.Create(school), SCHOOL);
                if (!string.IsNullOrEmpty(school.DemoPrefix) && !string.IsNullOrEmpty(prefixDemoSchool))
                {
                    ViewData[STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
                    ViewData[TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
                    ViewData[ADMIN_GRADE_ROLE] = CoreRoles.ADMIN_GRADE_ROLE.Name;
                    ViewData[ADMIN_EDIT_ROLE] = CoreRoles.ADMIN_EDIT_ROLE.Name;
                    ViewData[ADMIN_VIEW_ROLE] = CoreRoles.ADMIN_VIEW_ROLE.Name;
                }
            }
            //ViewData[NEEDS_TOUR] = needsTour;
            ViewData[CURR_SCHOOL_YEAR_ID] = GetCurrentSchoolYearId();
            ViewData[VERSION] = CompilerHelper.Version;
            ViewData[CROCODOC_API_URL] = PreferenceService.Get(Preference.CROCODOC_URL).Value;

            ViewData[UNSHOWN_NOTIFICATIONS_COUNT] = SchoolLocator.NotificationService.GetUnshownNotifications().Count;
            if (markingPeriod != null && SchoolLocator.Context.SchoolId.HasValue)
            {
                PrepareJsonData(MarkingPeriodViewData.Create(markingPeriod), MARKING_PERIOD);
                var nextmp = SchoolLocator.MarkingPeriodService.GetNextMarkingPeriodInYear(markingPeriod.Id);
                if (nextmp != null)
                    PrepareJsonData(MarkingPeriodViewData.Create(nextmp), NEXT_MARKING_PERIOD);
            }
        }
        
        private void PrepareTeacherJsonData(MarkingPeriod mp, bool getAllAnnouncementTypes)
        {
            var person = SchoolLocator.PersonService.GetPerson(SchoolLocator.Context.UserId);
            var personView = PersonViewData.Create(person);
            personView.DisplayName = person.ShortSalutationName;
            PrepareJsonData(personView, CURRENT_PERSON);

            var finalizedClasses = SchoolLocator.FinalGradeService.GetFinalizedClasses(mp.Id);
            PrepareJsonData(finalizedClasses.Select(x => x.Id), FINALIZED_CLASSES_IDS);

            var classes = SchoolLocator.ClassService.GetClasses(mp.SchoolYearRef, null, SchoolLocator.Context.UserId);
            var now = SchoolLocator.Context.NowSchoolTime;
            if (classes.Count > 0)
            {
                MarkingPeriod currentMp = mp;
                if(mp.StartDate > now || mp.EndDate < now)
                    currentMp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(now);
                if (currentMp != null)
                {
                    var cp = SchoolLocator.ClassPeriodService.GetNearestClassPeriod(null, now);
                    if (cp != null)
                    {
                        var minutes = (int) (now - now.Date).TotalMinutes;
                        if (cp.Period.StartTime - minutes <= 5)
                        {
                            SchoolLocator.CalendarDateService.GetCalendarDateByDate(now);
                            var attQuery = new ClassAttendanceQuery
                                {
                                    MarkingPeriodId = currentMp.Id,
                                    ClassId = cp.ClassRef,
                                    FromTime = cp.Period.StartTime,
                                    ToTime = cp.Period.EndTime,
                                    FromDate = now.Date,
                                    ToDate = now.Date
                                };
                            var attendances = SchoolLocator.AttendanceService.GetClassAttendanceDetails(attQuery);
                            //check is it tour now or demo school
                            if (attendances.Any(x => x.Type == AttendanceTypeEnum.NotAssigned))
                            {
                                ViewData[REDIRECT_URL_KEY] = string.Format(ATTENDANCE_CLASS_LIST_URL, cp.ClassRef);
                            }
                        }
                    }
                }
            } 
            var executionResult = classes.Select(ClassViewData.Create).ToList();
            PrepareJsonData(executionResult, CLASSES);
            PrepareClassesAdvancedData(classes, mp, getAllAnnouncementTypes);
            PrepareCommonViewData(null, mp);
            PrepareJsonData(AttendanceReasonViewData.Create(SchoolLocator.AttendanceReasonService.List()), ATTENDANCE_REASONS);
        }

        private void PrepareClassesAdvancedData(IEnumerable<ClassDetails> classDetailses, MarkingPeriod mp, bool getAllAnnouncementTypes)
        {
            var classesAdvancedData = new List<object>();
            classDetailses = classDetailses.Where(x => x.MarkingPeriodClasses.Any(y => y.MarkingPeriodRef == mp.Id));
            var classesMaskDic = ClassController.BuildClassesUsageMask(SchoolLocator, mp.Id, SchoolLocator.Context.SchoolTimeZoneId);
            var baseAnnTypes = SchoolLocator.AnnouncementTypeService.GetAnnouncementTypes(null);
            foreach (var classDetails in classDetailses)
            {
                Guid classId = classDetails.Id;
                var typesByClasses = getAllAnnouncementTypes ? baseAnnTypes
                                    : AnnouncementTypeController.GetTypesByClass(SchoolLocator, mp.Id, classId);
                classesAdvancedData.Add(new
                {
                    ClassId = classId,
                    Mask = classesMaskDic.ContainsKey(classId) ? classesMaskDic[classId] : new List<int>(),
                    TypesByClass = AnnouncementTypeViewData.Create(typesByClasses)
                });
            }
            PrepareJsonData(classesAdvancedData, CLASSES_ADV_DATA);
        }

        //TODO: test only. don't forget to remove :)
        public ActionResult Create(string userName, string password)
        {
            ServiceLocatorFactory.CreateMasterSysAdmin().UserService.CreateSysAdmin(userName, password);
            return Json(new { Success = true, UserName = userName }, JsonRequestBehavior.AllowGet);
        }        
    }
}
