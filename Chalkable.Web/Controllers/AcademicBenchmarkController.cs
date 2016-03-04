using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.ABStandardsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AcademicBenchmarkController : ChalkableController
    {
        public async Task<ActionResult> StandatdsIds(IList<Guid> standardsIds)
        {
            var academicBenchmarkStandards = await MasterLocator.AcademicBenchmarkService.GetStandardsByIds((standardsIds));
            return Json(academicBenchmarkStandards.Select(AcademicBenchmarkStandardViewData.Create));
        }

        public async Task<ActionResult> ListOfStandardRelationsByIds(IList<Guid> standardsIds)
        {
            var academicBenchmarkRelatedStandards = await MasterLocator.AcademicBenchmarkService.GetListOfStandardRelations((standardsIds));
            return Json(academicBenchmarkRelatedStandards.Select(AcademicBenchmarkStandardRelationsViewData.Create));
        }
    }
}