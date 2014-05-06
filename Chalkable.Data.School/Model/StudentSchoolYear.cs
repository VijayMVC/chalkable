﻿using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentSchoolYear
    {
        public const string GRADE_LEVEL_REF_FIELD = "GradeLevelRef";
        public const string STUDENT_FIELD_REF_FIELD = "StudentRef";
        public const string SCHOOL_YEAR_REF_FIELD = "SchoolYearRef";
        
        [PrimaryKeyFieldAttr]
        public int StudentRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int SchoolYearRef { get; set; }
        public int GradeLevelRef { get; set; }
        public StudentEnrollmentStatusEnum EnrollmentStatus { get; set; }

        [NotDbFieldAttr]
        public GradeLevel GradeLevel { get; set; }
       
        public bool IsEnrolled
        {
            get { return EnrollmentStatus == StudentEnrollmentStatusEnum.CurrentlyEnrolled; }
        }
    }

    public enum StudentEnrollmentStatusEnum
    {
        CurrentlyEnrolled = 0,
        PreviouslyEnrolled = 1
    }
}
