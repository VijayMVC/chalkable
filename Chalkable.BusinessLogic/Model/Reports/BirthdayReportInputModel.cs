using System;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class BirthdayReportInputModel : BaseReportInputModel
    {
        /// <summary>
        /// Id of the academic session
        /// </summary>
        public int AcadSessionId { get; set; }

        /// <summary>
        /// (optional) The end date of the report.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// (optional) The end month of the report
        /// </summary>
        public int? EndMonth { get; set; }

        /// <summary>
        /// Determines how to group students in the report. BlankColumn = 0 ( No Group), MonthNumber = 1 (Birthday Month), Sequence = 2 (Grade Level), HomeroomName = 3 (Homeroom)
        /// </summary>
        public int GroupBy { get; set; }

        //Indicates whether or not to include student photos
        public bool IncludePhoto { get; set; }

        /// <summary>
        /// Indicates whether or not to include withdrawn students
        /// </summary>
        public bool IncludeWithdrawn { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int? SectionId { get; set; }

        /// <summary>
        /// (optional)  The start date of the report
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// (optional)  The start month of the report.  
        /// </summary>
        public int? StartMonth { get; set; }
    }
}
