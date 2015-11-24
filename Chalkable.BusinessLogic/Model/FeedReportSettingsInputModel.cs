using System;

namespace Chalkable.BusinessLogic.Model
{
    public class FeedReportSettingsInputModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeDetails { get; set; }
        public bool IncludeHiddenActivities { get; set; }
        public bool IncludeHiddenAttributes { get; set; }
        public bool IncludeAttachments { get; set; }
        public bool LpOnly{ get; set; }
}
}