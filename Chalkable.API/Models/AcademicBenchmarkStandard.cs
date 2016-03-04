using System;
using System.Collections.Generic;

namespace Chalkable.API.Models
{
    public class AcademicBenchmarkStandard
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDeepest { get; set; }
        public Guid? ParentId { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public AcademicBenchmarkAuthority Authority { get; set; }
        public AcademicBenchmarkDocument Document { get; set; }
    }

    public class AcademicBenchmarkAuthority
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class AcademicBenchmarkDocument
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class AcademicBenchmarkStandardRelations
    {
        public AcademicBenchmarkStandard CurrentStandard { get; set; }
        public IList<AcademicBenchmarkStandard> Origins { get; set; }
        public IList<AcademicBenchmarkStandard> Derivative { get; set; }
        public IList<AcademicBenchmarkStandard> RelatedDerivative { get; set; }
    }

}
