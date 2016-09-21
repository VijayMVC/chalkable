using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    public class StandardDerivative
    {
        [PrimaryKeyFieldAttr]
        public Guid StandardRef { get; set; }
        [PrimaryKeyFieldAttr]
        public Guid DerivativeRef { get; set; }
    }
}
