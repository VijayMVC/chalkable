using System;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Web.Models
{
    public class FeedReportSettingsViewData
    {
        public DateTime StartDate { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime MaxDate { get; set; }
        public bool IncludeDetails { get; set; }
        public bool IncludeHiddenActivities { get; set; }
        public bool IncludeHiddenAttributes { get; set; }
        public bool IncludeAttachments { get; set; }
        public bool LessonPlanOnly { get; set; }
        public bool EditableLpOption { get; set; }
        public int? AnnouncementType { get; set; }

        public static FeedReportSettingsViewData Create(FeedReportSettingsInputModel feedReportSettings,
            FeedSettings feedSettings)
        {
            var res = new FeedReportSettingsViewData
            {
                AnnouncementType = feedSettings.AnnouncementType,
                EditableLpOption = !feedSettings.AnnouncementType.HasValue,
                IncludeAttachments = feedReportSettings.IncludeAttachments,
                IncludeDetails = feedReportSettings.IncludeDetails,
                IncludeHiddenActivities = feedReportSettings.IncludeHiddenActivities,
                IncludeHiddenAttributes = feedReportSettings.IncludeHiddenAttributes,
                LessonPlanOnly =
                    ((AnnouncementType?) feedSettings.AnnouncementType) ==
                    Data.School.Model.Announcements.AnnouncementType.LessonPlan,
                MinDate = feedSettings.FromDate ?? DateTime.MinValue,
                MaxDate = feedSettings.ToDate ?? DateTime.MaxValue
            };

            res.StartDate = feedReportSettings.StartDate ?? res.MinDate;
            res.EndDate = feedReportSettings.EndDate ?? res.MaxDate;

            return res;
        }
    }
}