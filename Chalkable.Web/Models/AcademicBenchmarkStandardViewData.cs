using Chalkable.Data.Master.Model;
using System;

namespace Chalkable.Web.Models
{
    public class AcademicBenchmarkStandardViewData
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsDeepest { get; set; }
        public Guid ParentId { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public AcademicBenchmarkAuthority Authority { get; set; }
        public AcademicBenchmarkDocument Document { get; set; }

        public static AcademicBenchmarkStandardViewData Create(AcademicBenchmarkStandard academicBenchmarkStandard)
        {
            return new AcademicBenchmarkStandardViewData
            {
                Id = academicBenchmarkStandard.Id,
                Description = academicBenchmarkStandard.Description,
                IsDeepest = academicBenchmarkStandard.IsDeepest,
                ParentId = academicBenchmarkStandard.ParentId,
                Level = academicBenchmarkStandard.Level,
                IsActive = academicBenchmarkStandard.IsActive,
                Authority = academicBenchmarkStandard.Authority,
                Document = academicBenchmarkStandard.Document
            };
        }
    }
}