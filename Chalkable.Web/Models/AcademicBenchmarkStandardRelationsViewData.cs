using System.Collections.Generic;
using Chalkable.BusinessLogic.Model.AcademicBenchmarkStandard;

namespace Chalkable.Web.Models
{
    public class AcademicBenchmarkStandardRelationsViewData : AcademicBenchmarkStandardViewData
    {
        public AcademicBenchmarkStandard CurrentStandard { get; set; }
        public IList<AcademicBenchmarkStandard> Origin { get; set; }
        public IList<AcademicBenchmarkStandard> Derivative { get; set; }
        public IList<AcademicBenchmarkStandard> RelatedDerivative { get; set; }

        public static AcademicBenchmarkStandardRelationsViewData Create(AcademicBenchmarkStandardRelations academickBenchmarkRelatedStandard)
        {
            return new AcademicBenchmarkStandardRelationsViewData
            {
                CurrentStandard = academickBenchmarkRelatedStandard.CurrentStandard,
                Origin = academickBenchmarkRelatedStandard.Origin,
                Derivative = academickBenchmarkRelatedStandard.Derivative,
                RelatedDerivative = academickBenchmarkRelatedStandard.RelatedDerivative
            };
        }
    }
}