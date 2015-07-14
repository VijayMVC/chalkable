using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AttendanceMonth
    {
        public const string SCHOOL_YEAR_REF_FIELD = "SchoolYearRef";
        public const string START_DATE_FIELD = "StartDate";
        public const string END_DATE_FIELD = "EndDate";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int SchoolYearRef { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsLockedAttendance { get; set; }
        public bool IsLockedDiscipline { get; set; }
    }
}
