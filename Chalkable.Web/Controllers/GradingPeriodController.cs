using System.Linq;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.GradingViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class GradingPeriodController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult List(int? schoolYearId)
        {
            schoolYearId = schoolYearId ?? GetCurrentSchoolYearId();
            var res = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(schoolYearId.Value);
            return Json(res.Select(GradingPeriodViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult ListByClassId(int classId)
        {
            var res = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetailsByClassId(classId);
            return Json(res.Select(GradingPeriodViewData.Create));
        }
    }
}