using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class AttendanceCalendar
    {
        public int AttendanceCalendarID { get; set; }
        public int AcadSessionID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
