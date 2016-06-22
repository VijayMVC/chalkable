using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model.Attendances
{
    public class StudentAttendanceDetailsInfo
    {
        public StudentDetails Student { get; set; }
        public StudentAttendanceSummary AttendanceSummary { get; set; }
        public StudentDateAttendance StudentDateAttendance { get; set; }
    }
}
