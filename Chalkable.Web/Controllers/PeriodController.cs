using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    public class PeriodController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher", true, new[] { AppPermissionType.Schedule })]
        public ActionResult List(int schoolYearId)
        {
            var res = SchoolLocator.PeriodService.GetPeriods(schoolYearId);
            return Json(PeriodViewData.Create(res));
        }
    }
}