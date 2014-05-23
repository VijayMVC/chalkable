using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class ClassAttendance
    {
        public int PersonRef { get; set; }
        public int ClassRef { get; set; }
        public int? AttendanceReasonRef { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }
        public bool AbsentPreviousDay { get; set; }

        public static bool IsLateLevel(string level)
        {
            return level == "T";
        }

        public static bool IsAbsentLevel(string level)
        {
            return level == "A"
                       || level == "AO"
                       || level == "H"
                       || level == "HO";
        }

        public static bool IsAbsentOrLateLevel(string level)
        {
            return IsLateLevel(level) || IsAbsentLevel(level);
        }

        public bool IsAbsent
        {
            get { return IsAbsentLevel(Level); }
        }

        public bool IsLate
        {
            get { return IsLateLevel(Level); }
        }

        public bool IsAbsentOrLate
        {
            get { return IsAbsentOrLateLevel(Level); }
        }

        public bool IsExcused
        {
            get { return Category == "E"; }
        }

        public static readonly string[] AbsentLevels = {"A", "AO", "H", "HO"};
        public static readonly string[] LateLevels = { "T"};
    }


    public class ClassAttendanceDetails : ClassAttendance
    {
        public Person Student { get; set; }
        public Class Class { get; set; }
        public bool IsPosted { get; set; }
    }

    public class AttendanceTotalPerType
    {
        public const string TOTAL_FIELD = "Total";
        public int Total { get; set; }
        public const string ATTENDANCE_TYPE_FIELD = "AttendanceType";
        public string Level { get; set; }
    }
    public class PersonAttendanceTotalPerType : AttendanceTotalPerType
    {
        public int PersonId { get; set; }
    }
    public class StudentAbsentFromPeriod
    {
        public DateTime Date { get; set; }
        public int PersonId { get; set; }
        public int PeriodOrder { get; set; }
    }
    public class StudentCountAbsentFromPeriod
    {
        public DateTime Date { get; set; }
        public int StudentCount { get; set; }
        public int PeriodOrder { get; set; }
    }

    public class AttendanceSummary
    {
        public IList<DailyAttendanceSummary> DaysStat { get; set; }
        public IList<StudentAttendanceSummary> Students { get; set; }
    }

    public class DailyAttendanceSummary
    {
        public decimal Absences { get; set; }
        public DateTime Date { get; set; }
        public int Tardies { get; set; }

        public static IList<DailyAttendanceSummary> Create(IList<DailySectionAttendanceSummary> dailySectionAttendances)
        {
            return dailySectionAttendances.GroupBy(x => x.Date).Select(x => new DailyAttendanceSummary
                {
                    Absences = x.Sum(y => y.Absences),
                    Tardies = x.Sum(y => y.Tardies),
                    Date = x.Key
                }).ToList();
        } 
    }

    public class StudentAttendanceSummary
    {
        public Person Student { get; set; }
        public IList<StudentClassAttendanceSummary> ClassAttendanceSummaries { get; set; }

        public static IList<StudentAttendanceSummary> Create(IList<StudentSectionAttendanceSummary> studentSectionAttendances, IList<Person> students, IList<ClassDetails> classes)
        {
            return studentSectionAttendances.GroupBy(x=>x.StudentId).Select(x => new StudentAttendanceSummary
                {
                    Student = students.First(student => student.Id == x.Key),
                    ClassAttendanceSummaries = StudentClassAttendanceSummary.Create(x.ToList(), classes)
                }).ToList();
        } 
    }

    public class StudentClassAttendanceSummary
    {
        public int StudentId { get; set; }
        public decimal Absences { get; set; }
        public Class Class { get; set; }
        public int Tardies { get; set; }

        public static IList<StudentClassAttendanceSummary> Create(IList<StudentSectionAttendanceSummary> studentSectionAttendances, IList<ClassDetails> classes)
        {
            return studentSectionAttendances.Select(x => new StudentClassAttendanceSummary
            {
                Absences = x.Absences,
                Tardies = x.Tardies,
                Class = classes.First(c=>c.Id == x.SectionId),
                
            }).ToList();
        } 
    }
}
