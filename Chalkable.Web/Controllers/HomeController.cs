using System;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.Web.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LogOn(string userName, string password, bool remember)
        {
            var b = DateTime.Now;
            var provider = Membership.Provider;
            if (provider.ValidateUser(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, remember);
                var dt = (DateTime.Now - b).TotalSeconds;
                return Json(new { Success = true, UserName = userName, ProcessTime = dt, CallTime = b }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            var userName = ControllerContext.HttpContext.User.Identity.Name;
            FormsAuthentication.SignOut();
            return Json(new { Success = true, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(string userName, string password)
        {
            ServiceLocatorFactory.CreateMasterSysAdmin().UserService.CreateSysAdmin(userName, password);
            return Json(new { Success = true, UserName = userName }, JsonRequestBehavior.AllowGet);
        }
    }
}
