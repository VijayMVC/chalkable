using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StudentExplorerDashboard
    {
        /// <summary>
        /// List of activities that are due the next 7 days for the student.  These are activities that haven't been scored.
        /// </summary>
        public IEnumerable<Activity> Activities { get; set; }
        /// <summary>
        /// List of student averages for the grading period
        /// </summary>
        public IEnumerable<StudentAverage> Averages { get; set; }
        /// <summary>
        /// Id of the GradingPeriod
        /// </summary>
        public int? GradingPeriodId { get; set; }
        /// <summary>
        /// List of standard averages for the student for the grading period.  
        /// </summary>
        public IEnumerable<StandardScore> Standards { get; set; }
    }
}
