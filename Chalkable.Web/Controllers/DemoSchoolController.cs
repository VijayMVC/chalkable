using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;
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
            ViewData[ViewConstants.DEMO_PREFIX_KEY] = Guid.NewGuid().ToString();
            return View(RoleViewData.Create(roles));
        }

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
            if (rolename.ToLower() == CoreRoles.ADMIN_GRADE_ROLE.LoweredName) return Redirect<HomeController>(c => c.Admin());
            if (rolename.ToLower() == CoreRoles.TEACHER_ROLE.LoweredName) return Redirect<HomeController>(c => c.Teacher());
            if (rolename.ToLower() == CoreRoles.STUDENT_ROLE.LoweredName) return Redirect<HomeController>(c => c.Student());
            
            throw new ChalkableSecurityException(ChlkResources.ERR_DEMO_SCHOOL_INCORRECT_ROLE);
        }


        [AuthorizationFilter("SysAdmin")]
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