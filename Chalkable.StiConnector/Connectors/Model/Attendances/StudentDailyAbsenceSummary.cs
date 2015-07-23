namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class StudentDailyAbsenceSummary
    {
        /// <summary>
        /// The total sum of the absence values for student
        /// </summary>
        public decimal? Absences { get; set; }
        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        /// The number of times a student was tardy
        /// </summary>
        public int? Tardies { get; set; }
        /// <summary>
        /// The number of times a student was present
        /// </summary>
        public decimal? Presents { get; set; }
    }
}
