namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class AttendanceRegisterReportParams
    {
        public int[] AbsenceReasonIds { get; set; }
        public int AcadSessionId { get; set; }
        public int IdToPrint { get; set; }
        public bool IncludeTardies { get; set; }
        public int MonthId { get; set; }
        public int ReportType { get; set; }
        public int SectionId { get; set; }
        public bool ShowLocalReasonCode { get; set; }
        public int UserId { get; set; }
    }
}
