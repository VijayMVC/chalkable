using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class GradingPeriod
    {
        public int GradingPeriodID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int TermID { get; set; }
        public int AcadSessionID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public string SchoolAnnouncement { get; set; }
        public bool AllowGradePosting { get; set; }
    }
}