﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ChalkableApiExplorerViewData;

namespace Chalkable.Web.Controllers
{
    public class DeveloperController : ChalkableController
    {
        public ActionResult ListApi()
        {
            var demoSchools = MasterLocator.SchoolService.GetSchools(null);
            //var id = MasterLocator.Context.SchoolId;
            //var currentSchool = demoSchools.First(x => x.Id == id);

            //TODO: get prefix from demo school 
            var prefix = "test";
            //if (currentSchool != null)
            //    prefix = currentSchool.DemoPrefix;

            var result = new List<ApiExplorerViewData>();

            if (!string.IsNullOrEmpty(prefix))
            {

                var descriptions = ChalkableApiExplorerLogic.GetApi();

                var apiRoles = new List<string>();

                foreach (var description in descriptions)
                {
                    var loweredDescription = description.Key.ToLowerInvariant();
                    if (loweredDescription == CoreRoles.SUPER_ADMIN_ROLE.LoweredName || loweredDescription == CoreRoles.CHECKIN_ROLE.LoweredName) continue;

                    apiRoles.Add(loweredDescription);
                    var userName = prefix + PreferenceService.Get("demoschool" + loweredDescription).Value;
                    var token = ChalkableApiExplorerLogic.GetAccessTokenFor(userName, MasterLocator);
                    result.Add(ApiExplorerViewData.Create(description.Value, token, description.Key));
                }

            }
            return Json(result, 8);
        }

        public ActionResult GetRequiredMethodCallsFor(string query, bool isMethod, string role)
        {
            var list = ApiPathfinder.GetRequiredMethodCallsFor(query, isMethod, role);
            return Json(list);
        }
        public ActionResult MethodParamList(string query, string role)
        {
            var list = ApiPathfinder.GetParamsListByQuery(query, role);
            return Json(list);
        }

        public ActionResult DeveloperDocs()
        {
            return View();
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult DeveloperInfo(Guid developerId)
        {
            var developer = MasterLocator.DeveloperService.GetDeveloperById(developerId);
            return Json(DeveloperViewData.Create(developer));
        }


        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult UpdateInfo(Guid developerId, string name, string websiteLink, string email)
        {
            var res = MasterLocator.DeveloperService.Edit(developerId, name, email, websiteLink);
            //TODO: mix panel 
            //MixPanelService.ChangedEmail(ServiceLocator.Context.UserName, email);
            //if (ServiceLocator.Context.RoleNameLowered == CoreRoles.DEVELOPER_ROLE.LoweredName)
            //{
            //    var timeZoneId = ServiceLocator.Context.TimeZoneId;
            //    var ip = RequestHelpers.GetClientIpAddress(Request);
            //    MixPanelService.IdentifyDeveloper(developer.Email, developer.DisplayName,
            //        string.IsNullOrEmpty(timeZoneId) ? DateTime.UtcNow : DateTime.UtcNow.ConvertFromUtc(timeZoneId), timeZoneId, ip);
            //}
            return Json(DeveloperViewData.Create(res));
        }

        public ActionResult Confirm(string key, Guid applicationId)
        {
            return Confirm(key, (context) => RedirectAction(context, applicationId));
        }
        private ActionResult RedirectAction(UserContext userContext, Guid applicationId)
        {
            if (userContext.Role == CoreRoles.DEVELOPER_ROLE)
                return Redirect<HomeController>(x => x.Developer(null, false, applicationId));
            return Redirect<HomeController>(x => x.Index());
        }
    }
}