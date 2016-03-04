﻿using System.Collections.Generic;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class AcademicBenchmarkRelatedStandardViewData : AcademicBenchmarkStandardViewData
    {
        public IList<AcademicBenchmarkStandard> RelatedStandard { get; set; }

        public static AcademicBenchmarkRelatedStandardViewData Create(AcademicBenchmarkRelatedStandard academickBenchmarkRelatedStandard)
        {
            return new AcademicBenchmarkRelatedStandardViewData
            {
                Id = academickBenchmarkRelatedStandard.Id,
                Description = academickBenchmarkRelatedStandard.Description,
                IsDeepest = academickBenchmarkRelatedStandard.IsDeepest,
                ParentId = academickBenchmarkRelatedStandard.ParentId,
                Level = academickBenchmarkRelatedStandard.Level,
                IsActive = academickBenchmarkRelatedStandard.IsActive,
                Authority = academickBenchmarkRelatedStandard.Authority,
                Document = academickBenchmarkRelatedStandard.Document,
                RelatedStandard = academickBenchmarkRelatedStandard.RelatedStandard
            };
        }
    }
}