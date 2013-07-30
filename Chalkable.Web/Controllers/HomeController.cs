using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Web;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Authentication;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;
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
            var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var context = serviceLocator.UserService.Login(userName, password);
            if (context != null)
            {
                ChalkableAuthentication.SignIn(context, remember);
                return Json(new { Success = true, data = new {Role = context.Role.LoweredName} }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            var userName = ControllerContext.HttpContext.User.Identity.Name;
            ChalkableAuthentication.SignOut();
            return Json(new { Success = true, UserName = userName }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult SysAdmin()
        {
            ViewData["FullName"] = ControllerContext.HttpContext.User.Identity.Name;
            return View();
        }

        public ActionResult Teacher()
        {
            var mp = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            PrepareCommonViewData(mp);
            PrepareTeacherJsonData(mp, false);
            return View();
        }

        private const string CLASSES_STR = "Classes";
        private const string CLASSES_ADV_DATA = "ClassesAdvancedData";
        private const string SCHOOL_PERSON_DATA = "SchoolPersonData";
        private const string MARKING_PERIOD_DATA = "MarkingPeriodData";
        private const string NEXT_MARKING_PERIOD_DATA = "NextMarkingPeriodData";

        private const string STUDENT_ROLE = "StudentRole";
        private const string TEACHER_ROLE = "TeacherRole";
        private const string ADMIN_GRADE_ROLE = "AdminGradeRole";
        private const string ADMIN_EDIT_ROLE = "AdminEditRole";
        private const string ADMIN_VIEW_ROLE = "AdminViewRole";

        private const string SCHOOL_ID = "SchoolId";
        private const string SCHOOL_NAME = "SchoolName";
        private const string UNKNOWN_SCHOOL_NAME = "no school name";

        private const string CROCODOC_API_URL = "CrocodocApiUrl";

        private const string UNSHOWN_NOTIFICATIONS_COUNT = "UnshownNotificationsCount";
        


        private const string VERSION = "Version";
        private const string CONFIRM_REDIRECT_URL_FORMAT = "setup/hello/{0}";
        private const string CURR_SCHOOL_YEAR_ID = "CurrentSchoolYearId";
        private const string LAST_SCHOOL_YEAR_ID = "LastSchoolYearId";
        private const string NEXT_SCHOOL_YEAR_ID = "NextSchoolYearId";
        private const string REDIRECT_URL_KEY = "RedirectUrl";
        private const string INVITE_URL = "setup/invite";
        private const string ROLENAME_KEY = "Rolename";
        private const string SCHOOL_STATUS = "SchoolStatus";
        private const string SCHEDULE_URL = "schools/schedule/{0}";


        private void PrepareCommonViewData(MarkingPeriod markingPeriod = null)
        {
            var person = SchoolLocator.PersonService.GetPerson(SchoolLocator.Context.UserId);
            PrepareJsonData(PersonViewData.Create(person), SCHOOL_PERSON_DATA);
            
            //TODO: render data for demo school 
            //if (!string.IsNullOrEmpty(prefDemoSchool))
            //{
            //    ViewData[STUDENT_ROLE] = CoreRoles.STUDENT_ROLE.Name;
            //    ViewData[TEACHER_ROLE] = CoreRoles.TEACHER_ROLE.Name;
            //    ViewData[ADMIN_GRADE_ROLE] = CoreRoles.ADMIN_GRADE_ROLE.Name;
            //    ViewData[ADMIN_EDIT_ROLE] = CoreRoles.ADMIN_EDIT_ROLE.Name;
            //    ViewData[ADMIN_VIEW_ROLE] = CoreRoles.ADMIN_VIEW_ROLE.Name;
            //    //ViewData[PREFIX] = prefDemoSchool;

            //}
            //if (schoolId != null)
            //{
            //    var currentSchool = ServiceLocator.SchoolService.GetById(schoolId.Value);
            //    if (currentSchool.DeveloperInfoRef.HasValue)
            //    {
            //        ViewData[DEVELOPER_ID] = currentSchool.DeveloperInfoRef.Value.ToString();
            //        PreparingJsonData(DeveloperInfoViewData.Create(currentSchool.DeveloperInfo), DEVELOPER_INFO);
            //    }
            //}

            //ViewData[NEEDS_TOUR] = needsTour;
            ViewData[SCHOOL_ID] = SchoolLocator.Context.SchoolId;
            ViewData[SCHOOL_NAME] = SchoolLocator.Context.SchoolName ?? UNKNOWN_SCHOOL_NAME;
            ViewData[CURR_SCHOOL_YEAR_ID] = GetCurrentSchoolYearId();

            ViewData[VERSION] = CompilerHelper.Version;
            ViewData[CROCODOC_API_URL] = PreferenceService.Get(Preference.CROCODOC_URL).Value;

            ViewData[UNSHOWN_NOTIFICATIONS_COUNT] = SchoolLocator.NotificationService.GetUnshownNotifications().Count;
            if (markingPeriod != null && SchoolLocator.Context.SchoolId.HasValue)
            {
                PrepareJsonData(MarkingPeriodViewData.Create(markingPeriod), MARKING_PERIOD_DATA);
                var nextmp = SchoolLocator.MarkingPeriodService.GetNextMarkingPeriodInYear(markingPeriod.Id);
                if (nextmp != null)
                {
                    PrepareJsonData(MarkingPeriodViewData.Create(nextmp), NEXT_MARKING_PERIOD_DATA);
                }
            }

        }


        private void PrepareTeacherJsonData(MarkingPeriod mp, bool getAllAnnouncementTypes)
        {
            var classes = SchoolLocator.ClassService.GetClasses(null, mp.Id, null);
            var executionResult = classes.Select(ClassViewData.Create).ToList();
            PrepareJsonData(executionResult, CLASSES_STR);
            PrepareClassesAdvancedData(classes, mp, getAllAnnouncementTypes);

        }

        private void PrepareClassesAdvancedData(IList<ClassDetails> classDetailses, MarkingPeriod mp, bool getAllAnnouncementTypes)
        {
            var classesAdvancedData = new List<object>();
            foreach (var classDetails in classDetailses)
            {
                Guid classId = classDetails.Id;
                var mask = ClassController.BuildClassUsageMask(SchoolLocator, classId, mp.Id, SchoolLocator.Context.SchoolTimeZoneId);
                var typesByClasses = getAllAnnouncementTypes ? SchoolLocator.AnnouncementTypeService.GetAnnouncementTypes(null)
                                                 : AnnouncementTypeController.GetTypesByClasses(SchoolLocator, mp.Id, classId);
                classesAdvancedData.Add(new
                {
                    ClassId = classId,
                    Mask = mask,
                    TypesByClass = AnnouncementTypeViewData.Create(typesByClasses)
                });
            }
            PrepareJsonData(classesAdvancedData, CLASSES_ADV_DATA);
        }

        private void PrepareJsonData(object data, string name, int maxDepth = 4)
        {
            var jsonResponse = new ChalkableJsonResponce(data);
            var serializer = new MagicJsonSerializer(false) {MaxDepth = maxDepth};
            var res = serializer.Serialize(jsonResponse);
            ViewData[name] = res;
        }



        //TODO: test only. don't forget to remove :)
        public ActionResult Create(string userName, string password)
        {
            ServiceLocatorFactory.CreateMasterSysAdmin().UserService.CreateSysAdmin(userName, password);
            return Json(new { Success = true, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        
    }
}
