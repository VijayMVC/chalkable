using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class MarkingPeriodClass
    {
        public Guid Id { get; set; }
        public Guid ClassRef { get; set; }
        public Guid MarkingPeriodRef { get; set; }

        [DataEntityAttr]
        public Class Class { get; set; }
        [DataEntityAttr]
        public MarkingPeriod MarkingPeriod { get; set; }
    }
}
