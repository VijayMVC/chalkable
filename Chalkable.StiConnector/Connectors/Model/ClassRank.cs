namespace Chalkable.StiConnector.Connectors.Model
{
    public class ClassRank
    {
        /// <summary>
        /// Academic Year of the class rank
        /// </summary>
        public short AcadYear { get; set; }
        /// <summary>
        /// Number of students in the class rank
        /// </summary>
        public short? ClassSize { get; set; }
        /// <summary>
        /// Class rank for the student
        /// </summary>
        public short? Rank { get; set; }
        /// <summary>
        /// Id of the student
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        /// Id of the Inow term.  If the transcript term doesn't map back to an Inow term, this will be null.
        /// </summary>
        public int? TermId { get; set; }
        /// <summary>
        /// Name of the term
        /// </summary>
        public string TermName { get; set; }
    }
}
