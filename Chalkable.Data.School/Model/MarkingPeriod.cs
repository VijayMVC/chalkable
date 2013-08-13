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
        public const string ID_FIELD = "Id";
        public Guid Id { get; set; }
        public string Name { get; set; }
        public const string START_DATE_FIELD = "StartDate";
        public DateTime StartDate { get; set; }
        public const string END_DATE_FIELD = "EndDate";
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";
        public Guid SchoolYearRef { get; set; }
        public int WeekDays { get; set; }
        public int? SisId { get; set; }
    }


}
