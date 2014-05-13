using System;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class ClassAttendance
    {
        public int PersonRef { get; set; }
        public int ClassRef { get; set; }
        public int? AttendanceReasonRef { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }
        public bool AbsentPreviousDay { get; set; }

        public static bool IsLateLevel(string level)
        {
            return level == "T";
        }

        public static bool IsAbsentLevel(string level)
        {
            return level == "A"
                       || level == "AO"
                       || level == "H"
                       || level == "HO";
        }

        public static bool IsAbsentOrLateLevel(string level)
        {
            return IsLateLevel(level) || IsAbsentLevel(level);
        }

        public bool IsAbsent
        {
            get { return IsAbsentLevel(Level); }
        }

        public bool IsLate
        {
            get { return IsLateLevel(Level); }
        }

        public bool IsAbsentOrLate
        {
            get { return IsAbsentOrLateLevel(Level); }
        }

        public bool IsExcused
        {
            get { return Category == "E"; }
        }

        public static readonly string[] AbsentLevels = {"A", "AO", "H", "HO"};
        public static readonly string[] LateLevels = { "T"};
    }


    public class ClassAttendanceDetails : ClassAttendance
    {
        public Person Student { get; set; }
        public Class Class { get; set; }
        public bool IsPosted { get; set; }
    }

    public class AttendanceTotalPerType
    {
        public const string TOTAL_FIELD = "Total";
        public int Total { get; set; }
        public const string ATTENDANCE_TYPE_FIELD = "AttendanceType";
        public string Level { get; set; }
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
}
