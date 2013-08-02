using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Period
    {
        public Guid Id { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public Guid MarkingPeriodRef { get; set; }
        public Guid SectionRef { get; set; }
        public int? SisId { get; set; }
        public int Order { get; set; }
        public int? SisId2 { get; set; }

        [NotDbFieldAttr]
        public ScheduleSection Section { get; set; }
    }
}
