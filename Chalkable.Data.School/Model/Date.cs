using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class Date
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public Guid ScheduleSectionRef { get; set; }
        public Guid MarkingPeriodRef { get; set; }
        public bool IsSchoolDay { get; set; }
        public int? SisId { get; set; }
    }
}
