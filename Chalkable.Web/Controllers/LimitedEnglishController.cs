using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class LimitedEnglishController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new [] {AppPermissionType.User})]
        public ActionResult List()
        {
            var res = SchoolLocator.LimitedEnglishService.GetList(true);
            return Json(LimitedEnglishViewData.Create(res));
        }
    }
}