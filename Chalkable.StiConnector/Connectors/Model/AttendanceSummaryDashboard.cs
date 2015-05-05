namespace Chalkable.StiConnector.Connectors.Model
{
    public class AttendanceSummaryDashboard
    {
        /// <summary>
        /// Total number of daily absences, tardies, and days present for the time frame(specific term or year)
        /// </summary>
        public DailyAbsenceSummary DailyAttendance { get; set; }
        /// <summary>
        /// Total number of period absences, tardies, and days present per enrolled section for the time frame (specific term or year)
        /// </summary>
        public SectionAbsenceSummary PeriodAttendance { get; set; }
    }
}
