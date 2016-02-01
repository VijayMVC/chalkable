using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class AbToCCMappingViewData
    {
        public Guid AcademicBenchmarkId { get; set; }
        public IList<CommonCoreStandardViewData> CommonCoreStandards { get; set; }

        public static IList<AbToCCMappingViewData> Create(IList<AbToCCMappingDetails> abToCcMapping)
        {
            return abToCcMapping.GroupBy(x=>x.AcademicBenchmarkId).Select(x=> Create(x.Key, x.Select(s=>s.Standard).ToList())).ToList();
        }

        public static AbToCCMappingViewData Create(Guid academicBenchmarkId, IList<CommonCoreStandard> standards)
        {
            return new AbToCCMappingViewData
            {
                AcademicBenchmarkId = academicBenchmarkId,
                CommonCoreStandards = CommonCoreStandardViewData.Create(standards)
            };
        }
    }
}