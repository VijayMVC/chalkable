using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class WeekCalendarViewData
    {
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public int DayOfWeek { get; set; }

        protected WeekCalendarViewData(){}
        protected WeekCalendarViewData(DateTime date)
        {
            Day = date.Day;
            Date = date.Date;
            DayOfWeek = (int)date.DayOfWeek;
        }
    }

    public class AnnouncementByDateViewData : WeekCalendarViewData
    {

        public IList<ShortAnnouncementViewData> Announcements { get; set; }
    
        protected AnnouncementByDateViewData(){}
        protected AnnouncementByDateViewData(DateTime date, IList<Announcement> announcements): base(date)
        {
            Announcements = announcements.Select(ShortAnnouncementViewData.Create).ToList();
        }

        //public static IList<AnnouncementByDateViewData> Create(IList<DateTime> dates, IList<AnnouncementComplex> announcements)
        //{
        //    var res = new List<AnnouncementByDateViewData>();
        //    foreach (var dateTime in dates)
        //    {
        //        var annPerDay = announcements.Where(x => x.Expires == dateTime.Date).ToList();
        //        res.Add(new AnnouncementByDateViewData(dateTime, annPerDay));
        //    }
        //    return res;
        //} 
    }
    

    public class AnnouncementCalendarWeekViewData : WeekCalendarViewData
    {
        public IList<AdminAnnouncementViewData> AdminAnnouncements { get; set; } 
        public IList<AnnouncementPeriodViewData> AnnouncementPeriods { get; set; }

        protected AnnouncementCalendarWeekViewData(DateTime date, IList<AnnouncementPeriodViewData> announcementPeriods, IList<AdminAnnouncement> adminAnnouncements) : base(date)
        {
            AnnouncementPeriods = announcementPeriods;
            AdminAnnouncements = AdminAnnouncementViewData.Create(adminAnnouncements);
        }

        public static AnnouncementCalendarWeekViewData Create(DateTime date, IList<AnnouncementPeriodViewData> announcementPeriods
            , IList<AdminAnnouncement> adminAnnouncements)
        {
            return new AnnouncementCalendarWeekViewData(date, announcementPeriods, adminAnnouncements); 
        }
    }
}