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
        public const string START_DATE_FIELD = "StartDate";
        public const string END_DATE_FIELD = "EndDate";
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";
   
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int SchoolYearRef { get; set; }
        public int WeekDays { get; set; }
        public int SchoolRef { get; set; }
    }


}
