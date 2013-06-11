using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentInfo
    {
        public Guid Id { get; set; }
        public Guid PersonRef { get; set; }
        public bool IEP { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string PreviousSchool { get; set; }
        public string PreviousSchoolPhone { get; set; }
        public string PreviousSchoolNote { get; set; }
        public Guid GradeLevelRef { get; set; }

        [DataEntityAttr]
        public GradeLevel GradeLevel { get; set; }
        [DataEntityAttr]
        public Person Person { get; set; }
    }
}
