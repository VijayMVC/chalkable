namespace Chalkable.StiConnector.SyncModel
{
    public class AppSetting : SyncModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public override int DefaultOrder => 55;
    }
}