using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard;

namespace Chalkable.Web.Models.ABStandardsViewData
{
    public class AcademicBenchmarkStandardRelationsViewData
    {
        public AcademicBenchmarkStandardViewData CurrentStandard { get; set; }
        public IList<AcademicBenchmarkStandardViewData> Origins { get; set; }
        public IList<AcademicBenchmarkStandardViewData> Derivatives { get; set; }
        public IList<AcademicBenchmarkStandardViewData> RelatedDerivatives { get; set; }
        public static AcademicBenchmarkStandardRelationsViewData Create(AcademicBenchmarkStandardRelations standardRelations)
        {
            return new AcademicBenchmarkStandardRelationsViewData
            {
                CurrentStandard = AcademicBenchmarkStandardViewData.Create(standardRelations.CurrentStandard),
                Origins = standardRelations.Origins.Select(AcademicBenchmarkStandardViewData.Create).ToList(),
                Derivatives = standardRelations.Derivatives.Select(AcademicBenchmarkStandardViewData.Create).ToList(),
                RelatedDerivatives = standardRelations.RelatedDerivatives.Select(AcademicBenchmarkStandardViewData.Create).ToList()
            };
        }
    }
}