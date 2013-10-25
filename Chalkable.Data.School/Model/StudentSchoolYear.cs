using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentSchoolYear
    {
        public const string ID_FIELD = "Id";
        public int Id { get; set; }

        public const string GRADE_LEVEL_REF_FIELD = "GradeLevelRef";
        public int GradeLevelRef { get; set; }

        public const string STUDENT_FIELD_REF_FIELD = "StudentRef";
        public int StudentRef { get; set; }
        public const string SCHOOL_YEAR_REF_FIELD = "SchoolYearRef";
        public int SchoolYearRef { get; set; }

        [NotDbFieldAttr]
        public GradeLevel GradeLevel { get; set; }
    }
}
