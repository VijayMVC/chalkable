using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class GradingSummaryDashboard
    {

        /// <summary>
        /// A list of averages for the student in each section
        /// </summary>
        public IEnumerable<StudentAverage> Averages { get; set; }

        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
    }
}
