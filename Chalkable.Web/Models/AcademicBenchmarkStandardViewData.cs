using Chalkable.Data.Master.Model;
using System;

namespace Chalkable.Web.Models
{
    public class AcademicBenchmarkStandardViewData
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public AcademicBenchmarkAuthority Authority { get; set; }
        public AcademicBenchmarkDocument Document { get; set; }
        public Guid ParentId { get; set; }
         
        public static AcademicBenchmarkStandardViewData Create(AcademicBenchmarkStandard academicBenchmarkStandard)
        {
            return new AcademicBenchmarkStandardViewData
            {
                Id = academicBenchmarkStandard.Id,
                Description = academicBenchmarkStandard.Description,
                Authority = academicBenchmarkStandard.Authority,
                Document = academicBenchmarkStandard.Document,
                ParentId = academicBenchmarkStandard.ParentId
            };
        }
    }
}