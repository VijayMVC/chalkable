using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model.Reports.ReportCards
{
    public class ReportCardOptions
    {
        /// <summary>
        /// Ids of the absence reasons to include on the report.  When null or blank, all absence reasons will be included.
        /// </summary>
        public List<int> AbsenceReasonIds { get; set; }

        /// <summary>
        /// Id of the academic session
        /// </summary>
        public int AcadSessionId { get; set; }

        /// <summary>
        /// Id of the grading period
        /// </summary>
        public int GradingPeriodId { get; set; }

        ///<summary>
        ///Indicates whether or not to include attendance information is included on the report card
        ///</summary>
        public bool IncludeAttendance { get; set; }

        /// <summary>
        /// Indicates where or not graing comments, one or more per graded item should be included on the report card
        /// </summary>
        public bool IncludeComments { get; set; }

        /// <summary>
        /// Indicates whether or not notes for each grading period should be included
        /// </summary>
        public bool IncludeGradingPeriodNotes { get; set; }

        /// <summary>
        /// Indicates whether or not to include grading scales
        /// </summary>
        public bool IncludeGradingScales { get; set; }

        /// <summary>
        /// Indicates whether or not to includes demer
        /// </summary>
        public bool IncludeMeritDemerit { get; set; }

        /// <summary>
        /// Indicates whether or not to include the student's promotion status
        /// </summary>
        public bool IncludePromotionStatus { get; set; }

        /// <summary>
        /// Indicates whether or not to include standards
        /// </summary>
        public bool IncludeStandards { get; set; }

        /// <summary>
        /// When true, withdrawn students are included in the results
        /// </summary>
        public bool IncludeWithdrawnStudents { get; set; }

        /// <summary>
        /// Indicates whether the report card should include year to date info.  True- Include info for previous grading periods.  False - Only include current grading period
        /// </summary>
        public bool IncludeYearToDateInformation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// Array of student ids
        /// </summary>
        public List<int> StudentIds { get; set; }
    }
}
