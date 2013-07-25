using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class DailyAttendanceViewData
    {
        public ShortPersonViewData Person { get; set; }
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public int? TimeIn { get; set; }
        public int? TimeOut { get; set; }
        public int? Arrival { get; set; }

        public static DailyAttendanceViewData Create(StudentDailyAttendanceDetails dailyAttendance)
        {
            return new DailyAttendanceViewData
                {
                    Id = dailyAttendance.Id,
                    Arrival = dailyAttendance.Arrival,
                    Date = dailyAttendance.Date,
                    TimeIn = dailyAttendance.TimeIn,
                    TimeOut = dailyAttendance.TimeOut,
                    Person = ShortPersonViewData.Create(dailyAttendance.Person)
                };
        }
    }
}