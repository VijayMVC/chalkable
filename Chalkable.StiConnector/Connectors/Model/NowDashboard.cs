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
    public class ClassRank
    {
        /// <summary>
        /// Academic Year of the class rank
        /// </summary>
        public short AcadYear { get; set; }
        /// <summary>
        /// Number of students in the class rank
        /// </summary>
        public short? ClassSize { get; set; }
        /// <summary>
        /// Class rank for the student
        /// </summary>
        public short? Rank { get; set; }
        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        /// Id of the Inow term.  If the transcript term doesn't map back to an Inow term, this will be null.
        /// </summary>
        public int? TermId { get; set; }
        /// <summary>
        /// Name of the term
        /// </summary>
        public string TermName { get; set; }
    }
    public class InfractionSummary
    {
        /// <summary>
        /// The number of times the discipline infraction occurred for the student
        /// </summary>
        public int Occurrences { get; set; }
        /// <summary>
        /// Id of the discipline infraction
        /// </summary>
        public int InfractionId { get; set; }
        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
    }
}
