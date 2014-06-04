using System;
using System.Configuration;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.MixPanel;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class UserController : ChalkableController
    {
        //TODO: remove this later 
        public ActionResult EncryptPassword(string password)
        {
           return Json(MasterLocator.UserService.PasswordMd5(password));
        }

        public ActionResult SisLogIn(string token, Guid districtId, DateTime? tokenExpiresTime, int? acadSessionId)
        {
            var expiresTime = tokenExpiresTime ?? DateTime.UtcNow.AddDays(2);
            var context = LogOn(false, us => us.SisLogIn(districtId, token, expiresTime, acadSessionId));
            if (context != null)
               return RedirectToHome(context.Role);
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
            var context = LogOn(remember, us => us.Login(userName, password));
            if (context != null)
                return Json(new { Success = true, data = new { Role = context.Role.LoweredName } }, JsonRequestBehavior.AllowGet);
            return Json(new { Success = false, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            ChalkableAuthentication.SignOut();
            return Json(new { success = true, data = new { success = true } }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Confirm(string key)
        {
            return Confirm(key, AfterConfirmAction);
        }

        public ActionResult ChangePassword(string oldPassword, string newPassword, string newPasswordConfirmation)
        {
            var login = Context.Login;
            if (MasterLocator.UserService.Login(login, oldPassword) != null)
            {
                if (newPassword == newPasswordConfirmation)
                {
                    MasterLocator.UserService.ChangePassword(login, newPassword);
                    MixPanelService.ChangedPassword(Context.Login);
                    return Json(true);
                }
                return Json(new ChalkableException("new password and confirmation dont't match"));
            }
            return Json(new ChalkableException("old password is incorrect"));
        }
   
        private ActionResult AfterConfirmAction(UserContext context)
        {
            //TODO: create default Announcement for teacher
            if(context.UserLocalId.HasValue)
                SchoolLocator.PersonService.ActivatePerson(context.UserLocalId.Value);
            //TODO: mix panel 
            if (context.Role == CoreRoles.SUPER_ADMIN_ROLE)
                return Redirect<HomeController>(x => x.SysAdmin());
            if (context.Role == CoreRoles.TEACHER_ROLE)
                return Redirect<HomeController>(x => x.Teacher(true));
            if (context.Role == CoreRoles.DEVELOPER_ROLE)
                return Redirect<HomeController>(x => x.Developer(null));
            return Redirect<HomeController>(c => c.Index());
        }

        protected ActionResult Confirm(string key, Func<UserContext, ActionResult> redirectAction)
        {
            var context = LogOn(false, us => us.Login(key));
            if (context != null)
            {
                InitServiceLocators(context);
                MixPanelService.UserLoggedInForFirstTime(context.Login, "", "", Context.SchoolId.ToString(), 
                    DateTime.UtcNow.ConvertFromUtc(Context.SchoolTimeZoneId), Context.SchoolTimeZoneId, Context.Role.Name);
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
                ChalkableAuthentication.SignIn(context, remember);
            return context;
        }

        private const string SERVICE_NAMESPACE = "WindowsAzure.OAuth.ServiceNamespace";
        private const string ACS_URL_FORMAT = "https://{0}.accesscontrol.windows.net/v2/OAuth2-13/";
        private const string RELYING_PARTY_REALM = "WindowsAzure.OAuth.RelyingPartyRealm";

        public ActionResult GetAccessToken(string login, string password, string clientId, string clientSecret, string redirectUri)
        {
            try
            {
                var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
                var context = serviceLocator.UserService.Login(login, password);
                if (context != null)
                {
                    var accessTokenUri = string.Format(ACS_URL_FORMAT, ConfigurationManager.AppSettings[SERVICE_NAMESPACE]);
                    var scope = ConfigurationManager.AppSettings[RELYING_PARTY_REALM];
                    return Json(new
                    {
                        token = serviceLocator.AccessControlService.GetAccessToken(accessTokenUri, redirectUri, clientId,
                                                           clientSecret, login, scope)
                    }, 5);
                }
            }
            catch (Exception e)
            {
                return Json(false);
            }
            return Json(false);
        }



    }
}