using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class SeatingChart
    {
        /// <summary>
        /// The number of columns in the seating chart
        /// </summary>
        public byte Columns { get; set; }

        /// <summary>
        /// The number of rows in the seating chart
        /// </summary>
        public byte Rows { get; set; }

        /// <summary>
        /// The seat positions for each student.
        /// </summary>
        public IEnumerable<Seat> Seats { get; set; }

        /// <summary>
        /// The Id of the section
        /// </summary>
        public int SectionId { get; set; }
    }
}
