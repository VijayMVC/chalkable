using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class MarkingPeriod
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public Guid SchoolYearRef { get; set; }
        public int WeekDays { get; set; }
        public int? SisId { get; set; }
        [DataEntityAttr]
        public SchoolYear SchoolYear { get; set; }

        public IList<MarkingPeriodClass> MarkingPeriodClasses { get; set; } 
    }


}
