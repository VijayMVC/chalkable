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
            if (!ChangeVersion.HasValue && other.ChangeVersion.HasValue)
                return 1;
            if (ChangeVersion.HasValue && !other.ChangeVersion.HasValue)
                return -1;
            if (OperationType > other.OperationType)
                return 1;
            if (OperationType < other.OperationType)
                return -1;
            if (OperationType == PersistOperationType.Delete)
            {
                if (Model.DefaultOrder > other.Model.DefaultOrder)
                    return -1;
                if (Model.DefaultOrder < other.Model.DefaultOrder)
                    return 1;
            }
            else
            {
                if (Model.DefaultOrder > other.Model.DefaultOrder)
                    return 1;
                if (Model.DefaultOrder < other.Model.DefaultOrder)
                    return -1;
            }
            
            return string.Compare(Model.GetType().Name, other.Model.GetType().Name);
        }

        public static SyncModelWrapper Create(long? changeVersion, PersistOperationType operationType, SyncModel model)
        {
            return new SyncModelWrapper
            {
                ChangeVersion = changeVersion,
                Model = model,
                OperationType = operationType
            };
        }
    }
}