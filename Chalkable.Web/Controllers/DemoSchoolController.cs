using System;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DemoSchoolController : UserController
    {
        public ActionResult LogOnIntoDemo(string rolename, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                Json(new ChalkableException(ChlkResources.ERR_DEMO_SCHOOL_INVALID_PREFIX));
            if (rolename.ToLower() == CoreRoles.DEVELOPER_ROLE.LoweredName)
            {
                var devContext = DeveloperAuthentication.GetUser();
                if (devContext != null)
                {
                    ChalkableAuthentication.SignIn(devContext, true);
                    return Redirect<HomeController>(c => c.Developer(null, null));
                }
                throw new UnassignedUserException();
            }

            var context = LogOn(false, userService => userService.DemoLogin(rolename, prefix));
            if (context == null)
                return Json(new ChalkableException(string.Format(ChlkResources.USER_NOT_FOUND_IN_DEMO_SCHOOL, rolename, prefix)));
            if (rolename.ToLower() == CoreRoles.DISTRICT_ADMIN_ROLE.LoweredName) return Redirect<HomeController>(c=>c.DistrictAdmin());
            if (rolename.ToLower() == CoreRoles.TEACHER_ROLE.LoweredName) return Redirect<HomeController>(c => c.Teacher());
            if (rolename.ToLower() == CoreRoles.STUDENT_ROLE.LoweredName) return Redirect<HomeController>(c => c.Student());
            
            throw new ChalkableSecurityException(ChlkResources.ERR_DEMO_SCHOOL_INCORRECT_ROLE);
        }


        [AuthorizationFilter("SysAdmin, AppTester")]
        public ActionResult TestApps(string prefix)
        {
            var context = LogOn(false, userService => userService.DemoLogin(CoreRoles.TEACHER_ROLE.LoweredName, prefix));
            if (context == null)
            {
                return Json(new ChalkableException(string.Format(ChlkResources.USER_NOT_FOUND_IN_DEMO_SCHOOL, CoreRoles.TEACHER_ROLE.LoweredName, prefix)));
            }
            var developer = SchoolLocator.ServiceLocatorMaster.DeveloperService.GetDeveloperByDictrict(Guid.Parse(prefix));
            if (developer != null)
            {
                var devContext = SchoolLocator.ServiceLocatorMaster.UserService.DeveloperTestLogin(developer);
                if (devContext != null)
                    DeveloperAuthentication.SignIn(devContext, true);
                return Redirect<HomeController>(c => c.Teacher());
            }
            throw new ChalkableSecurityException(ChlkResources.ERR_DEMO_SCHOOL_INCORRECT_ROLE);
        }
    }
}