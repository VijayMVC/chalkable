using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementPeriodViewData
    {
        public ScheduleItemViewData Period { get; set; }
        public List<AnnouncementShortViewData> Announcements { get; set; }

        public static AnnouncementPeriodViewData Create(ScheduleItem scheduleItem, IList<AnnouncementComplex> announcements)
        {
            var periodVd = ScheduleItemViewData.Create(scheduleItem);
            var annVd = announcements.Select(AnnouncementShortViewData.Create).ToList();
            var res = new AnnouncementPeriodViewData
            {
                Period = periodVd,
                Announcements = annVd,
            };
            return res;
        }
        
        public static IList<AnnouncementPeriodViewData> Create(IList<ScheduleItem> schedule, IList<AnnouncementComplex> announcements)
        {
            var res = new List<AnnouncementPeriodViewData>();
            foreach (var scheduleItem in schedule)
            {
                var annItems = announcements.Where(x => scheduleItem.ClassId == x.ClassRef && x.GradableType).ToList();
                res.Add(Create(scheduleItem, annItems));
            }
            return res;
        }
    }
}