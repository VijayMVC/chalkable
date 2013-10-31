using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Class
    {
        public const string ID_FIELD = "Id";
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";
        public const string TEACHER_REF_FIELD = "TeacherRef";
        public const string GRADE_LEVEL_REF_FIELD = "GradeLevelRef";
 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? SchoolYearRef { get; set; }
        public Guid? ChalkableDepartmentRef { get; set; }
        public int? TeacherRef { get; set; }
        public int GradeLevelRef { get; set; }
        public int? SchoolRef { get; set; }
    }

    public class ClassDetails : Class
    {
        [DataEntityAttr]
        public Person Teacher { get; set; }
        public IList<MarkingPeriodClass> MarkingPeriodClasses { get; set; }
        [DataEntityAttr]
        public GradeLevel GradeLevel { get; set; }
        public int StudentsCount { get; set; }
    }
}
