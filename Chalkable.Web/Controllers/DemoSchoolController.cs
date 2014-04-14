using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Common;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DemoSchoolController : UserController
    {
        public ActionResult Index()
        {
            var roles = new List<CoreRole>
                            {
                                CoreRoles.ADMIN_GRADE_ROLE,
                                CoreRoles.TEACHER_ROLE,
                                CoreRoles.STUDENT_ROLE
                            };
            var sysLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var demoSchool = sysLocator.DistrictService.UseDemoDistrict();
            if (demoSchool == null)
                ViewData[ViewConstants.ERROR_MESSAGE_KEY] = ChlkResources.ERR_DEMO_UNAVAILABLE;
            else ViewData[ViewConstants.DEMO_PREFIX_KEY] = demoSchool.DemoPrefix;
            return View(RoleViewData.Create(roles));
        }

        public ActionResult LogOnIntoDemo(string rolename, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                Json(new ChalkableException(ChlkResources.ERR_DEMO_SCHOOL_INVALID_PREFIX));

            var context = LogOn(false, userService => userService.LoginToDemo(rolename, prefix));
            if (context == null)
                return Json(new ChalkableException(string.Format(ChlkResources.USER_NOT_FOUND_IN_DEMO_SCHOOL, rolename, prefix)));
            if (rolename.ToLower() == CoreRoles.ADMIN_GRADE_ROLE.LoweredName) return Redirect<HomeController>(c => c.Admin(false));
            if (rolename.ToLower() == CoreRoles.TEACHER_ROLE.LoweredName) return Redirect<HomeController>(c => c.Teacher(false));
            if (rolename.ToLower() == CoreRoles.STUDENT_ROLE.LoweredName) return Redirect<HomeController>(c => c.Student(false));
            if (rolename.ToLower() == CoreRoles.DEVELOPER_ROLE.LoweredName) return Redirect<HomeController>(c => c.Developer(null));
            throw new ChalkableSecurityException(ChlkResources.ERR_DEMO_SCHOOL_INCORRECT_ROLE);
        }
    }
}