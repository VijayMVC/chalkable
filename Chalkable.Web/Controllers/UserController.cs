using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class UserController : ChalkableController
    {
        public ActionResult ChangePassword(string oldPassword, string newPassword, string newPasswordConfirmation)
        {
            var login = Context.Login;
            if (MasterLocator.UserService.Login(login, oldPassword) != null)
            {
                if (newPassword == newPasswordConfirmation)
                {
                    MasterLocator.UserService.ChangePassword(login, newPassword);
                    return Json(true);
                }
            }
            return Json(false);
        }
    }
}