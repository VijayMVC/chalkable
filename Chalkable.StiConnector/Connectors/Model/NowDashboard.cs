using System.Collections.Generic;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class NowDashboard
    {
        public ClassRank ClassRank { get; set; }
        public int? CurrentSectionId { get; set; }
        public string CurrentAttendanceStatus { get; set; }
        public StudentDailyAbsenceSummary DailyAttendance { get; set; }
        public IEnumerable<InfractionSummary> Infractions { get; set; }
        public IEnumerable<Score> Scores { get; set; }
        public IEnumerable<StudentSectionAbsenceSummary> SectionAttendance { get; set; }
    }
}
