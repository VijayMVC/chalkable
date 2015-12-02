using System;

namespace Chalkable.BusinessLogic.Model
{
    public class FeedSettingsInfo
    {
        public int? AnnouncementType { get; set; }
        public bool? SortType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? GradingPeriodId { get; set; }
        public bool ToSet { get; set; }
    }
}
