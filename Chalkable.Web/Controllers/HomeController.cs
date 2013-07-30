using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Authentication;

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
            ViewData["FullName"] = ControllerContext.HttpContext.User.Identity.Name;
            return View();
        }



        //TODO: test only. don't forget to remove :)
        public ActionResult Create(string userName, string password)
        {
            ServiceLocatorFactory.CreateMasterSysAdmin().UserService.CreateSysAdmin(userName, password);
            return Json(new { Success = true, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        
    }
}
