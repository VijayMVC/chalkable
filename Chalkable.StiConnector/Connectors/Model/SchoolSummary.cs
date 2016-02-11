using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class SchoolSummary
    {

        /// <summary>
        /// Total student daily absence values for today
        /// </summary>
        public decimal AbsenceCount { get; set; }

        /// <summary>
        /// Average of every student average for every class for the current or closest grading period
        /// </summary>
        public decimal Average { get; set; }

        /// <summary>
        /// Total number of discipline infractions that were entered today
        /// </summary>
        public int DisciplineCount { get; set; }

        /// <summary>
        /// The total number of students enrolled  for the current day in the school
        /// </summary>
        public int EnrollmentCount { get; set; }

        /// <summary>
        /// The id of the school
        /// </summary>
        public int SchoolId { get; set; }

        /// <summary>
        /// The name of the school
        /// </summary>
        public string SchoolName { get; set; }

    }
}
