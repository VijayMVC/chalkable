using System.Collections.Generic;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassScheduleViewData
    {
        public ClassViewData Class { get; set; }
        public IList<AnnouncementDayCalendarViewData> Schedule { get; set; }
        private ClassScheduleViewData() { }


        public static ClassScheduleViewData Create(ClassDetails classDetails, IList<AnnouncementDayCalendarViewData> schedule)
        {
            var res = new ClassScheduleViewData
            {
                Class = ClassViewData.Create(classDetails),
                Schedule = schedule
            };
            return res;
        }
    }
}