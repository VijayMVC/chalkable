using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class ClassAttendanceViewData
    {
        public Guid Id { get; set; }
        public Guid ClassPersonId { get; set; }
        public Guid ClassPeriodId { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public PeriodViewData Period { get; set; }
        public ShortPersonViewData Student { get; set; }
        public Guid? AttendanceReasonId { get; set; }
        public AttendanceReasonViewData AttendanceReason { get; set; }

        public Guid TeacherId { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }

        public static ClassAttendanceViewData Create(ClassAttendanceComplex attendance, AttendanceReason reason)
        {
            var res = new ClassAttendanceViewData
                {
                    Id = attendance.Id,
                    ClassPersonId = attendance.ClassPersonRef,
                    ClassPeriodId = attendance.ClassPeriodRef,
                    ClassId = attendance.Class.Id,
                    ClassName = attendance.Class.Name,
                    Date = attendance.Date,
                    AttendanceReasonId = attendance.AttendanceReasonRef,
                    Period = PeriodViewData.Create(attendance.ClassPeriod.Period),
                    Student = ShortPersonViewData.Create(attendance.Student)
                };
            if (reason != null)
                res.AttendanceReason = AttendanceReasonViewData.Create(reason);
            return res;
        }

        public static IList<ClassAttendanceViewData> Create(IList<ClassAttendanceComplex> attendances, IList<AttendanceReason> attendanceReasones = null)
        {
            var res = new List<ClassAttendanceViewData>();
            foreach (var attendance in attendances)
            {
                AttendanceReason reason;
                if (attendanceReasones != null && attendanceReasones.Count > 0 && attendance.AttendanceReasonRef.HasValue)
                    reason = attendanceReasones.First(x => x.Id == attendance.AttendanceReasonRef);
                else reason = null;
                res.Add(Create(attendance, reason));
            }
            return res;
        }
    }
}