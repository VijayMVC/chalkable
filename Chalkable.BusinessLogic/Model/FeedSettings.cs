using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Model
{
    public class FeedSettings
    {
        public int? AnnouncementType { get; set; }
        public bool? SortType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? GradingPeriodId { get; set; }
        public bool ToSet { get; set; }
    }
}
