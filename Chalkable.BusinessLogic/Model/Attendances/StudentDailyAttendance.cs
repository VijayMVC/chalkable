using System;

namespace Chalkable.BusinessLogic.Model.Attendances
{
    public class StudentDailyAttendance : BaseAttendance
    {
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public int StudentId { get; set; }
    }
}
