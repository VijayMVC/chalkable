﻿using System.Collections.Generic;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    public class StandardRelations
    {
        public Standard Standard { get; set; } 
        public IList<Standard> Origins { get; set; }
        public IList<Standard> Derivatives { get; set; }
        public IList<Standard> RelatedDerivatives { get; set; }
    }
}
