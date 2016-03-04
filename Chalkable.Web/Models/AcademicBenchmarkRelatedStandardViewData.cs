using System;
using System.Collections.Generic;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class AcademicBenchmarkRelatedStandardViewData
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public AcademicBenchmarkAuthority Authority { get; set; }
        public AcademicBenchmarkDocument Document { get; set; }
        public Guid ParentId { get; set; }
        public IList<AcademicBenchmarkStandard> RelatedStandard { get; set; }

        public static AcademicBenchmarkRelatedStandardViewData Create(AcademicBenchmarkRelatedStandard academickBenchmarkRelatedStandard)
        {
            return new AcademicBenchmarkRelatedStandardViewData
            {
                Id = academickBenchmarkRelatedStandard.Id,
                Description = academickBenchmarkRelatedStandard.Description,
                Authority = academickBenchmarkRelatedStandard.Authority,
                Document = academickBenchmarkRelatedStandard.Document,
                ParentId = academickBenchmarkRelatedStandard.ParentId,
                RelatedStandard = academickBenchmarkRelatedStandard.RelatedStandard
            };
        }
    }
}