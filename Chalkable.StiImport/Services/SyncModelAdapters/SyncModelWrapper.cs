using System;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class SyncModelWrapper : IComparable
    {
        public long? ChangeVersion { get; set; }
        public PersistOperationType OperationType { get; set; }
        public SyncModel Model { get; set; }
        public int CompareTo(object obj)
        {
            var other = (SyncModelWrapper) obj;
            if (ChangeVersion > other.ChangeVersion)
                return 1;
            if (ChangeVersion < other.ChangeVersion)
                return -1;
            if (OperationType > other.OperationType)
                return 1;
            if (OperationType < other.OperationType)
                return -1;
            return string.Compare(Model.GetType().Name, other.Model.GetType().Name);
        }
    }
}