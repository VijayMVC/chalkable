using System;

namespace Chalkable.API.Models.StudentAttendance
{
    public class StudentDailyAttendance
    {
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public int StudentId { get; set; }
    }
}
