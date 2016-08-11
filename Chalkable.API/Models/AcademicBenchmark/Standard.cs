using System;

namespace Chalkable.API.Models.AcademicBenchmark
{
    public class Standard
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDeepest { get; set; }
        public Guid? ParentId { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public Authority Authority { get; set; }
        public Document Document { get; set; }
    }
}
