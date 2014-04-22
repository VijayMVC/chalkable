using System;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class WorksheetReportParams
    {
        public int AcadSessionId { get; set; }
        public int[] ActivityIds { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Title3 { get; set; }
        public string Title4 { get; set; }
        public string Title5 { get; set; }
        public DateTime EndDate { get; set; }
        public int GradingPeriodId { get; set; }
        public string Header { get; set; }
        public int IdToPrint { get; set; }
        public bool PrintAverage { get; set; }
        public bool PrintLetterGrade { get; set; }
        public bool PrintScores { get; set; }
        public bool PrintStudent { get; set; }
        public int SectionId { get; set; }
        public int? StaffId { get; set; }
        public DateTime StartDate { get; set; }
        public int[] StudentIds { get; set; }
    }
}
