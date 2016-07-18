using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;

namespace Chalkable.Web.Models.AcademicBenchmarksViewData
{
    public class StandardRelationsViewData
    {
        public StandardViewData CurrentStandard { get; set; }
        public IList<StandardViewData> Origins { get; set; }
        public IList<StandardViewData> Derivatives { get; set; }
        public IList<StandardViewData> RelatedDerivatives { get; set; }
        public static StandardRelationsViewData Create(StandardRelations standardRelations)
        {
            return new StandardRelationsViewData
            {
                CurrentStandard = StandardViewData.Create(standardRelations.CurrentStandard),
                Origins = standardRelations.Origins?.Select(StandardViewData.Create).ToList(),
                Derivatives = standardRelations.Derivatives?.Select(StandardViewData.Create).ToList(),
                RelatedDerivatives = standardRelations.RelatedDerivatives?.Select(StandardViewData.Create).ToList()
            };
        }
    }
}