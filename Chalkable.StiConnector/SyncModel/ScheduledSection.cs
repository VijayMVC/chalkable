using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class ScheduledSection : SyncModel
    {
        public int DayTypeID { get; set; }
        public int SectionID { get; set; }
        public int TimeSlotID { get; set; }
        public Guid DistrictGuid { get; set; }
        //public int RoomID { get; set; }
        public override int DefaultOrder => 31;
    }
}
