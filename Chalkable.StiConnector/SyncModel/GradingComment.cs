namespace Chalkable.StiConnector.SyncModel
{
    public class GradingComment : SyncModel
    {
        public int GradingCommentID { get; set; }
        public int SchoolID { get; set; }
        public string Code { get; set; }
        public string Comment { get; set; }
        public override int DefaultOrder => 42;
    }
}