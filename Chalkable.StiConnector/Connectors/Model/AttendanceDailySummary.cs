using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class AttendanceDailySummary
    {

        /// <summary>
        /// The total sum of the absence values for all student in the section
        /// </summary>
        public decimal? Absences { get; set; }

        /// <summary>
        /// The date for which attendance data is summarized
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The number of times students were present in a specific section
        /// </summary>
        public decimal? Presents { get; set; }

        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// The number of times students were tardy for the section
        /// </summary>
        public int? Tardies { get; set; }
    }
}
