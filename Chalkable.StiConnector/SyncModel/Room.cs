using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class Room
    {
        public int RoomID { get; set; }
        public int SchoolID { get; set; }
        public string RoomNumber { get; set; }
        public string Description { get; set; }
        public short? StudentCapacity { get; set; }
        public int? LocationID { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
