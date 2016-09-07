using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class GradingDetailsDashboard
    {

        public int GradingPeriodId { get; set; }

        /// <summary>
        /// A list of the student's scores for the grading period
        /// </summary>
        public IEnumerable<Score> Scores { get; set; }

        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
    }
}