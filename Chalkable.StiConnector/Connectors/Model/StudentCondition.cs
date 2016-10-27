using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StudentCondition
    {
        /// <summary>
        /// Description of the student health condition
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The id of the student health condition.  This is the StudentHealthConditionId not the HealthConditionId
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Indicates whether or not to show the alert icon for this health condition
        /// </summary>
        public bool IsAlert { get; set; }

        /// <summary>
        /// Name of the medication Type
        /// </summary>
        public string MedicationType { get; set; }

        /// <summary>
        /// Name of the health condition
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of treatment for the health condition
        /// </summary>
        public string Treatment { get; set; }

    }
}
