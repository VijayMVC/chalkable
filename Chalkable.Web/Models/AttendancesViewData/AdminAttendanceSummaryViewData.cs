using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class AdminAttendanceSummaryViewData
    {
        public NowAttendanceViewData NowAttendanceData { get; set; }
        public AttendanceByDayViewData AttendanceByDayData { get; set; }
        public AttendanceByMpViewData AttendanceByMpData { get; set; }
    }

    public class AbsentStudentAttendanceViewData : ShortPersonViewData
    {
        public int AbsentsCount { get; set; }
        protected AbsentStudentAttendanceViewData(Person person): base(person)
        {
        }
        public static AbsentStudentAttendanceViewData Create(Person person, int absentCount)
        {
            return new AbsentStudentAttendanceViewData(person){AbsentsCount = absentCount};
        }
    }

    public class NowAttendanceViewData
    {
        public IList<AbsentStudentAttendanceViewData> AbsentNowStudents { get; set; }
        public int AbsentNowCount { get; set; }
        public int AbsentUsually { get; set; }
        public int AvgOfAbsentsInYear { get; set; }

        public static NowAttendanceViewData Create(IList<Person> studentsAbsentNow, IDictionary<Guid, int> studentsAbsentTotal, int absentUsually)
        {
            return new NowAttendanceViewData
                {
                    AbsentUsually = absentUsually,
                    AvgOfAbsentsInYear = studentsAbsentTotal.Count > 0 ? (int) studentsAbsentTotal.Average(x => x.Value) : 0,
                    AbsentNowStudents = studentsAbsentNow.Select(st => AbsentStudentAttendanceViewData.Create(st, studentsAbsentTotal[st.Id])).ToList(),
                    AbsentNowCount = studentsAbsentNow.Count
                };
        }
    }

    public class AttendanceByDayViewData
    {
        
    }

    public class AttendanceByMpViewData
    {
        
    }
}