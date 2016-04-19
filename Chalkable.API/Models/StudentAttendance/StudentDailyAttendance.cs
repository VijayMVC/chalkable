using System;

namespace Chalkable.API.Models.StudentAttendance
{
    public class StudentDailyAttendance
    {
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public int StudentId { get; set; }
        public int? AttendanceReasonId { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public bool IsAbsent { get; set; }
        public bool IsLate { get; set; }
        public bool IsAbsentOrLate { get; set; }
        public bool IsExcused { get; set; }
    }
}
