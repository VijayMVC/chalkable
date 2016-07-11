using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;
using Chalkable.Web.Logic.ApiExplorer;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ChalkableApiExplorerViewData;
using Chalkable.Web.Tools;

namespace Chalkable.Web.Controllers
{
    public class DeveloperController : UserController
    {
        private const string ICON_TEMPLATES_URL_FORMAT = "/Content/icons-templates/{0}";
        private const string ICON_FILENAME = "icon.psd";
        private const string BANNER_FILENAME = "banner.psd";
        private const string ATTACH_ICON_FIELNAME = "attachIcon.psd";
        private const string SCREENSHOT_FILENAME = "screenshot.psd";
        private const string ATTACHMENT_CONTENT_TYPE = "attachment";
        private const string CONTENT_LENGTH = "Content-Length";

        [HttpPost]
        public ActionResult SignUp(string email, string password)
        {
            if (!PasswordTools.IsSecurePassword(password))
                return Json(new ChalkableException("Your password is not secure enough"));

            var sysLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            if (sysLocator.UserService.GetByLogin(email) != null)
                return Json(new ChalkableException(ChlkResources.ERR_SIGNUP_USER_WITH_EMAIL_ALREADY_EXISTS));

            sysLocator.DeveloperService.Add(email, password, null, null, null);
            return LogOn(email, password, false);
        }

        public ActionResult SignUp()
        {
            return RedirectToAction("Info");
        }


        public ActionResult Info()
        {
            return View();
        }

        public ActionResult ListApi()
        {
            Trace.WriteLine("#123 Developer/ListApi start");
            var result = new List<ApiExplorerViewData>();

            var descriptions = ChalkableApiExplorerLogic.GetApi();

            foreach (var description in descriptions)
            {
                var roleName = description.Key.ToLowerInvariant();
                Trace.WriteLine("#123 Developer/GetAccessToken for role", roleName);
                if (ChalkableApiExplorerLogic.IsValidApiRole(roleName))
                {
                    var context = MasterLocator.UserService.DemoLogin(roleName, Context.UserId.ToString());
                    var token = GetAccessTokenFor(context.Login, context.SchoolYearId, Context.Role);
                    var viewData = ApiExplorerViewData.Create(description.Value, token, description.Key);
                    result.Add(viewData);
                }
            }
            return Json(result, 8);
        }

        private const string AcsUrlFormat = "https://{0}.accesscontrol.windows.net/v2/OAuth2-13/";

        private string GetAccessTokenFor(string userName, int? schoolYearId, CoreRole role)
        {
            var clientId = Settings.ApiExplorerClientId;
            var clientSecret = Settings.ApiExplorerSecret;
            var redirectUri = Settings.ApiExplorerRedirectUri;
            var accessTokenUri = string.Format(AcsUrlFormat, Settings.WindowsAzureOAuthServiceNamespace);
            var scope = Settings.ApiExplorerScope;
            var userInfo = OAuthUserIdentityInfo.Create(userName, role, schoolYearId, null);
            return MasterLocator.AccessControlService.GetAccessToken(accessTokenUri, redirectUri, clientId, clientSecret, userInfo, scope);
        }

        public ActionResult DeveloperDocs(bool InFrame = false)
        {
            ViewBag.InFrame = InFrame;
            return View();
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult DeveloperInfo(Guid developerId)
        {
            var developer = MasterLocator.DeveloperService.GetById(developerId);
            return Json(DeveloperViewData.Create(developer));
        }

        [AuthorizationFilter("SysAdmin, AppTester")]
        public ActionResult GetDevelopers()
        {
            var developers = MasterLocator.DeveloperService.GetDevelopers();
            return Json(developers.Select(DeveloperViewData.Create).ToList());
        }

        public ActionResult DownloadPictureTemplate(int type)
        {
            var vPath = ApplicationPath + ICON_TEMPLATES_URL_FORMAT;
            String name;
            switch (type)
            {
                case 1: name = ICON_FILENAME; break;
                case 2: name = BANNER_FILENAME; break;
                case 4: name = ATTACH_ICON_FIELNAME; break;
                default: name = SCREENSHOT_FILENAME; break;
            }
            vPath = String.Format(vPath, name);
            var path = HttpContext.Server.MapPath(vPath);
            var resContent = System.IO.File.ReadAllBytes(path);
            const string contentType = ATTACHMENT_CONTENT_TYPE;
            Response.AddHeader(CONTENT_LENGTH, resContent.Length.ToString());
            var r = File(resContent, contentType, name);
            return r;
        }


        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult UpdateInfo(Guid developerId, string name, string websiteLink, string email)
        {
            var user = MasterLocator.UserService.GetByLogin(email);
            if (user != null && user.Id != Context.UserId)
            {
                return Json(new ChalkableException("User email already exists"));    
            }

            var res = MasterLocator.DeveloperService.Edit(developerId, name, email, websiteLink, null);
            MasterLocator.UserTrackingService.ChangedEmail(Context.Login, email);
            if (Context.Role.LoweredName == CoreRoles.DEVELOPER_ROLE.LoweredName)
            {
                var timeZoneId = Context.DistrictTimeZone;
                var ip = RequestHelpers.GetClientIpAddress(Request);
                MasterLocator.UserTrackingService.IdentifyDeveloper(res.Email, res.DisplayName,
                    String.IsNullOrEmpty(timeZoneId) ? DateTime.UtcNow : DateTime.UtcNow.ConvertFromUtc(timeZoneId), timeZoneId, ip);
            }
            return Json(DeveloperViewData.Create(res));
        }

        [AuthorizationFilter("Developer")]
        public ActionResult ChangePayPalLogin(Guid developerId, string paypalAddress)
        {
            if (String.IsNullOrEmpty(paypalAddress))
                return Json(new ChalkableException("paypal address field is empty"));
            MasterLocator.DeveloperService.ChangePayPalLogin(developerId, paypalAddress);
            return Json(true);
        }

        public ActionResult GoLive(string key, Guid applicationId)
        {
            return Confirm(key, (context) => RedirectAction(context, applicationId));
        }
        private ActionResult RedirectAction(UserContext userContext, Guid applicationId)
        {
            if (userContext.Role == CoreRoles.DEVELOPER_ROLE)
                return Redirect<HomeController>(x => x.Developer(applicationId, null));
            return Redirect<HomeController>(x => x.Index());
        }
    }
}