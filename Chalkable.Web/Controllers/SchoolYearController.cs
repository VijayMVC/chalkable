using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class SchoolYearController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student", true, new[] {AppPermissionType.Schedule})]
        public ActionResult List(int? schoolId, int? start, int? count)
        {
            var res = SchoolLocator.SchoolYearService.GetSchoolYears(start ?? 0, count ?? 10, schoolId);
            return Json(res.Transform(SchoolYearViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student", true, new[] {AppPermissionType.Schedule})]
        public ActionResult CurrentSchoolYear()
        {
            var res = SchoolLocator.SchoolYearService.GetCurrentSchoolYear();
            return Json(SchoolYearViewData.Create(res));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult ListOfSchoolYearClasses()
        {
            Trace.Assert(Context.PersonId.HasValue);

            var schoolYears = SchoolLocator.SchoolYearService.GetSchoolYears();
            var schoolYearsClasses =
                SchoolLocator.ClassService.GetClassesBySchoolYearIds(schoolYears.Select(x => x.Id).ToList(), Context.PersonId.Value);
            var schools = SchoolLocator.SchoolService.GetSchoolsByIds(schoolYears.Select(x => x.SchoolRef).ToList());


            return Json(SchoolYearClassesViewData.Create(schoolYears, schoolYearsClasses, schools));
        }
    }
}