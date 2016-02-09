using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
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
            var schools = SchoolLocator.SchoolService.GetShortSchoolSummariesInfo(0, int.MaxValue, null, null);
            var schoolsCount = schools.Count;
            var studentsCount = schools.Sum(x => x.SchoolDetails.StudentsCount);
            return Json(ShortDistrictSummaryViewData.Create(district, studentsCount, schoolsCount));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult Schools(string filter, int? start, int? count, int? sortType)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var schools = SchoolLocator.SchoolService.GetShortSchoolSummariesInfo(start, count, filter, (SchoolSortType?) sortType);

            return Json(schools.Select(LocalSchoolSummaryViewData.Create));
        }
    }
}