using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class AttendanceMonth : SyncModel
    {
        public int AttendanceMonthID { get; set; }
        public int AcadSessionID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsLockedAttendance { get; set; }
        public bool IsLockedDiscipline { get; set; }
        public override int DefaultOrder => 44;
    }
}
