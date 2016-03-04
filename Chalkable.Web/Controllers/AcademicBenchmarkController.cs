using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    public class AcademicBenchmarkController : ChalkableController
    {
        public async Task<ActionResult> StandatdsIds(IList<Guid> standardsIds)
        {
            var academicBenchmarkStandards = await MasterLocator.AcademicBenchmarkService.GetStandardsByIds((standardsIds));
            return Json(academicBenchmarkStandards.Select(AcademicBenchmarkStandardViewData.Create));
        }

        public async Task<ActionResult> RelateStandardsByIds(IList<Guid> standardsIds)
        {
            var academicBenchmarkRelatedStandards = await MasterLocator.AcademicBenchmarkService.GetListOfStandardRelations((standardsIds));
            return Json(academicBenchmarkRelatedStandards.Select(AcademicBenchmarkStandardRelationsViewData.Create));
        }
    }
}