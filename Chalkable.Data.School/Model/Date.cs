using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Date
    {
        public const string DATE_TIME_FIELD = "Day";
        public const string DATE_TYPE_REF_FIELD = "DateTypeRef";
        public const string IS_SCHOOL_DAY_FIELD = "IsSchoolDay";
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";

        [PrimaryKeyFieldAttr]
        public DateTime Day { get; set; }
        public bool IsSchoolDay { get; set; }
        [PrimaryKeyFieldAttr]
        public int SchoolYearRef { get; set; }
        public int SchoolRef { get; set; }
        public int? DayTypeRef { get; set; }
        [NotDbFieldAttr]
        public DayType DayType { get; set; }
    }
}
