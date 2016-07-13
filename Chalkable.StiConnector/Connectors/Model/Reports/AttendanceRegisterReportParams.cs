namespace Chalkable.StiConnector.Connectors.Model.Reports
{
    public class AttendanceRegisterReportParams
    {
        /// <summary>
        /// Ids of the absence reasons to include in the report
        /// </summary>
        public int[] AbsenceReasonIds { get; set; }

        /// <summary>
        /// Id of the academic session
        /// </summary>
        public int AcadSessionId { get; set; }

        /// <summary>
        /// Indicates which student identifier should print on the report.
        /// </summary>
        public int IdToPrint { get; set; }

        /// <summary>
        /// Indicates whether or not to include tardies on the report
        /// </summary>
        public bool IncludeTardies { get; set; }

        /// <summary>
        /// Id of the AttendanceMonth to run the report for
        /// </summary>
        public int MonthId { get; set; }

        /// <summary>
        /// Determines how students should be grouped. Options include Both = 0, Detail = 1, Summary = 2
        /// </summary>
        public int ReportType { get; set; }

        /// <summary>
        /// Id of the section to include on the report
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// Indicates which absence reason code will be printed on the report.  When true, the district's absence reason code will be printed.  When false, the state code wil be printed
        /// </summary>
        public bool ShowLocalReasonCode { get; set; }


    }
}
