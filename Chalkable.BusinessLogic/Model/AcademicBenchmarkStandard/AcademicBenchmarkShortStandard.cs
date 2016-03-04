using System;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard
{
    public class AcademicBenchmarkShortStandard
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDeepest { get; set; }
        public Guid ParentId { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
    }
}
