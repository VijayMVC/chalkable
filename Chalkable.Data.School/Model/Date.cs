using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class Date
    {
        public const string DATE_TIME_FIELD = "Day";
        public const string DATE_TYPE_REF_FIELD = "DateTypeRef";
        public const string IS_SCHOOL_DAY_FIELD = "IsSchoolDay";
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";

        public DateTime Day { get; set; }
        public int? DateTypeRef { get; set; }
        public bool IsSchoolDay { get; set; }
        public int SchoolYearRef { get; set; }
        public int SchoolRef { get; set; }
    }

    public class DateDetails : Date
    {
        public DateType DateType { get; set; }
    }
}
