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
        public const string ID_FIELD = "Id";
        public Guid Id { get; set; }
        public const string START_TIME_FIELD = "StartTime";
        public int StartTime { get; set; }
        public const string END_TIME_FIELD = "EndTime";
        public int EndTime { get; set; }
        public const string MARKING_PERIOD_REF_FIELD = "MarkingPeriodRef";
        public Guid MarkingPeriodRef { get; set; }
        public const string SECTION_REF = "SectionRef";
        public Guid SectionRef { get; set; }
        public int? SisId { get; set; }
        public int Order { get; set; }
        public int? SisId2 { get; set; }

        [NotDbFieldAttr]
        public ScheduleSection Section { get; set; }
    }
}
