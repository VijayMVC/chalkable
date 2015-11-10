using System;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class FeedReportInputModel
    {
        public int? ClassId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool LessonPlanOnly { get; set; }
        public bool IncludeAttachments { get; set; }
        public bool IncludeDetails { get; set; }
        public bool IncludeHiddenAttributes { get; set; }
        public bool IncludeHiddentActivities { get; set; }

        public ReportingFormat? FormatTyped
        {
            get { return (ReportingFormat?)Format; }
            set { Format = (int?)value; }
        }
        public int? Format { get; set; }
    }
}