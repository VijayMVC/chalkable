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
        public const string DATE_TIME_FIELD = "DateTime";
        public DateTime DateTime { get; set; }
        public Guid? ScheduleSectionRef { get; set; }
        public Guid? MarkingPeriodRef { get; set; }
        public const string IS_SCHOOL_DAY_FIELD = "IsSchoolDay";
        public bool IsSchoolDay { get; set; }
        public int? SisId { get; set; }
    }

    public class DateDetails : Date
    {
        public ScheduleSection ScheduleSection { get; set; }
    }
}
