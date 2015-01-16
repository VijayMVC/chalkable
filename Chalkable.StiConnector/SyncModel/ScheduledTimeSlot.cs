using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class ScheduledTimeSlot
    {
        public int BellScheduleID { get; set; }
        public int TimeSlotID { get; set; }
        public short? StartTime { get; set; }
        public short? EndTime { get; set; }
        public string Description { get; set; }
        public bool IsDailyAttendancePeriod { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
