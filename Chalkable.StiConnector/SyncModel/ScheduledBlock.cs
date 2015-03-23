using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class ScheduledBlock
    {
        public int DayTypeID { get; set; }
        public int BlockID { get; set; }
        public int TimeSlotID { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
