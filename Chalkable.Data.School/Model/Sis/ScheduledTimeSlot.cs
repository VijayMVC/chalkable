﻿using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class ScheduledTimeSlot
    {
        [PrimaryKeyFieldAttr]
        public int BellScheduleRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int PeriodRef { get; set; }
        public int? StartTime { get; set; }
        public int? EndTime { get; set; }
        public string Description { get; set; }
        public bool IsDailyAttendancePeriod { get; set; }
    }
}