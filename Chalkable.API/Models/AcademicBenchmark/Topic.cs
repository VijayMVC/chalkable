using System;

namespace Chalkable.API.Models.AcademicBenchmark
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Deepest { get; set; }
        public short Level { get; set; }
        public string Status { get; set; }
        public Guid ParentId { get; set; }
        public bool IsDeepest { get; set; }
        public bool IsActive { get; set; }
    }
}
