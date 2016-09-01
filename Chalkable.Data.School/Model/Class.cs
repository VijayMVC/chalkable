using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Class
    {
        public const string ID_FIELD = "Id";
        public const string NAME_FIELD = "Name";
        public const string CLASS_NUMBER_FIELD = "ClassNumber";
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";
        public const string PRIMARY_TEACHER_REF_FIELD = "PrimaryTeacherRef";
        public const string ROOM_REF_FIELD = "RoomRef";
        public const string COURSE_REF_FIELD = "CourseRef";
        public const string COURSE_TYPE_REF_FIELD = "CourseTypeRef";
        public const string GRADING_SCALE_REF_FIELD = "GradingScaleRef";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ChalkableDepartmentRef { get; set; }
        public int? SchoolYearRef { get; set; }
        public int? PrimaryTeacherRef { get; set; }
        public int? MinGradeLevelRef { get; set; }
        public int? MaxGradeLevelRef { get; set; }
        public int? RoomRef { get; set; }
        public int? CourseRef { get; set; }
        public int CourseTypeRef { get; set; }
        public int? GradingScaleRef { get; set; }
        public string ClassNumber { get; set; }
    }

    public class ClassDetails : Class
    {
        [NotDbFieldAttr]
        public Person PrimaryTeacher { get; set; }
        [NotDbFieldAttr]
        public SchoolYear SchoolYear { get; set; }
        public IList<MarkingPeriodClass> MarkingPeriodClasses { get; set; }
        public IList<ClassTeacher> ClassTeachers { get; set; }
        public IList<ClassPeriod> ClassPeriods { get; set; } 
        public int StudentsCount { get; set; }
    }

    public class CourseDetails : Class
    {
        public IList<ClassDetails> Classes { get; set; } 
    }
}
