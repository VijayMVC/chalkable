using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class GradedItemViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int GradingPeriodId { get; set; }
        public bool AlphaOnly { get; set; }
        public bool AppearsOnReportCard { get; set; }
        
        public static IList<GradedItemViewData> Create(IList<GradedItem> gradedItems)
        {
            return gradedItems.Select(x => new GradedItemViewData
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    GradingPeriodId = x.GradingPeriodRef,
                    AlphaOnly = x.AlphaOnly,
                    AppearsOnReportCard = x.AppearsOnReportCard
                }).ToList();
        } 
    }
}