namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class StudentSectionAbsenceSummary
    {
        /// <summary>
        /// The total sum of the absence values for student in a specific section
        /// </summary>
        public decimal? Absences { get; set; }
        /// <summary>
        /// Id of the section
        /// </summary>
        public int SectionId { get; set; }
        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        /// The number of times a student was tardy for the section
        /// </summary>
        public int? Tardies { get; set; }
        /// <summary>
        /// The number of times a student was present in a specific section
        /// </summary>
        public decimal? Presents { get; set; }
    }
}
