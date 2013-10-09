using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class AnnouncementByDateViewData
    {
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public int DayOfWeek { get; set; }
        public IList<AnnouncementShortViewData> Announcements { get; set; }
    
        protected AnnouncementByDateViewData(){}
        protected AnnouncementByDateViewData(DateTime date, IList<AnnouncementComplex> announcements)
        {
            Day = date.Day;
            Date = date.Date;
            DayOfWeek = (int)date.DayOfWeek;
            Announcements = AnnouncementShortViewData.Create(announcements);
        }

        public static IList<AnnouncementByDateViewData> Create(IList<DateTime> dates, IList<AnnouncementComplex> announcements)
        {
            var res = new List<AnnouncementByDateViewData>();
            foreach (var dateTime in dates)
            {
                var annPerDay = announcements.Where(x => x.Expires == dateTime.Date).ToList();
                res.Add(new AnnouncementByDateViewData(dateTime, annPerDay));
            }
            return res;
        } 
    }


    public class AnnouncementCalendarWeekViewData : AnnouncementByDateViewData
    {
        public IList<AnnouncementPeriodViewData> AnnouncementPeriods { get; set; }

        protected AnnouncementCalendarWeekViewData(DateTime date, IList<AnnouncementPeriodViewData> announcementPeriods,
                                                   IList<AnnouncementComplex> announcements) : base(date, announcements)
        {
            AnnouncementPeriods = announcementPeriods;
        }

        public static AnnouncementCalendarWeekViewData Create(DateTime date, IList<AnnouncementPeriodViewData> announcementPeriods, IList<AnnouncementComplex> announcements)
        {
            return new AnnouncementCalendarWeekViewData(date, announcementPeriods, announcements); 
        }
    }
}