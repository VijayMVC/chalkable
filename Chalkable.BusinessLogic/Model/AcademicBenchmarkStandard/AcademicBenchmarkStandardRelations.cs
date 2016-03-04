using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard
{
    public class AcademicBenchmarkStandardRelations
    {
        public IList<AcademicBenchmarkStandard> Origign { get; set; }
        public IList<AcademicBenchmarkStandard> Derivative { get; set; }
        public IList<AcademicBenchmarkStandard> RelatedDerivative { get; set; }

    }
}
