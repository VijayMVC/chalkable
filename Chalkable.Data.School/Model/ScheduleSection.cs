using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class ScheduleSection
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public Guid MarkingPeriodRef { get; set; }
        public int? SisId { get; set; }
    }
}
