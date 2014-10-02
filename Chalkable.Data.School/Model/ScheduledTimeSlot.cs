using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ScheduledTimeSlot
    {
        [PrimaryKeyFieldAttr]
        public int BellScheduleID { get; set; }
        [PrimaryKeyFieldAttr]
        public int TimeSlotID { get; set; }
        public int? StartTime { get; set; }
        public int? EndTime { get; set; }
        public string Description { get; set; }
        public bool IsDailyAttendancePeriod { get; set; }
    }
}