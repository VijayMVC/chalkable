using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class PersonEthnicity : SyncModel
    {
        public int PersonID { get; set; }
        public short EthnicityID { get; set; }
        public byte Percentage { get; set; }
        public bool IsPrimary { get; set; }
        public Guid DistrictGuid { get; set; }
        public override int DefaultOrder => 7;
    }
}
