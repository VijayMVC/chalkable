namespace Chalkable.StiConnector.Connectors.Model
{
    public class InfractionSummary
    {
        /// <summary>
        /// The number of times the discipline infraction occurred for the student
        /// </summary>
        public int Occurrences { get; set; }
        /// <summary>
        /// Id of the discipline infraction
        /// </summary>
        public int InfractionId { get; set; }
        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
    }
}
