using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class MarkingPeriodController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Schedule })]
        public ActionResult List(int schoolYearId, DateTime? tillDate)
        {
            var res = SchoolLocator.MarkingPeriodService.GetMarkingPeriods(schoolYearId);
            if (tillDate.HasValue)
                res = res.Where(x => x.StartDate <= tillDate).ToList();
            return Json(MarkingPeriodViewData.Create(res));
        }
    }
}