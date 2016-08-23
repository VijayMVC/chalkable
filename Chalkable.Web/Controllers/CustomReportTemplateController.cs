using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class CustomReportTemplateController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher")]
        public ActionResult List()
        {
            var res = MasterLocator.CustomReportTemplateService.GetList();
            return Json(ShortCustomReportTemplateViewData.Create(res));
        }
    }
}