using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{

    public class ClassAttendance
    {
        public DateTime Date { get; set; }
        public bool IsDailyAttendancePeriod { get; set; }
        public bool IsPosted { get; set; }
        public bool MergeRosters { get; set; }
        public bool ReadOnly { get; set; }
        public string ReadOnlyReason { get; set; }
        public int ClassId { get; set; }
        public IList<StudentClassAttendance> StudentAttendances { get; set; }
    }

    public class ClassAttendanceDetails : ClassAttendance
    {
        public Class Class { get; set; }
    }

    public class StudentClassAttendance
    {
        public const string MISSING = "Missing";
        public const string ABSENT = "Absent";
        public const string PRESENT = "Present";
        public const string TARDY = "Tardy";
        public const string EXCUSED_CATEGORY = "E";

        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int? AttendanceReasonId { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public bool AbsentPreviousDay { get; set; }

        public bool ReadOnly { get; set; }
        public string ReadOnlyReason { get; set; }

        public static bool IsLateLevel(string level)
        {
            return LateLevels.Contains(level);
        }
        public static bool IsAbsentLevel(string level)
        {
            return AbsentLevels.Contains(level);
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
            get { return Category == EXCUSED_CATEGORY; }
        }

        public static readonly string[] AbsentLevels = {"A", "AO", "H", "HO"};
        public static readonly string[] LateLevels = { "T"};

        public StudentDetails Student { get; set; }
    }
   
    
    public class AttendanceSummary
    {
        public IList<ClassDailyAttendanceSummary> ClassesDaysStat { get; set; }
        public IList<StudentAttendanceSummary> Students { get; set; }
    }

    public class DailyAttendanceSummary
    {
        public decimal Absences { get; set; }
        public DateTime Date { get; set; }
        public int Tardies { get; set; }

        public static DailyAttendanceSummary Create(DateTime date, int tardies, decimal absences)
        {
            return new DailyAttendanceSummary {Absences = absences, Tardies = tardies, Date = date};
        }
    }

    public class ClassDailyAttendanceSummary
    {
        public Class Class { get; set; }
        public IList<DailyAttendanceSummary> DailyAttendances { get; set; }

        public static IList<ClassDailyAttendanceSummary> Create(IList<DailySectionAttendanceSummary> dailySectionAttendances, IList<ClassDetails> classes)
        {
            var res = new List<ClassDailyAttendanceSummary>();
            var sectionAttensDic = dailySectionAttendances.GroupBy(x => x.SectionId).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var kv in sectionAttensDic)
            {
                var cClass = classes.FirstOrDefault(c => c.Id == kv.Key);
                if(cClass != null)
                    res.Add(Create(kv.Value, cClass));
            }
            return res;
        } 

        public static ClassDailyAttendanceSummary Create(IList<DailySectionAttendanceSummary> dailySectionAttendances, Class cClass)
        {
            var res = new ClassDailyAttendanceSummary {Class = cClass, DailyAttendances = new List<DailyAttendanceSummary>()};
            var dateAttsDic = dailySectionAttendances.Where(x => x.SectionId == cClass.Id).GroupBy(x => x.Date)
                                                     .ToDictionary(x => x.Key, x => x.ToList());
            foreach (var kv in dateAttsDic)
            {
                var att = kv.Value.First();
                res.DailyAttendances.Add(DailyAttendanceSummary.Create(kv.Key, att.Tardies, att.Absences));
            }
            return res;
        }
    }

    public class StudentAttendanceSummary
    {
        public StudentDetails Student { get; set; }
        public IList<StudentClassAttendanceSummary> ClassAttendanceSummaries { get; set; }

        public static IList<StudentAttendanceSummary> Create(IList<StudentSectionAttendanceSummary> studentSectionAttendances, IList<StudentDetails> students, IList<ClassDetails> classes)
        {
            var stSectionAttsDic = studentSectionAttendances.GroupBy(x => x.StudentId).ToDictionary(x => x.Key, x => x.ToList());
            var res = new List<StudentAttendanceSummary>();
            foreach (var stSectionAtts in stSectionAttsDic)
            {
                var student = students.FirstOrDefault(x => x.Id == stSectionAtts.Key);
                if(student == null) continue;
                res.Add(new StudentAttendanceSummary
                    {
                        Student = student,
                        ClassAttendanceSummaries = StudentClassAttendanceSummary.Create(stSectionAtts.Value, classes)
                    });
            }
            return res;
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
                StudentId = x.StudentId
            }).ToList();
        } 
    }
}
