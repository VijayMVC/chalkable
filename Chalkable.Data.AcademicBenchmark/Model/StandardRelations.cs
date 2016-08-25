using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    public class StandardRelations
    {
        public IList<Standard> Origins { get; set; }
        public IList<Standard> Derivatives { get; set; }
        public IList<Standard> RelatedDerivatives { get; set; }
    }
}
