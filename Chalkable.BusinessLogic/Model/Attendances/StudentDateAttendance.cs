using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model.Attendances
{
    public class StudentDateAttendance
    {
        public StudentDetails Student { get; set; }
        public DateTime Date { get; set; }

        public bool IsAbsent
        {
            get 
            { 
                return  (DailyAttendance != null && DailyAttendance.IsAbsent )  
                        || (StudentPeriodAttendances != null && StudentPeriodAttendances.Count > 0 && StudentPeriodAttendances.All(x=> x.IsAbsent)); 
            }
        }

        public bool IsExcused
        {
            get
            {
                return  (DailyAttendance != null && DailyAttendance.IsExcused)
                        || (StudentPeriodAttendances != null && StudentPeriodAttendances.Count > 0 && StudentPeriodAttendances.All(x => x.IsExcused)); 
            }
        }


        public IList<CheckInCheckOut> CheckInCheckOuts { get; set; }
        public StudentDailyAttendance DailyAttendance { get; set; }
        public IList<StudentPeriodAttendance> StudentPeriodAttendances { get; set; }    
    }

    public class CheckInCheckOut
    {
        public int Time { get; set; }
        public int AttendanceReasonId { get; set; }
        public string Category { get; set; }
        public string Note { get; set; }
        public int? PeriodId { get; set; }
        public bool IsCheckIn { get; set; }
    }

    public class StudentPeriodAttendance : StudentClassAttendance
    {
        public Period Period { get; set; }
        public Class Class { get; set; }
    }
}
