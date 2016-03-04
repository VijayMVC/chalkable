using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.ABStandardsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AcademicBenchmarkController : ChalkableController
    {
        //TODO: impl some auth logic for these methods later
        public async Task<ActionResult> StandatdsIds(GuidList standardsIds)
        {
            MasterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var academicBenchmarkStandards = await MasterLocator.AcademicBenchmarkService.GetStandardsByIds((standardsIds));
            return Json(academicBenchmarkStandards.Select(AcademicBenchmarkStandardViewData.Create));
        }

        public async Task<ActionResult> ListOfStandardRelationsByIds(GuidList standardsIds)
        {
            MasterLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var academicBenchmarkRelatedStandards = await MasterLocator.AcademicBenchmarkService.GetListOfStandardRelations((standardsIds));
            return Json(academicBenchmarkRelatedStandards.Select(AcademicBenchmarkStandardRelationsViewData.Create));
        }
    }
}