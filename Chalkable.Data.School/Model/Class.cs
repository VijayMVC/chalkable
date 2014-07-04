using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Class
    {
        public const string ID_FIELD = "Id";
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";
        public const string PRIMARY_TEACHER_REF_FIELD = "PrimaryTeacherRef";
        public const string GRADE_LEVEL_REF_FIELD = "GradeLevelRef";
        public const string ROOM_REF_FIELD = "RoomRef";
        public const string COURSE_REF_FIELD = "CourseRef";
        public const string GRADING_SCALE_REF_FIELD = "GradingScaleRef";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ChalkableDepartmentRef { get; set; }
        public int? SchoolYearRef { get; set; }
        public int? PrimaryTeacherRef { get; set; }
        public int GradeLevelRef { get; set; }
        public int? SchoolRef { get; set; }
        public int? RoomRef { get; set; }
        public int? CourseRef { get; set; }
        public int? GradingScaleRef { get; set; }
        public string ClassNumber { get; set; }
    }

    public class ClassDetails : Class
    {
        [NotDbFieldAttr]
        public Person PrimaryTeacher { get; set; }
        public IList<MarkingPeriodClass> MarkingPeriodClasses { get; set; }
        [NotDbFieldAttr]
        public GradeLevel GradeLevel { get; set; }
        public int StudentsCount { get; set; }
    }
}
