using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementPeriodViewData
    {
        public PeriodViewData Period { get; set; }
        public string RoomNumber { get; set; }
        public List<AnnouncementShortViewData> Announcements { get; set; }

        public static AnnouncementPeriodViewData Create(Period period, DateTime date, IList<AnnouncementComplex> announcements, string roomNumber)
        {
            var periodVd = PeriodViewData.Create(period);
            var annVd = announcements.Select(AnnouncementShortViewData.Create).ToList();
            var res = new AnnouncementPeriodViewData
            {
                Period = periodVd,
                Announcements = annVd,
                RoomNumber = roomNumber
            };
            return res;

        }
        public static IList<AnnouncementPeriodViewData> Create(IList<Period> periods, IList<ClassPeriod> classPeriods,
                                                               Date date, IList<AnnouncementComplex> announcements, IList<Room> rooms)
        {
            var res = new List<AnnouncementPeriodViewData>();
            foreach (var period in periods)
            {
                var cp = classPeriods.Where(x => x.PeriodRef == period.Id && date.DayTypeRef == x.DayTypeRef).ToList();
                var annItems = announcements.Where(x => cp.Any(y => y.ClassRef == x.ClassRef) && x.GradableType).ToList();
                string roomNumber = cp.Aggregate("", (current, classePeriod) => current + rooms.First(x => x.Id == classePeriod.RoomRef).RoomNumber + " ");
                res.Add(Create(period, date.Day, annItems, roomNumber));
            }
            return res;
        }
    }
}