using System;
using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model.Reports.ReportCards
{
    public class ReportCard
    {
        public ReportCard()
        {
            this.School = new School();
            this.GradingPeriod = new GradingPeriod();
            this.GradingScales = new List<GradingScale>();
            this.Students = new List<Student>();
        }
        public int AcadYear { get; set; }
        public string AcadSessionName { get; set; }
        /// <summary>
        /// Information about the school
        /// </summary>
        public School School { get; set; }
        /// <summary>
        /// Information about the grading period
        /// </summary>
        public GradingPeriod GradingPeriod { get; set; }
        public List<GradingScale> GradingScales { get; set; }
        /// <summary>
        /// A list of 
        /// </summary>
        public List<Student> Students { get; set; }
        public string Message { get; set; }
    }
}
