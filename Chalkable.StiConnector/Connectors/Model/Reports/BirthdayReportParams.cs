using System;

namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class BirthdayReportParams
    {
        public int AcadSessionId { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public int? StartMonth { get; set; }
        public int? EndMonth { get; set; }
        public int GroupBy { get; set; }
        public string Header { get; set; }
        public bool IncludePhoto { get; set; }
        public bool IncludeWithdrawn { get; set; }
        public int? SectionId { get; set; }
        public int? StudentFilterId { get; set; }
        public int? UserId { get; set; }
    }
}
