using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    public class AcademicBenchmarkController : ChalkableController
    {
        public ActionResult StandatdsIds(IList<Guid> standardsIds)
        {
            var academicBenchmarkStandards = MasterLocator.AcademicBenchmarkService.StandatdsIds((standardsIds));
            return Json(academicBenchmarkStandards.Select(AcademicBenchmarkStandardViewData.Create));
        }

        public ActionResult RelateStandardsByIds(IList<Guid> standardsIds)
        {
            var academicBenchmarkRelatedStandards = MasterLocator.AcademicBenchmarkService.RelatedStandardsByIds((standardsIds));
            return Json(academicBenchmarkRelatedStandards.Select(AcademicBenchmarkRelatedStandardViewData.Create));
        }
    }
}