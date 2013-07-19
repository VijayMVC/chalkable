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
        public Guid PeriodRef { get; set; }
        public Guid ClassRef { get; set; }
        public Guid RoomRef { get; set; }
        public int? SisId { get; set; }

        [DataEntityAttr]
        public Period Period { get; set; }
    }

    public class ClassPeriodDetails : ClassPeriod
    {
        [DataEntityAttr]
        public Room Room { get; set; }
    }
}
