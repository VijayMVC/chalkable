using System;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class MonthCalendarViewData
    {
        public int Day { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsSunday { get; set; }
        public DateTime Date { get; set; }
        
        protected MonthCalendarViewData(DateTime date, bool isCurrentMonth)
        {
            Date = date;
            Day = date.Day;
            IsSunday = date.DayOfWeek == DayOfWeek.Sunday;
            IsCurrentMonth = isCurrentMonth;
        } 
    }
}