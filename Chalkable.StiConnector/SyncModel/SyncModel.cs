namespace Chalkable.StiConnector.SyncModel
{
    public abstract class SyncModel
    {
        public long? SYS_CHANGE_VERSION { get; set; }
        public long? SYS_CHANGE_CREATION_VERSION { get; set; }

        public abstract int DefaultOrder { get; }

        public SyncModel Clone() => (SyncModel)MemberwiseClone();
    }
}