namespace Chalkable.API.Models.StudentAttendance
{
    public class StudentPeriodAttendance
    {
        public string ClassName { get; set; }
        public int? TeacherId { get; set; }
        public int PeriodOrder { get; set; }
        public int PeriodId { get; set; }
    }
}
