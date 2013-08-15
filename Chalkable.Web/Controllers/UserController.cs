using System.Web.Mvc;
using Chalkable.Common.Exceptions;
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
                return Json(new ChalkableException("new password and confirmation dont't match"));
            }
            return Json(new ChalkableException("old password is incorrect"));
        }
    }
}