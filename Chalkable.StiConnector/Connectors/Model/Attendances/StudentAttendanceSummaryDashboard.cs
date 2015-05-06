using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class StudentAttendanceSummaryDashboard
    {
        /// <summary>
        /// Total number of daily absences, tardies, and days present for the time frame(specific term or year)
        /// </summary>
        public StudentDailyAbsenceSummary DailyAttendance { get; set; }

        /// <summary>
        /// Total number of period absences, tardies, and days present per enrolled section for the time frame (specific term or year)
        /// </summary>
        public IEnumerable<StudentSectionAbsenceSummary> PeriodAttendance { get; set; }
    }
}
