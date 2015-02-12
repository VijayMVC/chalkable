using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class ScheduledTimeSlotVariation
    {
        public int TimeSlotVariationId { get; set; }
        public int BellScheduleId { get; set; }
        public int TimeSlotId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short StartTime { get; set; }
        public short EndTime { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}