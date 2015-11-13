using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.SchoolsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AdminDistrictController : ChalkableController
    {
        // GET: AdminDistrict
        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DistrictSummary()
        {
            Trace.Assert(Context.DistrictId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            var district = MasterLocator.DistrictService.GetByIdOrNull(Context.DistrictId.Value);
            var currSchoolYear = SchoolLocator.SchoolYearService.GetSchoolYearById(Context.SchoolYearId.Value);
            var schoolsCount = SchoolLocator.SchoolService.GetSchoolsCountByAcadYear(currSchoolYear.AcadYear);
            var studentsCount = SchoolLocator.StudentService.GetStudentsCountByAcadYear(currSchoolYear.AcadYear);
            return Json(ShortDistrictSummaryViewData.Create(district, studentsCount, schoolsCount));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult Schools(string filter, int? start, int? count)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);

            var currSchoolYear = SchoolLocator.SchoolYearService.GetSchoolYearById(Context.SchoolYearId.Value);
            var schools = SchoolLocator.SchoolService.GetShortSchoolSummariesByAcadYear(currSchoolYear.AcadYear, start,
                count, filter);
            return Json(LocalSchoolSummaryViewData.Create(schools));
        }
    }
}