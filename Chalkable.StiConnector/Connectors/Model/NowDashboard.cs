using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class NowDashboard
    {
        public ClassRank ClassRank { get; set; }
        public DailyAbsenceSummary DailyAttendance { get; set; }
        public IEnumerable<InfractionSummary> Infractions { get; set; }
        public IEnumerable<Score> Scores { get; set; }
        public IEnumerable<SectionAbsenceSummary> SectionAttendance { get; set; }
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

    public class DailyAbsenceSummary
    {
        /// <summary>
        /// The total sum of the absence values for student
        /// </summary>
        public decimal? Absences { get; set; }
        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        /// The number of times a student was tardy
        /// </summary>
        public int? Tardies { get; set; }

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

    public class SectionAbsenceSummary
    {
        /// <summary>
        /// The total sum of the absence values for student in a specific section
        /// </summary>
        public decimal? Absences { get; set; }
        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }
        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        /// The number of times a student was tardy for the section
        /// </summary>
        public int? Tardies { get; set; }
    }

}
