namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class StudentProgressReportComment
    {
        /// <summary>
        /// Comment text
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Id of the Grading Period
        /// </summary>
        public int GradingPeriodId { get; set; }

        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }
    }
}
