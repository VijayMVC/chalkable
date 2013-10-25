using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class Date
    {   
        public const string DATE_TIME_FIELD = "Date";
        public DateTime DateTime { get; set; }
        public const string DATE_TYPE_REF_FIELD = "DateTypeRef";
        public int? DateTypeRef { get; set; }
        public const string IS_SCHOOL_DAY_FIELD = "IsSchoolDay";
        public bool IsSchoolDay { get; set; }
        public int SchoolYearRef { get; set; }     
    }

    public class DateDetails : Date
    {
        public DateType ScheduleSection { get; set; }
    }
}
