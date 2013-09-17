using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.DataAccess;
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
        public IList<ShortPersonViewData> StudentsAbsentFromDay { get; set; }
        public IList<ShortPersonViewData> AbsentStudents { get; set; }
        public IList<ShortPersonViewData> ExcusedStudents { get; set; }
        public IList<ShortPersonViewData> LateStudents { get; set; }
        public IList<ShortPersonViewData> NoAttendanceTakenTeachers { get; set; } 

        public IList<AttendanceStatsViewData> AttendancesStats { get; set; }
 
        public static AttendanceByDayViewData Create(IList<Person> allStudents, IList<AttendanceTotalPerType> stsAttendanceTotalPerType
               , IList<Guid> stsIdsAbsentFromDay, IList<ClassAttendanceDetails> lastAttendances)
        {

            var groupedSts = GroupStudentsByType(stsAttendanceTotalPerType, allStudents);
            return new AttendanceByDayViewData
                {
                    StudentsAbsentFromDay = ShortPersonViewData.Create(allStudents.Where(x => stsIdsAbsentFromDay.Contains(x.Id)).ToList()),
                    AbsentStudents = PrepareStudentsByAttendanceType(AttendanceTypeEnum.Absent, groupedSts),
                    ExcusedStudents = PrepareStudentsByAttendanceType(AttendanceTypeEnum.Excused, groupedSts),
                    LateStudents = PrepareStudentsByAttendanceType(AttendanceTypeEnum.Late, groupedSts),
                    AttendancesStats = AttendanceStatsViewData.BuildStatsPerDate(lastAttendances, "ddd")
                };
        }

        private static IList<ShortPersonViewData> PrepareStudentsByAttendanceType(AttendanceTypeEnum type,
                                           IDictionary<AttendanceTypeEnum, List<Person>> stsByType)
        {
            var res = stsByType.ContainsKey(type) ? stsByType[type] : new List<Person>();
            return ShortPersonViewData.Create(res);
        }

        private static IDictionary<AttendanceTypeEnum, List<Person>> GroupStudentsByType(IList<AttendanceTotalPerType> stsAttendanceTotalPerType
            , IList<Person> allStudents)
        {
            var res = stsAttendanceTotalPerType.GroupBy(x => x.AttendanceType)
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

        public static AttendanceByMpViewData Create(IList<Person> allStudents, IList<Guid> absentAndLateStudentsIds, int absentStsCountAvg)
        {
            var res = new AttendanceByMpViewData();
            res.AbsentStudentsCountAvg = absentStsCountAvg;
            res.AbsentAndLateStudents = new List<ShortPersonViewData>();
            foreach (var absentAndLateStudentsId in absentAndLateStudentsIds)
            {
                var st = allStudents.First(x => absentAndLateStudentsId == x.Id);
                res.AbsentAndLateStudents.Add(ShortPersonViewData.Create(st));
            }
            return res;
        }
    }
}