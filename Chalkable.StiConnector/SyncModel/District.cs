using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class District : SyncModel
    {
        public Guid DistrictGUID { get; set; }
        public string Name { get; set; }
        public override int DefaultOrder => 51;
    }
}
