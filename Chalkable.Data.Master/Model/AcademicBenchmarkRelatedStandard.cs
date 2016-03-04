using System;
using System.Collections.Generic;

namespace Chalkable.Data.Master.Model
{
    public class AcademicBenchmarkRelatedStandard : AcademicBenchmarkStandard
    {
        public IList<AcademicBenchmarkStandard> RelatedStandard { get; set; }
    }
}
