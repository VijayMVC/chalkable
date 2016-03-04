using System;
using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard
{
    public class AcademicBenchmarkRelatedStandard : AcademicBenchmarkStandard
    {
        public IList<AcademicBenchmarkStandard> RelatedStandard { get; set; }
    }
}
