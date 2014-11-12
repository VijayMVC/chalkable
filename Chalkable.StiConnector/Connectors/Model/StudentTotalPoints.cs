using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StudentTotalPoints
    {
        /// <summary>
        /// Id of the grading period
        /// </summary>
        public int GradingPeriodId { get; set; }

        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// Sum of activity points the student earned for the grading period
        /// </summary>
        public decimal TotalPointsEarned { get; set; }

        /// <summary>
        /// Sum of the activity maxiumum possible points for activities the student recieved grades for during the grading period
        /// </summary>
        public decimal TotalPointsPossible { get; set; }

    }
}
