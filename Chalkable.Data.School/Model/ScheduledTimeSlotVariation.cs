using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ScheduledTimeSlotVariation
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int BellScheduleRef { get; set; }
        public int PeriodRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
    }
}