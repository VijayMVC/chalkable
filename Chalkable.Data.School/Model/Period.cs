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
        public int Id { get; set; }
        public const string START_TIME_FIELD = "StartTime";
        public int StartTime { get; set; }
        public const string END_TIME_FIELD = "EndTime";
        public int EndTime { get; set; }
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";
        public int SchoolYearRef { get; set; }
        public const string ORDER_FIELD = "Order";
        public int Order { get; set; }   
    }
}
