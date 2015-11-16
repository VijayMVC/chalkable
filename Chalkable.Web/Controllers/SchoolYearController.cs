using System.Collections.Generic;
using System.Linq;
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
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Schedule })]
        public ActionResult List(int? schoolId, int? start, int? count)
        {
            var res = SchoolLocator.SchoolYearService.GetSchoolYears(start ?? 0, count ?? 10, schoolId);
            return Json(res.Transform(SchoolYearViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Schedule })]
        public ActionResult CurrentSchoolYear()
        {
            var res = SchoolLocator.SchoolYearService.GetCurrentSchoolYear();
            return Json(SchoolYearViewData.Create(res));
        }
 }
}