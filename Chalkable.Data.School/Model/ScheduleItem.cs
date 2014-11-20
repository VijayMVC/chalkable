using System;

namespace Chalkable.Data.School.Model
{
    public class ScheduleItem
    {
        public DateTime Day { get; set; }
        public bool IsSchoolDay { get; set; }
        public int? DayTypeId { get; set; }
        public int SchoolYearId { get; set; }
        public int PeriodId { get; set; }
        public int PeriodOrder { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public string ClassNumber { get; set; }
        public int? GradeLevelId { get; set; }
        public int? RoomId { get; set; }
        public string RoomNumber { get; set; }
        public int? StartTime { get; set; }
        public int? EndTime { get; set; }
        public Guid? ChalkableDepartmentId { get; set; }
    }
}