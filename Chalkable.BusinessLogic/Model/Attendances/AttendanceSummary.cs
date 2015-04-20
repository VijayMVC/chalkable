using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Model.Attendances
{
    public class AttendanceSummary
    {
        public IList<ClassDailyAttendanceSummary> ClassesDaysStat { get; set; }
        public IList<ShortStudentAttendanceSummary> Students { get; set; }
    }

    public class DailyAttendanceSummary
    {
        public decimal Absences { get; set; }
        public DateTime Date { get; set; }
        public int Tardies { get; set; }

        public static DailyAttendanceSummary Create(DateTime date, int tardies, decimal absences)
        {
            return new DailyAttendanceSummary { Absences = absences, Tardies = tardies, Date = date };
        }
    }

    public class ClassDailyAttendanceSummary
    {
        public Class Class { get; set; }
        public IList<DailyAttendanceSummary> DailyAttendances { get; set; }

        public static IList<ClassDailyAttendanceSummary> Create(IList<DailySectionAbsenceSummary> dailySectionAttendances, IList<ClassDetails> classes)
        {
            var res = new List<ClassDailyAttendanceSummary>();
            var sectionAttensDic = dailySectionAttendances.GroupBy(x => x.SectionId).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var kv in sectionAttensDic)
            {
                var cClass = classes.FirstOrDefault(c => c.Id == kv.Key);
                if (cClass != null)
                    res.Add(Create(kv.Value, cClass));
            }
            return res;
        }

        public static ClassDailyAttendanceSummary Create(IList<DailySectionAbsenceSummary> dailySectionAttendances, Class cClass)
        {
            var res = new ClassDailyAttendanceSummary { Class = cClass, DailyAttendances = new List<DailyAttendanceSummary>() };
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

    public class ShortStudentAttendanceSummary
    {
        public StudentDetails Student { get; set; }
        public IList<StudentClassAttendanceSummary> ClassAttendanceSummaries { get; set; }

        public static IList<ShortStudentAttendanceSummary> Create(IList<StudentSectionAbsenceSummary> studentSectionAttendances, IList<StudentDetails> students, IList<ClassDetails> classes)
        {
            var stSectionAttsDic = studentSectionAttendances.GroupBy(x => x.StudentId).ToDictionary(x => x.Key, x => x.ToList());
            var res = new List<ShortStudentAttendanceSummary>();
            foreach (var stSectionAtts in stSectionAttsDic)
            {
                var student = students.FirstOrDefault(x => x.Id == stSectionAtts.Key);
                if (student == null) continue;
                res.Add(new ShortStudentAttendanceSummary
                {
                    Student = student,
                    ClassAttendanceSummaries = StudentClassAttendanceSummary.Create(stSectionAtts.Value, classes)
                });
            }
            return res;
        }
    }


    public class FullStudentAttendanceSummary : ShortStudentAttendanceSummary
    {
        public StudentDailyAttendanceSummary DailyAttendanceSummary { get; set; }

        //public static FullStudentAttendanceSummary Create(StudentAttendanceSummaryDashboard studentAttendanceSummary, StudentDetails student, IList<ClassDetails> classDetailses)
        //{
        //    return new FullStudentAttendanceSummary
        //        {
        //            Student = student,
        //            DailyAttendanceSummary = StudentDailyAttendanceSummary.Create(studentAttendanceSummary.DailyAttendance),
        //            ClassAttendanceSummaries = StudentClassAttendanceSummary.Create(studentAttendanceSummary.PeriodAttendance.ToList(), classDetailses)
        //        };
        //}
        
    }

    public class StudentDailyAttendanceSummary
    {
        public int? Tardies { get; set; }
        public decimal? Absences { get; set; }
        public int StudentId { get; set; }
        public decimal? Presents { get; set; }

        public static StudentDailyAttendanceSummary Create(StudentDailyAbsenceSummary absenceSummary)
        {
            return new StudentDailyAttendanceSummary
                {
                    Absences = absenceSummary.Absences,
                    StudentId = absenceSummary.StudentId,
                    Presents = absenceSummary.Presents,
                    Tardies = absenceSummary.Tardies
                };
        }
    }
}
