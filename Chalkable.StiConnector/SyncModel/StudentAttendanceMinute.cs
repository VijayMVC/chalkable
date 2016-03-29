using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StudentAttendanceMinute
    {
        public int StudentID { get; set; }
        public int AcadSessionID { get; set; }
        public DateTime Date { get; set; }
        public short MinutesExpected { get; set; }
        public short MinutesAbsent { get; set; }
        public short MinutesPresent { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
