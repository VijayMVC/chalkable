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

            if (standard.Relations.Origins != null)
                Origins = standard.Relations.Origins.Select(x =>AcademicBenchmarkStandard.Create(x.CurrentStandard)).ToList();

            if (standard.Relations.Derivatives != null)
                Origins = standard.Relations.Derivatives.Select(x => AcademicBenchmarkStandard.Create(x.CurrentStandard)).ToList();

            if (standard.Relations.RelatedDerivatives != null)
                Origins = standard.Relations.RelatedDerivatives.Select(x => AcademicBenchmarkStandard.Create(x.CurrentStandard)).ToList();
        }

        public static AcademicBenchmarkStandardRelations Create(RelatedStandard standard)
        {
            return new AcademicBenchmarkStandardRelations(standard);
        }
    }
}
