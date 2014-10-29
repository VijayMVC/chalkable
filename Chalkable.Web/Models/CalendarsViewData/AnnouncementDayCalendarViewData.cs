using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class AnnouncementDayCalendarViewData
    {
        public DateTime Date { get; set; }
        public int Day { get; set; }
        public IList<AnnouncementCalendarDayItemViewData> CalendarDayItems { get; set; }

        public static AnnouncementDayCalendarViewData Create(IList<Period> periods, DateTime date,IList<ClassPeriod> classPeriods, 
            IList<ClassDetails> classes, IList<AnnouncementComplex> announcements, IList<Room> rooms)
        {
            var res = new AnnouncementDayCalendarViewData
                {
                    Date = date.Date,
                    Day = date.Day,
                   
                };
            if (periods != null)
                res.CalendarDayItems = AnnouncementCalendarDayItemViewData.Create(announcements, periods, date, classPeriods, classes, rooms);
            return res;
        }
    }

    public class AnnouncementCalendarDayItemViewData
    {
        public PeriodViewData Period { get; set; }
        public IList<AnnouncementsClassPeriodViewData> AnnouncementClassPeriods { get; set; }
 
        public static AnnouncementCalendarDayItemViewData Create(IList<AnnouncementComplex> announcements, Period period, 
            DateTime date , IList<ClassPeriod> classPeriods, IList<ClassDetails> classes, IList<Room> rooms)
        {
            classPeriods = classPeriods.Where(x => x.PeriodRef == period.Id).ToList();
            return new AnnouncementCalendarDayItemViewData
                {
                    Period = PeriodViewData.Create(period),
                    AnnouncementClassPeriods = AnnouncementsClassPeriodViewData.Create(announcements, classPeriods, classes, rooms, date)
                };
        }
        public static IList<AnnouncementCalendarDayItemViewData> Create(IList<AnnouncementComplex> announcements, IList<Period> periods,
            DateTime date, IList<ClassPeriod> classPeriods, IList<ClassDetails> classes, IList<Room> rooms)
        {
            return periods.Select(period => Create(announcements, period, date, classPeriods, classes, rooms)).ToList();
        }
    }
}