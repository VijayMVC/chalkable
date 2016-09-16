using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.PanoramaSettings;

namespace Chalkable.Web.Models.Settings
{
    public class StandardizedTestFilterViewData
    {
        public int StandardizedTestId { get; set; }
        public int ComponentId { get; set; }
        public int ScoreTypeId { get; set; }
        public static StandardizedTestFilterViewData Create(StandardizedTestFilter filter)
        {
            return new StandardizedTestFilterViewData
            {
                StandardizedTestId = filter.StandardizedTestId,
                ComponentId = filter.ComponentId,
                ScoreTypeId = filter.ScoreTypeId
            };
        }

        public static IList<StandardizedTestFilterViewData> Create(IList<StandardizedTestFilter> filters)
        {
            return filters?.Select(Create).ToList();
        } 
    }
}