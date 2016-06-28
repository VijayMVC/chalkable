using System;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model.Attendances
{
    public class BaseAttendance
    {
        public const string MISSING = "Missing";
        public const string ABSENT = "Absent";
        public const string PRESENT = "Present";
        public const string TARDY = "Tardy";
        public const string EXCUSED_CATEGORY = "E";

        public int? AttendanceReasonId { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public static bool IsLateLevel(string level)
        {
            return LateLevels.Contains(level);
        }
        public static bool IsAbsentLevel(string level)
        {
            return AbsentLevels.Contains(level);
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
            get { return Category == EXCUSED_CATEGORY; }
        }

        public static readonly string[] AbsentLevels = { "A", "AO", "H", "HO" };
        public static readonly string[] LateLevels = { "T" };

    }

    public class StudentClassAttendance : BaseAttendance
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime Date { get; set; }
        public bool AbsentPreviousDay { get; set; }
        public bool ReadOnly { get; set; }
        public string ReadOnlyReason { get; set; }
        public Student Student { get; set; }
        public bool IsDailyAttendancePeriod { get; set; }
    }
}
