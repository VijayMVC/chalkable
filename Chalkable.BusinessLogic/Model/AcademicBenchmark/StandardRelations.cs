using System.Collections.Generic;
using System.Linq;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class StandardRelations
    {
        public Standard CurrentStandard { get; set; }
        public IList<Standard> Origins { get; set; }
        public IList<Standard> Derivatives { get; set; }
        public IList<Standard> RelatedDerivatives { get; set; }

        protected StandardRelations(AcademicBenchmarkConnector.Models.StandardRelations standardRelations)
        {
            if (standardRelations.Data != null)
                CurrentStandard = Standard.Create(standardRelations.Data);

            if (standardRelations.Relations == null) return;
            
            Origins = standardRelations.Relations.Origins?.Select(x =>Standard.Create(x.Standard)).ToList();
            Derivatives = standardRelations.Relations.Derivatives?.Select(x => Standard.Create(x.Standard)).ToList();
            RelatedDerivatives = standardRelations.Relations.RelatedDerivatives?.Select(x => Standard.Create(x.Standard)).ToList();
        }

        public static StandardRelations Create(AcademicBenchmarkConnector.Models.StandardRelations standardRelations)
        {
            return new StandardRelations(standardRelations);
        }
    }
}
