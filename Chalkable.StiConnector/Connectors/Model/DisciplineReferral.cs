using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class DisciplineReferral
    {

        /// <summary>
        /// Date of the discipline referral
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Id of the discipline referral.  In INow database, this is the OccurrenceId
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// List of infractions that occurred
        /// </summary>
        public IEnumerable<Infraction> Infractions { get; set; }

        /// <summary>
        /// A note about the discipline referral.  This is the occurrence note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// The id of the section 
        /// </summary>
        public int? SectionId { get; set; }

        /// <summary>
        /// The id of the student
        /// </summary>
        public int StudentId { get; set; }

    }

    public class Infraction
    {

        /// <summary>
        /// Id of the infraction
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// Name of the infraction
        /// </summary>
        public string Name { get; set; }
    }
}
