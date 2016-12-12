namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class SeatingChartReportPrams
    {
        /// <summary>
        /// Id of the academic session
        /// </summary>
        public int AcadSessionId { get; set; }

        /// <summary>
        /// Indicates whether or not to include the student photo on the report
        /// </summary>
        public bool IncludeStudentPhoto { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// Id of the term
        /// </summary>
        public int TermId { get; set; }

    }
}
