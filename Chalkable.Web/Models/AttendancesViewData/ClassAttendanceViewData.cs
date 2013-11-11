using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class ClassAttendanceViewData
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int ClassPeriodId { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public PeriodViewData Period { get; set; }
        public ShortPersonViewData Student { get; set; }
        public int? AttendanceReasonId { get; set; }
        public AttendanceReasonViewData AttendanceReason { get; set; }

        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public static ClassAttendanceViewData Create(ClassAttendanceDetails attendance, AttendanceReason reason)
        {
            var res = new ClassAttendanceViewData
                {
                    PersonId = attendance.PersonRef,
                    ClassId = attendance.Class.Id,
                    ClassName = attendance.Class.Name,
                    Date = attendance.Date,
                    AttendanceReasonId = attendance.AttendanceReasonRef,
                    Student = ShortPersonViewData.Create(attendance.Student),
                    Type = (int)attendance.Type
                };
            if (reason != null)
                res.AttendanceReason = AttendanceReasonViewData.Create(reason);
            return res;
        }

        public static IList<ClassAttendanceViewData> Create(IList<ClassAttendanceDetails> attendances, IList<AttendanceReason> attendanceReasones = null)
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