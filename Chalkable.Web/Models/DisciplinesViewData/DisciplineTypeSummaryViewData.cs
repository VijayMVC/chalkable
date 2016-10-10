using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.Web.Models.DisciplinesViewData
{
    public class DisciplineTypeSummaryViewData
    {
        public DisciplineTypeViewData Type { get; set; }
        public int Total { get; set; }

        public static IList<DisciplineTypeSummaryViewData> Create(IList<InfractionSummaryInfo> disciplineTotalPerTypes)
        {
            return disciplineTotalPerTypes.Select(x => new DisciplineTypeSummaryViewData
            {
                Type = DisciplineTypeViewData.Create(x.Infraction),
                Total = x.Occurrences
            }).ToList();
        }
    }

}