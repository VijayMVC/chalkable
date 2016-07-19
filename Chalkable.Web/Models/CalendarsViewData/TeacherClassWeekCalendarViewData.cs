using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class TeacherClassWeekCalendarViewData
    {
        public DateTime Date { get; set; }
        public int Day { get; set; }
        public IList<ClassPeriodShortViewData> ClassPeriods { get; set; } 

        public static TeacherClassWeekCalendarViewData Create(DateTime date, IList<ClassPeriod> classPeriods)
        {
            return new TeacherClassWeekCalendarViewData
                {
                    Date = date.Date,
                    Day = date.Day,
                    ClassPeriods = classPeriods.Select(x => ClassPeriodShortViewData.Create(x, null)).ToList()
                };
        }
    }
}