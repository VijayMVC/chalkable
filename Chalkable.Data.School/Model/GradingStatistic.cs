using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentGradeAvg
    {
        [DataEntityAttr]
        public Person Student { get; set; }
        public int? Avg { get; set; }
    }

    public class StudentGradeAvgPerMPC : StudentGradeAvg
    {
        [DataEntityAttr]
        public MarkingPeriodClass MarkingPeriodClass { get; set; }
    }

    public class StudentGradeAvgPerClass : StudentGradeAvg
    {
        public Guid ClassRef { get; set; }
    }

    public class MarkingPeriodClassGradeAvg
    {
        [DataEntityAttr]
        public Class Class { get; set; }
        [DataEntityAttr]
        public MarkingPeriod MarkingPeriod { get; set; }
        public int? Avg { get; set; }
    }
}
