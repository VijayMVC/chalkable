using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementsClassPeriodViewData
    {
        public int DayNumber { get; set; }
        public ClassPeriodViewData ClassPeriod { get; set; }
        public IList<AnnouncementShortViewData> Announcements { get; set; } 

        public static IList<AnnouncementsClassPeriodViewData> Create(IList<AnnouncementComplex> announcements,
            IList<ClassPeriod> classPeriods, IList<ClassDetails> classes, IList<Room> rooms, DateTime date)
        {
            var res = new List<AnnouncementsClassPeriodViewData>();
            foreach (var classPeriod in classPeriods)
            {
                var c = classes.First(x => x.Id == classPeriod.ClassRef);
                var anns = announcements.Where(x => x.ClassRef == classPeriod.ClassRef).ToList();
                var room = c.RoomRef.HasValue ? rooms.First(x => x.Id == c.RoomRef) : null;
                res.Add(new AnnouncementsClassPeriodViewData
                    {
                        Announcements = AnnouncementShortViewData.Create(anns),
                        ClassPeriod = ClassPeriodViewData.Create(classPeriod, c, room),
                        DayNumber = date.Day
                    });
            }
            return res;
        }
    }
}