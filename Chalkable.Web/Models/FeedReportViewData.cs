using System;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.Web.Models
{
    public class FeedReportViewData
    {
        public DateTime StartDate { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime MaxDate { get; set; }
        public bool IncludeDetails { get; set; }
        public bool IncludeHiddenActivities { get; set; }
        public bool IncludeHiddenAttributes { get; set; }
        public bool IncludeAttachments { get; set; }
        public int? AnnouncementType { get; set; }
        public bool LessonPlanOnly { get; set; }
        public bool EditableLPOption { get; set; }

        public static FeedReportViewData Create(FeedReportSettingsInfo feedReportSettings,
            FeedSettingsInfo feedSettingsInfo)
        {
            var res = new FeedReportViewData();

            if (!feedReportSettings.StartDate.HasValue)
                res.StartDate = feedSettingsInfo.FromDate.Value;

            return res;
        }
    }
}