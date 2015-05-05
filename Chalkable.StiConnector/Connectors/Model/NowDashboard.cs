using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class NowDashboard
    {
        public ClassRank ClassRank { get; set; }
        public int? CurrentSectionId { get; set; }
        public string CurrentAttendanceStatus { get; set; }
        public DailyAbsenceSummary DailyAttendance { get; set; }
        public IEnumerable<InfractionSummary> Infractions { get; set; }
        public IEnumerable<Score> Scores { get; set; }
        public IEnumerable<SectionAbsenceSummary> SectionAttendance { get; set; }
    }
}
