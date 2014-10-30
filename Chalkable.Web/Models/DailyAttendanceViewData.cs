using System;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{

    public class ShortDailyAttendanceViewData
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public int? TimeIn { get; set; }
        public int? TimeOut { get; set; }
        public int? Arrival { get; set; }
        protected ShortDailyAttendanceViewData(StudentDailyAttendance dailyAttendance)
        {
            Arrival = dailyAttendance.Arrival;
            Date = dailyAttendance.Date;
            TimeIn = dailyAttendance.TimeIn;
            TimeOut = dailyAttendance.TimeOut;
        }
        public static ShortDailyAttendanceViewData Create(StudentDailyAttendance dailyAttendance)
        {
            return new ShortDailyAttendanceViewData(dailyAttendance);
        }
    }

    public class DailyAttendanceViewData : ShortDailyAttendanceViewData
    {
        public ShortPersonViewData Person { get; set; }
        protected DailyAttendanceViewData(StudentDailyAttendance dailyAttendance) : base(dailyAttendance)
        {
        }    
        
        public static DailyAttendanceViewData Create(StudentDailyAttendance dailyAttendance, Person person)
        {
            return new DailyAttendanceViewData(dailyAttendance)
                {
                    Person = ShortPersonViewData.Create(person)
                };
        }
    }
}