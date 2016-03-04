using System;
using System.Collections.Generic;

namespace Chalkable.Data.Master.Model
{
    public class AcademicBenchmarkRelatedStandard
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public AcademicBenchmarkAuthority Authority { get; set; }
        public AcademicBenchmarkDocument Document { get; set; }
        public Guid ParentId { get; set; }
        public IList<AcademicBenchmarkStandard> RelatedStandard { get; set; }
    }
}
