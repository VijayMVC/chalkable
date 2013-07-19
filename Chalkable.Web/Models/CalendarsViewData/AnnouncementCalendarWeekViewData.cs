using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class AnnouncementCalendarWeekViewData
    {

        public int Day { get; set; }
        public DateTime Date { get; set; }
        public IList<AnnouncementShortViewData> Announcements { get; set; }
        public IList<AnnouncementPeriodViewData> AnnouncementPeriods { get; set; }

        protected AnnouncementCalendarWeekViewData(DateTime date, IList<AnnouncementPeriodViewData> announcementPeriods, IList<AnnouncementComplex> announcements)
        {
            Day = date.Day;
            Date = date.Date;
            Announcements = announcements.Select(AnnouncementShortViewData.Create).ToList();
            AnnouncementPeriods = announcementPeriods;
        }

        public static AnnouncementCalendarWeekViewData Create(DateTime date, IList<AnnouncementPeriodViewData> announcementPeriods, IList<AnnouncementComplex> announcements)
        {
            return new AnnouncementCalendarWeekViewData(date, announcementPeriods, announcements); 
        }
    }
}