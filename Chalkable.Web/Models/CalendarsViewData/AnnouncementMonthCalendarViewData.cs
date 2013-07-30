using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class AnnouncementMonthCalendarViewData : MonthCalendarViewData
    {
        public ScheduleSectionViewData ScheduleSection { get; set; }
        public IList<AnnouncementShortViewData> Announcements { get; set; }
        public IList<AnnouncementShortViewData> Items { get; set; }

        protected AnnouncementMonthCalendarViewData(DateTime date, bool isCurrentMonth, IList<AnnouncementComplex> announcements, ScheduleSection section)
            : base(date, isCurrentMonth)
        {
            Announcements = announcements.Where(x => !x.GradableType).Select(AnnouncementShortViewData.Create).ToList();
            Items = announcements.Where(x => x.GradableType).Select(AnnouncementShortViewData.Create).ToList();
            ScheduleSection = ScheduleSectionViewData.Create(section);
        }

        public static AnnouncementMonthCalendarViewData Create(DateTime dateTime, bool isCurrentMonth, IList<AnnouncementComplex> announcements
            , IList<DateDetails> dates)
        {
            var anns = announcements.Where(x => x.Expires.Date == dateTime).ToList();
            var dateDetails = dates.FirstOrDefault(x => x.DateTime == dateTime.Date);
            var section = dateDetails != null ? dateDetails.ScheduleSection : null;
            return new AnnouncementMonthCalendarViewData(dateTime, isCurrentMonth, anns, section);
        }
    }
}