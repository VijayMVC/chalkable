using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class SchoolGradeLevel
    {
        public const string SCHOOL_REF_FIELD = "SchoolRef";
        public const string GRADE_LEVEL_REF_FIELD = "SchoolRef";

        public int SchoolRef { get; set; }
        public int GradeLevelRef { get; set; }
    }
}
