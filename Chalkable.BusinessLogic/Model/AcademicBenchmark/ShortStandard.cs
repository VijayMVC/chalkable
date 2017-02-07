using System;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class ShortStandard
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDeepest { get; set; }
        public Guid? ParentId { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }

        public ShortStandard() { }
        protected ShortStandard(AcademicBenchmarkConnector.Models.ShortStandard standard)
        {
            Id = standard.Id;
            Code = standard.Number;
            Description = standard.Description;
            IsDeepest = standard.IsDeepest;
            IsActive = standard.IsActive;
            ParentId = standard.Parent?.Id;
            Level = standard.Level;
        }
    }
}
