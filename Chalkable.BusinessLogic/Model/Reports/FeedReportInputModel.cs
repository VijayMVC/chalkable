namespace Chalkable.BusinessLogic.Model.Reports
{
    public class FeedReportInputModel
    {
        public int? ClassId { get; set; }
        public bool? Complete { get; set; }
        public FeedReportSettingsInfo Settings { get; set; }
        public int? AnnouncementType { get; set; }
        public string DefaultPath { get; set; }
        public ReportingFormat? FormatTyped
        {
            get { return (ReportingFormat?)Format; }
            set { Format = (int?)value; }
        }
        public int? Format { get; set; }
    }
}