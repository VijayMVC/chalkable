using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class DateType
    {
        public const string NUMBER_FIELD = "Number";
        public const string SCHOOL_YEAR_REF = "SchoolYearRef";

        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public int SchoolYearRef { get; set; }
    }
}
