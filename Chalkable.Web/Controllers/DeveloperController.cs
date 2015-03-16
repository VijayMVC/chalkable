﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
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
                    var token = ChalkableApiExplorerLogic.GetAccessTokenFor(context.Login, context.SchoolYearId, MasterLocator);
                    result.Add(ApiExplorerViewData.Create(description.Value, token, description.Key));
                }
                    
            }
            return Json(result, 8);
        }

        public ActionResult GetRequiredMethodCallsFor(string query, bool isMethod, string role)
        {
            var list = ChalkableApiExplorerLogic.IsValidApiRole(role) ? ApiPathfinder.GetRequiredMethodCallsFor(query, isMethod, role) 
                                                                      : new List<ApiExplorerDropdownItemViewData>();
            return Json(list);
        }
        public ActionResult MethodParamList(string query, string role)
        {
            var list = ChalkableApiExplorerLogic.IsValidApiRole(role) ? ApiPathfinder.GetParamsListByQuery(query, role)
                                                                      : new List<ApiExplorerDropdownItemViewData>();
            return Json(list);
        }

        public ActionResult DeveloperDocs()
        {
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

        public string ApplicationPath
        {
            get
            {
                string res = Request.ApplicationPath.ToLower();
                if (res == "/")      //a site
                    res = "/";
                else if (!res.EndsWith(@"/")) //a virtual
                    res += @"/";
                return res;

            }
        }

        public ActionResult DownloadPictureTemplate(int type)
        {
            var vPath = ApplicationPath + ICON_TEMPLATES_URL_FORMAT;
            String name;
            switch (type)
            {
                case 1: name = ICON_FILENAME; break;
                case 2: name = BANNER_FILENAME; break;
                default: name = SCREENSHOT_FILENAME; break;
            }
            vPath = string.Format(vPath, name);
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
                    string.IsNullOrEmpty(timeZoneId) ? DateTime.UtcNow : DateTime.UtcNow.ConvertFromUtc(timeZoneId), timeZoneId, ip);
            }
            return Json(DeveloperViewData.Create(res));
        }

        [AuthorizationFilter("Developer")]
        public ActionResult ChangePayPalLogin(Guid developerId, string paypalAddress)
        {
            if (string.IsNullOrEmpty(paypalAddress))
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