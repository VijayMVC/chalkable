using Chalkable.Data.Master.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chalkable.Web.Models
{
    public class AcademicBenchmarkStandardViewData
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int AuthorityId { get; set; }
        public int DocumentId { get; set; }
        public int ParentId { get; set; }
         
        public static AcademicBenchmarkStandardViewData Create(AcademicBenchmarkStandard academicBenchmarkStandard)
        {
            return new AcademicBenchmarkStandardViewData
            {
                Id = academicBenchmarkStandard.Id,
                Description = academicBenchmarkStandard.Description,
                AuthorityId = academicBenchmarkStandard.AuthorityId,
                DocumentId = academicBenchmarkStandard.DocumentId,
                ParentId = academicBenchmarkStandard.ParentId
            };
        }
    }
}