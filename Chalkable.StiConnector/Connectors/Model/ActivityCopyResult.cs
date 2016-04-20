namespace Chalkable.StiConnector.Connectors.Model
{
    public class ActivityCopyResult
    {
        /// <summary>
        /// The id of the section that the activity is copied to
        /// </summary>
        public int CopyToSectionId { get; set; }

        /// <summary>
        /// The id of the activity to copy
        /// </summary>
        public int SourceActivityId { get; set; }

        /// <summary>
        /// The of the newly created activity
        /// </summary>
        public int? NewActivityId { get; set; }
    }
}
