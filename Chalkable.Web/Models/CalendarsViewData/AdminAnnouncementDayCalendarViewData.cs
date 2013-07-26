using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class AdminDayCalendarViewData
    {
        public IList<PeriodViewData> Periods { get; set; }
        public IList<GradeLevelViewData> GradeLevels { get; set; }
        public IList<CalendarDayClassPeriodsViewData> CalendarDayItems { get; set; }
        public DateTime Date { get; set; }
        public int Day { get; set; }

        public static AdminDayCalendarViewData Create(IList<Period> periods, IList<GradeLevel> gradeLevels, IList<ClassPeriod> classPeriods
           , IList<ClassDetails> classes, IList<Room> rooms, DateTime date)
        {
            IList<CalendarDayClassPeriodsViewData> cdClassperiods = new List<CalendarDayClassPeriodsViewData>();
            foreach (var gradeLevel in gradeLevels)
            {
                var classesByGl = classes.Where(x => x.GradeLevelRef == gradeLevel.Id).ToList();
                foreach (var period in periods)
                {
                    var list = classPeriods.Where(x => x.PeriodRef == period.Id && classesByGl.Any(y => y.Id == x.ClassRef)).ToList();
                    cdClassperiods.Add(CalendarDayClassPeriodsViewData.Create(list, period, gradeLevel, classesByGl, rooms, date));
                }
            }
            return new AdminDayCalendarViewData
            {
                Periods = PeriodViewData.Create(periods),
                GradeLevels = GradeLevelViewData.Create(gradeLevels),
                CalendarDayItems = cdClassperiods.OrderBy(x => x.Period.Order).ToList(),
                Date = date,
                Day = date.Day
            };
        }
    }

    public class CalendarDayClassPeriodsViewData
    {
        public PeriodViewData Period { get; set; }
        public GradeLevelViewData GradeLevel { get; set; }
        public IList<ClassPeriodViewData> ClassPeriods { get; set; }

        public static CalendarDayClassPeriodsViewData Create(IList<ClassPeriod> classPeriods, Period period,
           GradeLevel gradeLevel , IList<ClassDetails> classes, IList<Room> rooms, DateTime date)
        {
            var res = new CalendarDayClassPeriodsViewData
            {
                Period = PeriodViewData.Create(period),
                GradeLevel = GradeLevelViewData.Create(gradeLevel),
                ClassPeriods = new List<ClassPeriodViewData>(),
            };
            Room room = null;
            foreach (var classPeriod in classPeriods)
            {
                var clazz = classes.First(x => x.Id == classPeriod.ClassRef);
                if (rooms != null && rooms.Count > 0)
                    room = rooms.First(x => x.Id == classPeriod.RoomRef);
                res.ClassPeriods.Add(ClassPeriodViewData.Create(classPeriod, clazz, room));
            }
            return res;
        }
    }
}