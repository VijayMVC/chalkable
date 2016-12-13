using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model.StudentPanorama
{
    public class StudentInfraction
    {

        /// <summary>
        /// The number of demerits earned due to the infraction.
        /// </summary>
        public byte DemeritsEarned { get; set; }

        /// <summary>
        /// The id of the primary disposition. Null if there is no primary disposition.
        /// </summary>
        public short? DispositionId { get; set; }

        /// <summary>
        /// The name of the primary disposition. Null if there is no primary disposition.
        /// </summary>
        public string DispositionName { get; set; }

        /// <summary>
        /// The note of the primary disposition. Null if there is no primary disposition.
        /// </summary>
        public string DispositionNote { get; set; }

        /// <summary>
        /// The start date and time of the primary disposition. Null if there is no primary disposition.
        /// </summary>
        public DateTime? DispositionStartDateTime { get; set; }

        /// <summary>
        /// The id of the primary infraction.
        /// </summary>
        public short InfractionId { get; set; }

        /// <summary>
        /// The name of the primary infraction.
        /// </summary>
        public string InfractionName { get; set; }

        /// <summary>
        /// The state code of the primary infraction.
        /// </summary>
        public string InfractionStateCode { get; set; }

        /// <summary>
        /// The date of the disciplinary occurrence.
        /// </summary>
        public DateTime OccurrenceDate { get; set; }

        /// <summary>
        /// The id of the disciplinary occurrence.
        /// </summary>
        public int OccurrenceId { get; set; }

        /// <summary>
        /// The id of the student related to the infraction.
        /// </summary>
        public int StudentId { get; set; }

    }
}
