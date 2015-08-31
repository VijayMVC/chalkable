namespace Chalkable.StiConnector.SyncModel
{
    public class SpEdStatus : SyncModel
    {
        public int SpEdStatusID { get; set; }
        public string Name { get; set; }
        public override int DefaultOrder => 0;
    }
}