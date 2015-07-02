namespace Chalkable.StiConnector.Connectors.Model
{
    public class ActivityAssignedAttribute
    {
        /// <summary>
        /// The Id of the activity
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        /// The activity attachment
        /// </summary>
        public StiAttachment Attachment { get; set; }

        /// <summary>
        /// The id of the Attribute. (type)
        /// </summary>
        public short AttributeId { get; set; }

        /// <summary>
        /// The id of the ActivityAssignedAttribute. 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the attribute
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The text of the attribute
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Indicates whether or not the attribute should be visible to students in the home portal
        /// </summary>
        public bool VisibleInHomePortal { get; set; }
    }
}
