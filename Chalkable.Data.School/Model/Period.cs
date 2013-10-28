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
        public const string START_TIME_FIELD = "StartTime";
        public const string END_TIME_FIELD = "EndTime";
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";
        public const string ORDER_FIELD = "Order";
        
        public int Id { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int SchoolYearRef { get; set; }
        public int Order { get; set; }   
    }
}
