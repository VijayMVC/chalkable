using System;

namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class AttendanceProfileReportParams
    {
        /// <summary>
        /// Ids of the absence reasons that should be included in the report
        /// </summary>       
        public int[] AbsenceReasonIds { get; set; }

        /// <summary>
        /// Id of the academic session
        /// </summary>
        public int AcadSessionId { get; set; }

        /// <summary>
        /// Date to end the report
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Determines how students should be grouped. Options include None = 0, Grade Level = 1, Homeroom = 3
        /// </summary>
        public int GroupBy { get; set; }

        /// <summary>
        /// Indicates which student identifier should print on the report.
        /// </summary>
        public int IdToPrint { get; set; }

        /// <summary>
        /// Indicates whether or not to include check in and check outs 
        /// </summary>
        public bool IncludeCheckInCheckOut { get; set; }

        /// <summary>
        /// Indicates whether or not to include absence note
        /// </summary>
        public bool IncludeNote { get; set; }

        /// <summary>
        /// Indicates whether or not to include period absences
        /// </summary>
        public bool IncludePeriodAbsences { get; set; }

        /// <summary>
        /// Indicates whether or not to include reason totals
        /// </summary>
        public bool IncludeReasonTotals { get; set; }

        /// <summary>
        /// Indicates whether or not to include unlisted student information
        /// </summary>
        public bool IncludeUnlisted { get; set; }

        /// <summary>
        /// Indicates whether or not to include withdrawn students
        /// </summary>
        public bool IncludeWithdrawnStudents { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int? SectionId { get; set; }

        /// <summary>
        /// Date to start the report
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// List of student ids to run the report for
        /// </summary>
        public int[] StudentIds { get; set; }

        /// <summary>
        /// Ids of the Terms that should be included in the report 
        /// </summary>
        public int[] TermIds { get; set; }
    }
}
