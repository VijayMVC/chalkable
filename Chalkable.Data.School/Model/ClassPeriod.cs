using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassPeriod
    {
        public Guid Id { get; set; }
        public Guid PersonRef { get; set; }
        public Guid ClassRef { get; set; }
        public Guid RoominfoRef { get; set; }
        public int? SisId { get; set; }

        [DataEntityAttr]
        public Period Period { get; set; }
    }
}
