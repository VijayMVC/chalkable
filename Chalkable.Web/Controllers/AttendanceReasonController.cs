using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AttendancesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AttendanceReasonController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher", true, new[] {AppPermissionType.Attendance})]
        public ActionResult List()
        {
            return Json(AttendanceReasonViewData.Create(SchoolLocator.AttendanceReasonService.List()));
        }

        public ActionResult GetAllReasons()
        {
            return Json(AttendanceReasonDetailsViewData.Create(SchoolLocator.AttendanceReasonService.List(false)));
        }
    }
}