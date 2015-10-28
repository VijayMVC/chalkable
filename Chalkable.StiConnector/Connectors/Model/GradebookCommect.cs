namespace Chalkable.StiConnector.Connectors.Model
{
    public class GradebookCommect
    {
        /// <summary>
        /// The text of the comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The id of the SectionComment
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Teachers should only be able to edit comments that aren't system comments. 
        /// Grading comments are system generated. They don't have a section comment id.
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// The id of the Teacher
        /// </summary>
        public int? TeacherId { get; set; }
    }
}
