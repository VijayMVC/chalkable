using System;

namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class LunchCountReportParams
    {
        /// <summary>
        /// Id of the academic session
        /// </summary>
        public int AcadSessionId { get; set; }
        /// <summary>
        /// Date to end the report
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Indicates which student identifier should print on the report.
        /// </summary>
        public int IdToPrint { get; set; }
        public string Title { get; set; }
        public bool? IncludeGroupTotals { get; set; }
        public bool? IncludeGrandTotals { get; set; }
        public bool? IncludeStudentsOnly { get; set; }
        public bool? IncludeSummaryOnly { get; set; }
        /// <summary>
        /// Indicates the sort order of the resulting report
        /// </summary>
        public int SortOption { get; set; }
        /// <summary>
        /// Date to start the report
        /// </summary>
        public DateTime? StartDate { get; set; }
    }
}
