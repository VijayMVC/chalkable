using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
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
        public string Level { get; set; }
        public PeriodViewData Period { get; set; }
        public StudentViewData Student { get; set; }
        public int? AttendanceReasonId { get; set; }
        public AttendanceReasonViewData AttendanceReason { get; set; }

        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public bool IsPosted { get; set; }
        public bool AbsentPreviousDay { get; set; }

        public bool ReadOnly { get; set; }
        public string ReadOnlyReason { get; set; }

        public static ClassAttendanceViewData Create(ClassAttendanceDetails attendance, AttendanceReason reason)
        {
            var res = new ClassAttendanceViewData
                {
                    PersonId = attendance.PersonRef,
                    ClassId = attendance.Class.Id,
                    ClassName = attendance.Class.Name,
                    Date = attendance.Date,
                    AttendanceReasonId = attendance.AttendanceReasonRef,
                    Student = StudentViewData.Create(attendance.Student),
                    Level = attendance.Level,
                    IsPosted = attendance.IsPosted,
                    AbsentPreviousDay = attendance.AbsentPreviousDay,
                    ReadOnly = attendance.ReadOnly,
                    ReadOnlyReason = attendance.ReadOnlyReason
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


    //int personId, int classId, DateTime date, string level, int? attendanceReasonId
    public class SetClassAttendanceViewData
    {
        public class SetClassAttendanceViewDataItem
        {
            public int PersonId { get; set; }
            public string Level { get; set; }
            public int? AttendanceReasonId { get; set; }
            public string Category { get; set; }
        }

        public int ClassId { get; set; }
        public DateTime Date { get; set; }
        public IList<SetClassAttendanceViewDataItem> Items { get; set; }
    }
}