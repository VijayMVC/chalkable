using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DistrictController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin")]
        public ActionResult List(int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
            var districts = MasterLocator.DistrictService.GetDistricts(start.Value, count.Value);
            return Json(districts.Transform(DistrictViewData.Create));
        }

    }
}
