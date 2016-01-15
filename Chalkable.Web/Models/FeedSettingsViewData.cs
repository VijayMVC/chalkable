using System;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School.Announcements;

namespace Chalkable.Web.Models
{
    public class FeedSettingsViewData
    {
        public int? AnnouncementType { get; set; }
        public int? SortType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? GradingPeriodId { get; set; }
        public bool ToSet { get; set; }

        public static FeedSettingsViewData Create(FeedSettingsInfo feedSett)
        {
            return new FeedSettingsViewData
            {
                AnnouncementType = feedSett.AnnouncementType,
                FromDate = feedSett.GradingPeriodId.HasValue || feedSett.AnyDate ? null : feedSett.FromDate,
                GradingPeriodId = feedSett.GradingPeriodId,
                SortType = feedSett.SortType,
                ToDate = feedSett.GradingPeriodId.HasValue || feedSett.AnyDate ? null : feedSett.ToDate,
            };
        }
    }
}