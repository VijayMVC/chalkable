using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Web.Logic;
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
    }
}