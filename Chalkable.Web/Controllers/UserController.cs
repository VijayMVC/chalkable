using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;
using Chalkable.Web.Common;
using Chalkable.Web.Tools;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class UserController : ChalkableController
    {
        [IgnoreTimeOut]
        public ActionResult SisLogIn(string token, Guid districtId, DateTime? tokenExpiresTime, int? acadSessionId, string returnUrl)
        {
            var expiresTime = tokenExpiresTime ?? DateTime.UtcNow.AddDays(2);
            var context = LogOn(false, us => us.SisLogIn(districtId, token, expiresTime, acadSessionId, returnUrl));
            if (context != null)
            {
                MasterLocator.UserTrackingService.LoggedIn(context.Login);
                return RedirectToHome(context.Role); 
            }
            return Redirect<HomeController>(x => x.Index());
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student"), IgnoreTimeOut]
        public ActionResult RedirectToINow()
        {
            if (!Context.DistrictId.HasValue)
                throw new Exception("District id should be defined for redirect to SIS");
            var schoolYearId = GetCurrentSchoolYearId();
            var url = UrlTools.UrlCombine(Context.SisRedirectUrl, $"TokenLogin.aspx?Token={Context.SisToken}&AcadSessionId={schoolYearId}");
            return Redirect(url);
        }

        [IgnoreTimeOut]
        public ActionResult LogOn(string userName, string password, bool remember)
        {
            if(string.IsNullOrEmpty(userName))
                return Json(new ChalkableException("Enter your email"));

            string error = null;
            var context = LogOn(remember, us => us.Login(userName, password, out error));
            if (context == null)
                return Json(new ChalkableException(error ?? ""));
            
            MasterLocator.UserTrackingService.LoggedIn(context.Login);

            if (Request.IsAjaxRequest())
                return Json(new { Role = context.Role.LoweredName });

            var role = context.Role.LoweredName;
            if (role == "admingrade" || role == "adminview" || role == "adminedit")
                role = "admin";

            return Redirect("/Home/" + role + ".aspx");
        }

        [AuthorizationFilter("Teacher"), IgnoreTimeOut]
        public ActionResult SwitchToDistrictAdmin()
        {
            SwitchToRole(CoreRoles.DISTRICT_ADMIN_ROLE);
            return Redirect<HomeController>(x => x.DistrictAdmin());
        }

        [AuthorizationFilter("DistrictAdmin"), IgnoreTimeOut]
        public ActionResult SwitchToTeacher()
        {
            SwitchToRole(CoreRoles.TEACHER_ROLE);
            return Redirect<HomeController>(x => x.Teacher());
        }

        [IgnoreTimeOut]
        private void SwitchToRole(CoreRole role)
        {
            var context = MasterLocator.UserService.SwitchToRole(role);
            ChalkableAuthentication.SignIn(context, false);
        }

        [IgnoreTimeOut]
        public ActionResult LogOut()
        {
            ChalkableAuthentication.SignOut();
            DeveloperAuthentication.SignOut();
            return Json(new { success = true, data = new { success = true } }, JsonRequestBehavior.AllowGet);
        }

        [IgnoreTimeOut]
        public ActionResult LogOutWithRedirect()
        {
            ChalkableAuthentication.SignOut();
            DeveloperAuthentication.SignOut();
            return Redirect<HomeController>(x => x.Index());
        }

        [IgnoreTimeOut]
        public ActionResult Confirm(string key)
        {
            return Confirm(key, AfterConfirmAction);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student"), IgnoreTimeOut]
        public ActionResult ActivateUser(string newUserEmail)
        {
            if (!Context.PersonId.HasValue)
                return Json(new UnassignedUserException());
            string error;
            SchoolLocator.PersonService.EditEmailForCurrentUser(Context.PersonId.Value, newUserEmail, out error);
            if (!string.IsNullOrEmpty(error))
                return Json(new ChalkableException(error));
            SchoolLocator.PersonService.ActivatePerson(Context.PersonId.Value);
            return Json(true);            
        }
        
        public ActionResult ChangePassword(string oldPassword, string newPassword, string newPasswordConfirmation, bool resetPassword)
        {
            if (!PasswordTools.IsSecurePassword(newPassword))
                return Json(new ChalkableException("new password is not secure enough"));

            var login = Context.Login;
            if (!resetPassword && MasterLocator.UserService.Login(login, oldPassword) == null)
                return Json(new ChalkableException("old password is incorrect"));

            if (newPassword != newPasswordConfirmation)
                return Json(new ChalkableException("new password and confirmation doesn't match"));

            MasterLocator.UserService.ChangePassword(login, newPassword);
            MasterLocator.UserTrackingService.ChangedPassword(Context.Login);
            return Json(true);
        }

        [IgnoreTimeOut]
        public ActionResult ResetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(false);

            var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            /*try
            {*/
                return Json(serviceLocator.UserService.ResetPassword(email));
            /*}
            catch (ChalkableException e)
            {
                return Json(e);
            }*/
        }

        [IgnoreTimeOut]
        private ActionResult AfterConfirmAction(UserContext context)
        {
            if (context.PersonId.HasValue)
                SchoolLocator.PersonService.ActivatePerson(context.PersonId.Value);
            //TODO: mix panel 
            if (context.Role == CoreRoles.SUPER_ADMIN_ROLE)
                return Redirect<HomeController>(x => x.SysAdmin(true));
            if (context.Role == CoreRoles.TEACHER_ROLE)
                return Redirect<HomeController>(x => x.Teacher());
            if (context.Role == CoreRoles.DEVELOPER_ROLE)
                return Redirect<HomeController>(x => x.Developer(null, true));
            if (context.Role == CoreRoles.APP_TESTER_ROLE)
                return Redirect<HomeController>(x => x.AppTester(true));
            return Redirect<HomeController>(c => c.Index());
        }

        [IgnoreTimeOut]
        protected ActionResult Confirm(string key, Func<UserContext, ActionResult> redirectAction)
        {
            var context = LogOn(false, us => us.Login(key));
            if (context != null)
            {
                InitServiceLocators(context);
                MasterLocator.UserTrackingService.UserLoggedInForFirstTime(context.Login, "", "", Context.DistrictId.ToString(), 
                        Context.NowSchoolTime, Context.DistrictTimeZone ?? "UTC", Context.Role.Name, Context.SCEnabled);
                return redirectAction(context);
            }
            return Redirect<HomeController>(c => c.Index());
        }

        [IgnoreTimeOut]
        protected UserContext LogOn(bool remember, Func<IUserService, UserContext> logOnAction)
        {
            var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            return LogOn(remember, serviceLocator.UserService, logOnAction);
        }

        [IgnoreTimeOut]
        protected UserContext LogOn(bool remember, IUserService userService, Func<IUserService, UserContext> logOnAction)
        {
            var context = logOnAction(userService);
            if (context != null)
            {
                ChalkableAuthentication.SignIn(context, false);
                if (context.DeveloperId.HasValue && !DemoUserService.IsDemoUser(context))
                    DeveloperAuthentication.SignIn(context, remember);
            }
            return context;
        }

        [IgnoreTimeOut]
        public ActionResult GetAccessToken(string login, string password, string clientId, string clientSecret, string redirectUri)
        {
            // THIS WAS REMOVED WITH ACS SUPPORT
            return Json(false);
        }
    }
}