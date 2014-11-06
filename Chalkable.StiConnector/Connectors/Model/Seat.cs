
namespace Chalkable.StiConnector.Connectors.Model
{
    public class Seat
    {
        /// <summary>
        /// The column position where the student sits on the seating chart.  If the value is null, the student has not been placed on the seating chart yet.
        /// </summary>
        public byte? Column { get; set; }

        /// <summary>
        /// The row position where the student sits on the seating chart.  If the value is null, the student has not been placed on the seating chart yet.
        /// </summary>
        public byte? Row { get; set; }

        /// <summary>
        /// The Id of the student
        /// </summary>
        public int StudentId { get; set; }
    }
}
