using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class TimeSlot
    {
        public int TimeSlotID { get; set; }
        public int AcadSessionID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public short Sequence { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
