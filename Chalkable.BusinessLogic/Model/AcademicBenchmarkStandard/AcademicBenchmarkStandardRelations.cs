using Chalkable.AcademicBenchmarkConnector.Models;
using System.Collections.Generic;
using System.Linq;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard
{
    public class AcademicBenchmarkStandardRelations
    {
        public AcademicBenchmarkStandard CurrentStandard { get; set; }
        public IList<AcademicBenchmarkStandard> Origin { get; set; }
        public IList<AcademicBenchmarkStandard> Derivative { get; set; }
        public IList<AcademicBenchmarkStandard> RelatedDerivative { get; set; }

        protected AcademicBenchmarkStandardRelations(RelatedStandard standard)
        {
            if (standard.CurrentStandard != null)
                CurrentStandard = new AcademicBenchmarkStandard(standard.CurrentStandard);

            if (standard.Relations == null) return;

            if (standard.Relations.Origins != null)
                Origin = standard.Relations.Origins.Select(x => new AcademicBenchmarkStandard(x.CurrentStandard)).ToList();

            if (standard.Relations.Derivatives != null)
                Origin = standard.Relations.Derivatives.Select(x => new AcademicBenchmarkStandard(x.CurrentStandard)).ToList();

            if (standard.Relations.RelatedDerivatives != null)
                Origin = standard.Relations.RelatedDerivatives.Select(x => new AcademicBenchmarkStandard(x.CurrentStandard)).ToList();
        }

        public static AcademicBenchmarkStandardRelations Create(RelatedStandard standard)
        {
            return new AcademicBenchmarkStandardRelations(standard);
        }
    }
}
