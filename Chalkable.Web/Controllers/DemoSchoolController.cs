using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DemoSchoolController : ChalkableController
    {

        private const string errorMessageKey = "ErrorMessage";
        private const string demoPrefixKey = "DemoPrefix";

        public ActionResult Index()
        {
            var roles = new List<CoreRole>
                            {
                                CoreRoles.ADMIN_GRADE_ROLE,
                                CoreRoles.ADMIN_EDIT_ROLE,
                                CoreRoles.ADMIN_VIEW_ROLE,
                                CoreRoles.TEACHER_ROLE,
                                CoreRoles.STUDENT_ROLE
                            };
            var sysLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var demoSchool = sysLocator.SchoolService.UseDemoSchool();
            if (demoSchool == null)
                ViewData[errorMessageKey] = ChlkResources.ERR_DEMO_UNAVAILABLE;
            else ViewData[demoPrefixKey] = demoSchool.DemoPrefix;
            return View(RoleViewData.Create(roles));
        }

        [HttpPost]
        public ActionResult LogOn(string rolename, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                Json(new ChalkableException(ChlkResources.ERR_DEMO_SCHOOL_INVALID_PREFIX));

            var context = LogOn(false, userService => userService.LoginToDemo(rolename, prefix));
            if (context == null)
                return Json(new ChalkableException("Invalid user name"));
            return Json(new {Role = context.Role.LoweredName});
        }
    }
}