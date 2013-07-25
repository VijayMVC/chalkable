using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class StudentAttendanceViewData 
    {
        public ShortPersonViewData Student { get; set; }
        public IList<ClassAttendanceViewData> Attendances { get; set; }
        public ShortDailyAttendanceViewData DailyAttendance { get; set; }

        public static StudentAttendanceViewData Create(Person student, IList<ClassAttendanceComplex> attendances, 
            StudentDailyAttendance dailyAttendance)
        {
            return new StudentAttendanceViewData
                {
                    Student = ShortPersonViewData.Create(student),
                    Attendances = ClassAttendanceViewData.Create(attendances),
                    DailyAttendance = ShortDailyAttendanceViewData.Create(dailyAttendance)
                };
        }
    }
}