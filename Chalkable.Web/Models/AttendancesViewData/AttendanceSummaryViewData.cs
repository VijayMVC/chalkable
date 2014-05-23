using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;
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
                    Stat = ShortAttendanceStatViewData.Create(attendanceSummary.DaysStat, AttendanceTypeEnum.Absent),
                    Students = ShortStudentAttendanceViewData.Create(attendanceSummary.Students, alerts, AttendanceTypeEnum.Absent)
                };
            res.Late = new ShortAttendanceSummaryViewData
                {
                    Stat = ShortAttendanceStatViewData.Create(attendanceSummary.DaysStat, AttendanceTypeEnum.Tardies),
                    Students = ShortStudentAttendanceViewData.Create(attendanceSummary.Students, alerts, AttendanceTypeEnum.Tardies)
                };
            return res;
        }
    }


    public class ShortAttendanceSummaryViewData
    {
        public IList<ShortAttendanceStatViewData> Stat { get; set; }
        public IList<ShortStudentAttendanceViewData> Students { get; set; }
    }

    public class ShortAttendanceStatViewData
    {
        public int StudentCount { get; set; }
        public string Summary { get; set; }

        public static ShortAttendanceStatViewData Create(int studentCount, DateTime day)
        {
            return new ShortAttendanceStatViewData
                {
                    StudentCount = studentCount,
                    Summary = day.Day.ToString()
                };
        }

        public static ShortAttendanceStatViewData Create(DailyAttendanceSummary dailyAttendanceSummary, AttendanceTypeEnum type)
        {
            var attCount = 0;
            if (type == AttendanceTypeEnum.Absent)
                attCount = (int)dailyAttendanceSummary.Absences;
            if (type == AttendanceTypeEnum.Tardies)
                attCount = dailyAttendanceSummary.Tardies;
            return Create(attCount, dailyAttendanceSummary.Date);
        }

        public static IList<ShortAttendanceStatViewData> Create(IList<DailyAttendanceSummary> dailyAttendanceSummaries, AttendanceTypeEnum type)
        {
            return dailyAttendanceSummaries.Select(x => Create(x, type)).ToList();
        }
    }

    public class ShortStudentAttendanceViewData
    {
        public ShortPersonViewData StudentInfo { get; set; }
        public IList<string> Alerts { get; set; }
        public IList<AttendanceStatByClassViewData> StatByClass { get; set; }

        public static IList<ShortStudentAttendanceViewData> Create(IList<StudentAttendanceSummary> studentAttendanceSummaries, IList<string> alerts, AttendanceTypeEnum type)
        {
            var res = studentAttendanceSummaries.Select(x => new ShortStudentAttendanceViewData
                {
                    StudentInfo = ShortPersonViewData.Create(x.Student),
                    Alerts = alerts,
                    StatByClass = AttendanceStatByClassViewData.Create(x.ClassAttendanceSummaries, type)
                }).ToList();
            return res;
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