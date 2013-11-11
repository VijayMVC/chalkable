using System;

namespace Chalkable.Data.School.Model
{
    public class ClassAttendance
    {
        public int PersonRef { get; set; }
        public int ClassRef { get; set; }
        public int? AttendanceReasonRef { get; set; }
        public string Description { get; set; }
        public AttendanceTypeEnum Type { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }
    }


    public class ClassAttendanceDetails : ClassAttendance
    {
        public Person Student { get; set; }
        public Class Class { get; set; }
        
    }

    public class AttendanceTotalPerType
    {
        public const string TOTAL_FIELD = "Total";
        public int Total { get; set; }
        public const string ATTENDANCE_TYPE_FIELD = "AttendanceType";
        public AttendanceTypeEnum AttendanceType { get; set; }     
    }
    public class PersonAttendanceTotalPerType : AttendanceTotalPerType
    {
        public int PersonId { get; set; }
    }
    public class StudentAbsentFromPeriod
    {
        public DateTime Date { get; set; }
        public int PersonId { get; set; }
        public int PeriodOrder { get; set; }
    }
    public class StudentCountAbsentFromPeriod
    {
        public DateTime Date { get; set; }
        public int StudentCount { get; set; }
        public int PeriodOrder { get; set; }
    }


    [Flags]
    public enum AttendanceTypeEnum
    {
        NotAssigned = 1,
        Present = 2,
        Excused = 4,
        Absent = 8,
        Late = 16
    }
}
