﻿using System;

namespace Chalkable.Data.School.Model.Sis
{
    public class ScheduleItem
    {
        public DateTime Day { get; set; }
        public bool IsSchoolDay { get; set; }
        public int? DayTypeId { get; set; }
        public int SchoolYearId { get; set; }
        public int PeriodId { get; set; }
        public int PeriodOrder { get; set; }
        public string PeriodName { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public string ClassNumber { get; set; }
        public int? MinGradeLevelId { get; set; }
        public int? MaxGradeLevelId { get; set; }
        public int? RoomId { get; set; }
        public string RoomNumber { get; set; }
        public int? StartTime { get; set; }
        public int? EndTime { get; set; }
        public Guid? ChalkableDepartmentId { get; set; }
        public bool? CanCreateItem { get; set; }
        public int? TeacherId { get; set; }
        public string TeacherFirstName { get; set; }
        public string TeacherLastName { get; set; }
        public string TeacherGender { get; set; }
    }
}