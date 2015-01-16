using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class TeacherAttendanceSummaryViewData
    {
        public ShortAttendanceSummaryViewData Absent { get; set; }
        public ShortAttendanceSummaryViewData Late { get; set; }

        public static TeacherAttendanceSummaryViewData Create(AttendanceSummary attendanceSummary)
        {
            var res = new TeacherAttendanceSummaryViewData();
            var alerts = new List<string>();
            res.Absent = new ShortAttendanceSummaryViewData
                {
                    ClassesStats = ClassAttendanceStatViewData.Create(attendanceSummary.ClassesDaysStat, AttendanceTypeEnum.Absent),
                    Students = ShortStudentAttendanceViewData.Create(attendanceSummary.Students, alerts, AttendanceTypeEnum.Absent)
                };
            res.Late = new ShortAttendanceSummaryViewData
                {
                    ClassesStats = ClassAttendanceStatViewData.Create(attendanceSummary.ClassesDaysStat, AttendanceTypeEnum.Tardies),
                    Students = ShortStudentAttendanceViewData.Create(attendanceSummary.Students, alerts, AttendanceTypeEnum.Tardies)
                };
            return res;
        }
    }


    public class ShortAttendanceSummaryViewData
    {
        public IList<ClassAttendanceStatViewData> ClassesStats { get; set; }
        public IList<ShortStudentAttendanceViewData> Students { get; set; }
    }

    public class ClassAttendanceStatViewData
    {
        public ShortClassViewData Class { get; set; }
        public IList<ShortAttendanceStatViewData> DayStats { get; set; }
 
        public static IList<ClassAttendanceStatViewData> Create(IList<ClassDailyAttendanceSummary> classDailyAttendances, AttendanceTypeEnum type)
        {
            return classDailyAttendances.Select(x => new ClassAttendanceStatViewData()
                {
                    Class = ShortClassViewData.Create(x.Class),
                    DayStats = ShortAttendanceStatViewData.Create(x.DailyAttendances, type)
                }).ToList();
        }
    }

    public class ShortAttendanceStatViewData
    {
        public int StudentCount { get; set; }
        public string Summary { get; set; }
        public DateTime Date { get; set; }

        public static IList<ShortAttendanceStatViewData> Create(IList<DailyAttendanceSummary> dailyAttendanceSummaries, AttendanceTypeEnum type)
        {
            var res = new List<ShortAttendanceStatViewData>();
            string prevMonth = "";
            foreach (var dailyAttendanceSummary in dailyAttendanceSummaries)
            {
                var attCount = 0;
                if (type == AttendanceTypeEnum.Absent)
                    attCount = (int)dailyAttendanceSummary.Absences;
                if (type == AttendanceTypeEnum.Tardies)
                    attCount = dailyAttendanceSummary.Tardies;

                var date = dailyAttendanceSummary.Date;
                string summary = date.Day.ToString();
                if (prevMonth != date.ToString("MMM"))
                {
                    prevMonth = date.ToString("MMM");
                    summary = prevMonth + " " + summary;
                }
                res.Add(new ShortAttendanceStatViewData
                    {
                        Date = dailyAttendanceSummary.Date,
                        StudentCount = attCount,
                        Summary = summary
                    });
            }
            return res;
        }
    }

    public class ShortStudentAttendanceViewData
    {
        public StudentViewData StudentInfo { get; set; }
        public IList<string> Alerts { get; set; }
        public IList<AttendanceStatByClassViewData> StatByClass { get; set; }
        public int TotalAttendanceCount { get; set; }

        private ShortStudentAttendanceViewData(StudentDetails student, IList<string> alerts, IList<AttendanceStatByClassViewData> statByClassView)
        {
            StudentInfo = StudentViewData.Create(student);
            Alerts = alerts;
            StatByClass = statByClassView;
            TotalAttendanceCount = statByClassView.Sum(x => x.AttendanceCount);
        }

        public static IList<ShortStudentAttendanceViewData> Create(IList<StudentAttendanceSummary> studentAttendanceSummaries, IList<string> alerts, AttendanceTypeEnum type)
        {
            var res = studentAttendanceSummaries.Select(x => new ShortStudentAttendanceViewData(x.Student, alerts
                , AttendanceStatByClassViewData.Create(x.ClassAttendanceSummaries, type))).ToList();
            
            return res.Where(x=>x.TotalAttendanceCount > 2).OrderByDescending(x=>x.TotalAttendanceCount).ToList();
        }
    }

    public class AttendanceStatByClassViewData
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int AttendanceCount { get; set; }

        public static AttendanceStatByClassViewData Create(int studentId, Class cClass, int attendanceCount)
        {
            return new AttendanceStatByClassViewData
                {
                    AttendanceCount = attendanceCount,
                    StudentId = studentId,
                    ClassId = cClass.Id,
                    ClassName = cClass.Name
                };
        }
        public static AttendanceStatByClassViewData Create(StudentClassAttendanceSummary studentClassAttendance, AttendanceTypeEnum type)
        {
            var attCount = 0;
            if (type == AttendanceTypeEnum.Absent)
                attCount = (int) studentClassAttendance.Absences;
            if (type == AttendanceTypeEnum.Tardies)
                attCount = studentClassAttendance.Tardies;
            return Create(studentClassAttendance.StudentId, studentClassAttendance.Class, attCount);
        }

        public static IList<AttendanceStatByClassViewData> Create(IList<StudentClassAttendanceSummary> studentsClassAttendance, AttendanceTypeEnum type)
        {
            return studentsClassAttendance.Select(x => Create(x, type)).ToList();
        }
    }

    public enum AttendanceTypeEnum
    {
        Absent = 0,
        Tardies = 1,
    }
}