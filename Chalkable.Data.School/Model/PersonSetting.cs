using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class PersonSetting
    {
        [PrimaryKeyFieldAttr]
        public int PersonRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int SchoolYearRef { get; set; }
        [PrimaryKeyFieldAttr]
        public string Key { get; set; }
        public string Value { get; set; }

        //Feed Settings
        public const string FEED_SORTING = "feedsort";
        public const string FEED_START_DATE = "feedstartdate";
        public const string FEED_END_DATE = "feedenddate";
        public const string FEED_ANNOUNCEMENT_TYPE = "feedannouncementtype";
        public const string FEED_GRADING_PERIOD_ID = "feedgradingperiodid";

        //Feed Report Settings
        public const string FEED_REPORT_START_DATE = "feedreportstartdate";
        public const string FEED_REPORT_END_DATE = "feedreportenddate";
        public const string FEED_REPORT_INCLUDE_DETAILS = "feedreportincludedetails";
        public const string FEED_REPORT_INCLUDE_HIDDEN_ATTRIBUTES = "feedreportincludehiddenattributes";
        public const string FEED_REPORT_INCLUDE_HIDDEN_ACTIVITIES = "feedreportincludehiddenactivities";
        public const string FEED_REPORT_LP_ONLY = "feedreportlponly";
        public const string FEED_REPORT_INCLUDE_ATTACHMENTS = "feedreportincludeattachments";
    }
}
