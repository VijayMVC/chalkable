using System.Linq;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DisciplineTypeController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher", true, new[] { AppPermissionType.Discipline })]
        public ActionResult List(int? start, int? count)
        {
            var res = SchoolLocator.InfractionService.GetInfractions(true, true).OrderBy(x => x.Name).ToList();
            return Json(DisciplineTypeViewData.Create(res), 3);
        }
    }
}