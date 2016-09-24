namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class SectionAttendanceSummaryDashboard
    {
        /// <summary>
        /// Total number of period absences, tardies, and days present for the section for the time frame (specific term or year)
        /// </summary>
        public SectionAbsenceSummary PeriodAttendance { get; set; }
    }
}
