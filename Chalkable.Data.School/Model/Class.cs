﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Class
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid SchoolYearRef { get; set; }
        public Guid CourseRef { get; set; }
        public Guid TeacherRef { get; set; }
        public Guid GradeLevelRef { get; set; }
        public int? SisId { get; set; }
    }

    public class ClassComplex : Class
    {
        public Person Teacher { get; set; }
        public MarkingPeriodClass MarkingPeriodClass { get; set; }
        public GradeLevel GradeLevel { get; set; }
        public Course Course { get; set; }
        public int StudentsCount { get; set; }
    }
}
