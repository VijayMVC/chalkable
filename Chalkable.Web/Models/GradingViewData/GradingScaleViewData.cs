using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.GradingViewData
{
    public class GradingScaleViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? HomeGradeToDisplay { get; set; }

        public static IList<GradingScaleViewData> Create(IList<GradingScale> gradingScales)
        {
            return gradingScales.Select(x => new GradingScaleViewData
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    HomeGradeToDisplay = x.HomeGradeToDisplay
                }).ToList();
        }
    }
}