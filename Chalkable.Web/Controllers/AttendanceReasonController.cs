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
        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_ATTENDANCE_REASON_LIST, true, CallType.Post, new[] {AppPermissionType.Attendance})]
        public ActionResult List()
        {
            return Json(AttendanceReasonViewData.Create(SchoolLocator.AttendanceReasonService.List()));
        }
    }
}