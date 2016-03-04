using Chalkable.AcademicBenchmarkConnector.Models;
using System.Collections.Generic;
using System.Linq;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard
{
    public class AcademicBenchmarkStandardRelations
    {
        public AcademicBenchmarkStandard CurrentStandard { get; set; }
        public IList<AcademicBenchmarkStandard> Origins { get; set; }
        public IList<AcademicBenchmarkStandard> Derivatives { get; set; }
        public IList<AcademicBenchmarkStandard> RelatedDerivatives { get; set; }

        protected AcademicBenchmarkStandardRelations(RelatedStandard standard)
        {
            if (standard.CurrentStandard != null)
                CurrentStandard = AcademicBenchmarkStandard.Create(standard.CurrentStandard);

            if (standard.Relations == null) return;
            
            Origins = standard.Relations.Origins?.Select(x =>AcademicBenchmarkStandard.Create(x.Standard)).ToList();
            Derivatives = standard.Relations.Derivatives?.Select(x => AcademicBenchmarkStandard.Create(x.Standard)).ToList();
            RelatedDerivatives = standard.Relations.RelatedDerivatives?.Select(x => AcademicBenchmarkStandard.Create(x.Standard)).ToList();
        }

        public static AcademicBenchmarkStandardRelations Create(RelatedStandard standard)
        {
            return new AcademicBenchmarkStandardRelations(standard);
        }
    }
}
