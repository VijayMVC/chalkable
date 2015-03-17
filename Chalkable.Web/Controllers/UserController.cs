﻿using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;
using Chalkable.Web.Tools;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class UserController : ChalkableController
    {
        public ActionResult SisLogIn(string token, Guid districtId, DateTime? tokenExpiresTime, int? acadSessionId)
        {
            var expiresTime = tokenExpiresTime ?? DateTime.UtcNow.AddDays(2);
            var context = LogOn(false, us => us.SisLogIn(districtId, token, expiresTime, acadSessionId));
            if (context != null)
            {
                MasterLocator.UserTrackingService.LoggedInFromINow(context.Login);
                return RedirectToHome(context.Role); 
            }
            return Redirect<HomeController>(x => x.Index());
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult RedirectToINow()
        {
            if (!Context.DistrictId.HasValue)
                throw new Exception("District id should be defined for redirect to SIS");
            var schoolYearId = GetCurrentSchoolYearId();
            var district = MasterLocator.DistrictService.GetByIdOrNull(Context.DistrictId.Value);
            var sisUrl = district.SisRedirectUrl;
            var url = UrlTools.UrlCombine(sisUrl, string.Format("TokenLogin.aspx?Token={0}&AcadSessionId={1}", Context.SisToken, schoolYearId));
            return Redirect(url);
        }

        public ActionResult LogOn(string userName, string password, bool remember)
        {
            if(string.IsNullOrEmpty(userName))
                return Json(new { Success = false, ErrorMessage = "Enter your email", UserName = userName }, JsonRequestBehavior.AllowGet);
            string error = null;
            var context = LogOn(remember, us => us.Login(userName, password, out error));
            if (context != null)
            {
                MasterLocator.UserTrackingService.LoggedInFromChalkable(context.Login);
                return Json(new { Success = true, data = new { Role = context.Role.LoweredName } }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, ErrorMessage = error, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            ChalkableAuthentication.SignOut();
            DeveloperAuthentication.SignOut();
            return Json(new { success = true, data = new { success = true } }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Confirm(string key)
        {
            return Confirm(key, AfterConfirmAction);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ActivateUser(string newUserEmail)
        {
            if (!Context.PersonId.HasValue)
                return Json(new UnassignedUserException());
            string error;
            SchoolLocator.PersonService.EditEmailForCurrentUser(newUserEmail, out error);
            if (!string.IsNullOrEmpty(error))
                return Json(new ChalkableException(error));
            SchoolLocator.PersonService.ActivatePerson(Context.PersonId.Value);
            return Json(true);            
        }
        
        public ActionResult ChangePassword(string oldPassword, string newPassword, string newPasswordConfirmation)
        {
            if (!PasswordTools.IsSecurePassword(newPassword))
                return Json(new ChalkableException("new password is not secure enough"));

            var login = Context.Login;
            if (!string.IsNullOrEmpty(oldPassword) && MasterLocator.UserService.Login(login, oldPassword) == null)
                return Json(new ChalkableException("old password is incorrect"));

            if (newPassword != newPasswordConfirmation)
                return Json(new ChalkableException("new password and confirmation doesn't match"));

            MasterLocator.UserService.ChangePassword(login, newPassword);
            MasterLocator.UserTrackingService.ChangedPassword(Context.Login);
            return Json(true);
        }
        
        public ActionResult ResetPassword(string email)
        {
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
   
        private ActionResult AfterConfirmAction(UserContext context)
        {
            if (context.PersonId.HasValue)
                SchoolLocator.PersonService.ActivatePerson(context.PersonId.Value);
            //TODO: mix panel 
            if (context.Role == CoreRoles.SUPER_ADMIN_ROLE)
                return Redirect<HomeController>(x => x.SysAdmin());
            if (context.Role == CoreRoles.TEACHER_ROLE)
                return Redirect<HomeController>(x => x.Teacher());
            if (context.Role == CoreRoles.DEVELOPER_ROLE)
                return Redirect<HomeController>(x => x.Developer(null, true));
            return Redirect<HomeController>(c => c.Index());
        }

        protected ActionResult Confirm(string key, Func<UserContext, ActionResult> redirectAction)
        {
            var context = LogOn(false, us => us.Login(key));
            if (context != null)
            {
                InitServiceLocators(context);
                MasterLocator.UserTrackingService.UserLoggedInForFirstTime(context.Login, "", "", Context.DistrictId.ToString(), 
                        DateTime.UtcNow.ConvertFromUtc(Context.DistrictTimeZone), Context.DistrictTimeZone, Context.Role.Name);
                return redirectAction(context);
            }
            return Redirect<HomeController>(c => c.Index());
        }

        protected UserContext LogOn(bool remember, Func<IUserService, UserContext> logOnAction)
        {
            var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            return LogOn(remember, serviceLocator.UserService, logOnAction);
        }

        protected UserContext LogOn(bool remember, IUserService userService, Func<IUserService, UserContext> logOnAction)
        {
            var context = logOnAction(userService);
            if (context != null)
            {
                ChalkableAuthentication.SignIn(context, remember);
                if (context.DeveloperId.HasValue && !DemoUserService.IsDemoUser(context))
                    DeveloperAuthentication.SignIn(context, remember);
            }
            return context;
        }

        public ActionResult GetAccessToken(string login, string password, string clientId, string clientSecret, string redirectUri)
        {
            try
            {
                var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
                var context = serviceLocator.UserService.Login(login, password);
                if (context != null)
                {
                    var accessTokenUri = string.Format("https://{0}.accesscontrol.windows.net/v2/OAuth2-13/", Settings.WindowsAzureOAuthServiceNamespace);
                    var scope = Settings.WindowsAzureOAuthRelyingPartyRealm;
                    return Json(new
                    {
                        token = serviceLocator.AccessControlService.GetAccessToken(accessTokenUri, redirectUri, clientId,
                                                           clientSecret, login, context.SchoolYearId, scope)
                    }, 5);
                }
            }
            catch (Exception)
            {
                return Json(false);
            }
            return Json(false);
        }
    }
}