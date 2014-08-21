using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class SchoolYearController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_SCHOOLYEAR_LIST, true, CallType.Get, new[] { AppPermissionType.Schedule })]
        public ActionResult List(int? start, int? count)
        {
            var res = SchoolLocator.SchoolYearService.GetSchoolYears(start ?? 0, count ?? 10);
            return Json(res.Transform(SchoolYearViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_SCHOOLYEAR_CURRENT, true, CallType.Get, new[] { AppPermissionType.Schedule })]
        public ActionResult CurrentSchoolYear()
        {
            var res = SchoolLocator.SchoolYearService.GetCurrentSchoolYear();
            return Json(SchoolYearViewData.Create(res));
        }
    }
}