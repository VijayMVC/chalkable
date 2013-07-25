using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentGradeAvgPerMPC
    {
        [DataEntityAttr]
        public Person Student { get; set; }
        [DataEntityAttr]
        public MarkingPeriodClass MarkingPeriodClass { get; set; }
        public int? Avg { get; set; }
    }

    public class StudentGradeAvgPerClass
    {
        [DataEntityAttr]
        public Person Student { get; set; }
        public Guid ClassRef { get; set; }
        public int? Avg { get; set; }
    }
}
