using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class SchoolGradeLevel
    {
        public const string SCHOOL_REF_FIELD = "SchoolRef";
        public const string GRADE_LEVEL_REF_FIELD = "GradeLevelRef";

        [PrimaryKeyFieldAttr]
        public int SchoolRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int GradeLevelRef { get; set; }
    }
}
