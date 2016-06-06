using System.Collections.Generic;

namespace Chalkable.API.Models.AcademicBenchmark
{
    public class StandardRelations
    {
        public Standard CurrentStandard { get; set; }
        public IList<Standard> Origins { get; set; }
        public IList<Standard> Derivatives { get; set; }
        public IList<Standard> RelatedDerivatives { get; set; }
    }
}
