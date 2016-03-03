using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class AcademicBenchmarkRelatedStandardViewData
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int AuthorityId { get; set; }
        public int DocumentId { get; set; }
        public int ParentId { get; set; }
        public IList<AcademicBenchmarkStandard> RelatedStandard { get; set; }

        public static AcademicBenchmarkRelatedStandardViewData Create(AcademicBenchmarkRelatedStandard academickBenchmarkRelatedStandard)
        {
            return new AcademicBenchmarkRelatedStandardViewData
            {
                Id = academickBenchmarkRelatedStandard.Id,
                Description = academickBenchmarkRelatedStandard.Description,
                AuthorityId = academickBenchmarkRelatedStandard.AuthorityId,
                DocumentId = academickBenchmarkRelatedStandard.DocumentId,
                ParentId = academickBenchmarkRelatedStandard.ParentId,
                RelatedStandard = academickBenchmarkRelatedStandard.RelatedStandard
            };
        }
    }
}