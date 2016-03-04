using System;

namespace Chalkable.Data.Master.Model
{
    public class AcademicBenchmarkStandard
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public AcademicBenchmarkAuthority Authority { get; set; }
        public AcademicBenchmarkDocument Document { get; set; }
        public Guid ParentId { get; set; }
    }
}
