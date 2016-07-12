using System;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;
using System.Collections.Generic;
using System.Linq;

namespace Chalkable.Web.Models
{
    public class StudentDailyAttendanceViewData
    {
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public int StudentId { get; set; }
        public int? AttendanceReasonId { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public bool IsAbsent { get; set; }
        public bool IsLate { get; set; }
        public bool IsAbsentOrLate { get; set; }
        public bool IsExcused { get; set; }

        public static StudentDailyAttendanceViewData Create(StudentDailyAttendance studentAttendance, IList<AttendanceReason> reasons)
        {
            if (studentAttendance == null)
                return null;
            var reason = reasons.FirstOrDefault(r => r.Id == studentAttendance.AttendanceReasonId);
            return new StudentDailyAttendanceViewData
            {
                Date = studentAttendance.Date,
                Note = studentAttendance.Note,
                StudentId = studentAttendance.StudentId,
                AttendanceReasonId = reason?.Id,
                Category = reason?.Category,
                Level = studentAttendance.Level,
                IsAbsent = studentAttendance.IsAbsent,
                IsLate = studentAttendance.IsLate,
                IsAbsentOrLate = studentAttendance.IsAbsentOrLate,
                IsExcused = studentAttendance.IsExcused
            };
        }
    }
}