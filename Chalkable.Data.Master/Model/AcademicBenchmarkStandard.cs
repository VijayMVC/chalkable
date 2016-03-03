using System;

namespace Chalkable.Data.Master.Model
{
    public class AcademicBenchmarkStandard
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int AuthorityId { get; set; }
        public int DocumentId { get; set; }
        public int ParentId { get; set; }
    }
}
