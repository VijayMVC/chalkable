using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [TraceControllerFilter]
    public class PreferenceController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin")]
        public ActionResult List(PreferenceCategoryEnum? category)
        {
            var preferences = MasterLocator.PreferenceService.List(category);
            return Json(PreferenceViewData.Create(preferences));
        }
        public ActionResult ListPublic()
        {
            var preferences = MasterLocator.PreferenceService.ListPublic();
            IList<PreferenceViewData> res = new List<PreferenceViewData>();
            if(preferences.Count > 0)
                res = PreferenceViewData.Create(preferences);
            return Json(res);
        }
        [AuthorizationFilter("SysAdmin")]
        public ActionResult Get(string key)
        {
            var preference = PreferenceService.Get(key);
            if(preference != null)
                return Json(PreferenceViewData.Create(preference));
            return Json(null);
        }
        [Authorize]
        public ActionResult GetPublic(string key)
        {
            var preference = PreferenceService.GetPublic(key);
            if(preference != null)
                return Json(PreferenceViewData.Create(preference));
            return Json(null);
        }
        [ValidateInput(false)]
        [AuthorizationFilter("SysAdmin")]
        public ActionResult Set(string key, string value, bool ispublic)
        {
            MasterLocator.PreferenceService.Set(key, value, ispublic);
            var preferences = MasterLocator.PreferenceService.List();
            return Json(PreferenceViewData.Create(preferences));
        }

     }
}