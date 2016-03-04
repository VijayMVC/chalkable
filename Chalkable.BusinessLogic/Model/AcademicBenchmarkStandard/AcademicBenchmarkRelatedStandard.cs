using System;
using System.Collections.Generic;
using Chalkable.AcademicBenchmarkConnector.Models;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard
{
    public class AcademicBenchmarkRelatedStandard : AcademicBenchmarkStandard
    {
        public AcademicBenchmarkStandardRelations RelatedStandard { get; set; }

        protected AcademicBenchmarkRelatedStandard(Standard standard) : base(standard)
        {
        }
    }
}
