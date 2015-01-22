using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
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

        public IList<AttendanceStatsViewData> AttendanceStats { get; set; } 

        public static NowAttendanceViewData Create(IList<Person> studentsAbsentNow, IDictionary<int, int> studentsAbsentTotal
            , int absentUsually, IList<AttendanceStatsViewData>  attendanceStats)
        {
            return new NowAttendanceViewData
                {
                    AbsentUsually = absentUsually,
                    AvgOfAbsentsInYear = studentsAbsentTotal.Count > 0 ? (int) studentsAbsentTotal.Average(x => x.Value) : 0,
                    AbsentNowStudents = studentsAbsentNow.Select(st => AbsentStudentAttendanceViewData.Create(st, studentsAbsentTotal[st.Id])).ToList(),
                    AbsentNowCount = studentsAbsentNow.Count,
                    AttendanceStats = attendanceStats
                };
        }
    }

    public class AttendanceByDayViewData
    {
        public int StudentsCountAbsentWholeDay { get; set; }
        public IList<ShortPersonViewData> StudentsAbsentWholeDay { get; set; }
        public IList<ShortPersonViewData> AbsentStudents { get; set; }
        public IList<ShortPersonViewData> ExcusedStudents { get; set; }
        public IList<ShortPersonViewData> LateStudents { get; set; }
        public IList<ShortPersonViewData> NoAttendanceTakenTeachers { get; set; } 

        public IList<AttendanceStatsViewData> AttendancesStats { get; set; }
 
        public static AttendanceByDayViewData Create(IList<Person> allStudents, IList<PersonAttendanceTotalPerType> stsAttendanceTotalPerType
               , IList<int> stsIdsAbsentFromDay, IList<AttendanceStatsViewData> attendancesStats)
        {

            throw new NotImplementedException();
        }

        private static IList<ShortPersonViewData> PrepareStudentsByAttendanceType(string level,
                                           IDictionary<string, List<Person>> stsByType)
        {
            var res = stsByType.ContainsKey(level) ? stsByType[level] : new List<Person>();
            return ShortPersonViewData.Create(res);
        }

        private static IDictionary<string, List<Person>> GroupStudentsByType(IList<PersonAttendanceTotalPerType> stsAttendanceTotalPerType
            , IList<Person> allStudents)
        {
            var res = stsAttendanceTotalPerType.GroupBy(x => x.Level)
                                            .ToDictionary
                                            (
                                                x => x.Key,
                                                x => allStudents.Where(st => x.Any(y => y.PersonId == st.Id))
                                                                .Select(st => new {Total = x.First(y=>y.PersonId == st.Id).Total, Student = st})
                                                                .ToList()
                                            );
            return res.ToDictionary(x=>x.Key, x=>x.Value.OrderByDescending(y=>y.Total).Select(y=>y.Student).ToList());
        }
    }

    public class AttendanceByMpViewData
    {
        public IList<ShortPersonViewData> AbsentAndLateStudents { get; set; }
        public int AbsentStudentsCountAvg { get; set; }

        public IList<AttendanceStatsViewData> AttendanceStats { get; set; }

        public static AttendanceByMpViewData Create(IList<Person> allStudents, IList<int> absentAndLateStudentsIds
            , int absentStsCountAvg, IList<AttendanceStatsViewData> attendanceStats)
        {
            var res = new AttendanceByMpViewData
                {
                    AbsentStudentsCountAvg = absentStsCountAvg,
                    AbsentAndLateStudents = new List<ShortPersonViewData>(),
                    AttendanceStats = attendanceStats
                };
            foreach (var absentAndLateStudentsId in absentAndLateStudentsIds)
            {
                var st = allStudents.First(x => absentAndLateStudentsId == x.Id);
                res.AbsentAndLateStudents.Add(ShortPersonViewData.Create(st));
            }
            return res;
        }
    }
}