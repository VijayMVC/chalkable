using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    //TODO: delete this model after removing old version api method 
    public class StudentClassAttendanceOldViewData
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

        public bool FullClassReadOnly { get; set; }
        public string FullClassReadOnlyReason { get; set; }

        public bool IsDailyAttendancePeriod { get; set; }

        public static StudentClassAttendanceOldViewData Create(ClassAttendanceDetails attendance, StudentClassAttendance studentAttendance, AttendanceReason reason)
        {
            var res = new StudentClassAttendanceOldViewData
                {
                    PersonId = studentAttendance.StudentId,
                    ClassId = attendance.Class.Id,
                    ClassName = attendance.Class.Name,
                    Date = studentAttendance.Date,
                    AttendanceReasonId = studentAttendance.AttendanceReasonId,
                    Student = StudentViewData.Create(studentAttendance.Student),
                    Level = studentAttendance.Level,
                    IsPosted = attendance.IsPosted,
                    AbsentPreviousDay = studentAttendance.AbsentPreviousDay,
                    ReadOnly = studentAttendance.ReadOnly,
                    ReadOnlyReason = studentAttendance.ReadOnlyReason,
                    FullClassReadOnly = attendance.ReadOnly,
                    FullClassReadOnlyReason = attendance.ReadOnlyReason,
                    IsDailyAttendancePeriod = attendance.IsDailyAttendancePeriod
                };
            if (reason != null)
                res.AttendanceReason = AttendanceReasonViewData.Create(reason);
            return res;
        }

        public static IList<StudentClassAttendanceOldViewData> Create(ClassAttendanceDetails attendance, IList<AttendanceReason> attendanceReasons = null)
        {
            var res = new List<StudentClassAttendanceOldViewData>();
            if (attendance != null && attendance.StudentAttendances.Count > 0)
            {
                foreach (var studentAtt in attendance.StudentAttendances)
                {
                    AttendanceReason reason = null;
                    if (attendanceReasons != null && attendanceReasons.Count > 0 && studentAtt.AttendanceReasonId.HasValue)
                        reason = attendanceReasons.FirstOrDefault(x => x.Id == studentAtt.AttendanceReasonId);
                    res.Add(Create(attendance, studentAtt, reason));
                }    
            }
            return res;
        }
    }


    
    //New attendance structure models , for new api version

    public class StudentClassAttendanceViewData
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime Date { get; set; }
        public string Level { get; set; }
        public StudentViewData Student { get; set; }
        public int? AttendanceReasonId { get; set; }
        public AttendanceReasonViewData AttendanceReason { get; set; }
        public bool ReadOnly { get; set; }
        public string ReadOnlyReason { get; set; }
        public bool AbsentPreviousDay { get; set; }

        protected StudentClassAttendanceViewData(StudentClassAttendance studentAttendance)
        {
            StudentId = studentAttendance.StudentId;
            ClassId = studentAttendance.ClassId;
            AbsentPreviousDay = studentAttendance.AbsentPreviousDay;
            AttendanceReasonId = studentAttendance.AttendanceReasonId;
            Student = StudentViewData.Create(studentAttendance.Student);
            ReadOnly = studentAttendance.ReadOnly;
            ReadOnlyReason = studentAttendance.ReadOnlyReason;
            Date = studentAttendance.Date;
            Level = studentAttendance.Level;
        }

        protected StudentClassAttendanceViewData(StudentClassAttendance studentAttendance, AttendanceReason attendanceReason) : this(studentAttendance)
        {
            if (attendanceReason != null)
                AttendanceReason = AttendanceReasonViewData.Create(attendanceReason);
        }

        public static StudentClassAttendanceViewData Create(StudentClassAttendance studentAttendance, AttendanceReason attendanceReason)
        {
            var res = new StudentClassAttendanceViewData(studentAttendance);
            if (attendanceReason != null)
                res.AttendanceReason = AttendanceReasonViewData.Create(attendanceReason);
            return res;
        }

        public static IList<StudentClassAttendanceViewData> Create(IList<StudentClassAttendance> studentAttendances, IList<AttendanceReason> attendanceReasons = null)
        {
            var res = new List<StudentClassAttendanceViewData>();
            foreach (var attendance in studentAttendances)
            {
                AttendanceReason reason;
                if (attendanceReasons != null && attendanceReasons.Count > 0 && attendance.AttendanceReasonId.HasValue)
                    reason = attendanceReasons.First(x => x.Id == attendance.AttendanceReasonId);
                else reason = null;
                res.Add(Create(attendance, reason));
            }
            return res;
        }
    }

    public class ClassAttendanceViewData
    {
        public DateTime Date { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public bool IsPosted { get; set; }
        public bool ReadOnly { get; set; }
        public string ReadOnlyReason { get; set; }
        public bool IsDailyAttendancePeriod { get; set; }

        public IList<StudentClassAttendanceViewData> StudentAttendances { get; set; }

        public static ClassAttendanceViewData Create(int classId, DateTime date)
        {
            return new ClassAttendanceViewData
                {
                    ClassId = classId,
                    Date = date,
                    StudentAttendances = new List<StudentClassAttendanceViewData>()
                };
        }

        public static ClassAttendanceViewData Create(ClassAttendanceDetails classAttendance, IList<AttendanceReason> attendanceReasons)
        {
            var res = new ClassAttendanceViewData
                {
                    ClassId = classAttendance.Class.Id,
                    ClassName = classAttendance.Class.Name,
                    Date = classAttendance.Date,
                    IsPosted = classAttendance.IsPosted,
                    ReadOnly = classAttendance.ReadOnly,
                    ReadOnlyReason = classAttendance.ReadOnlyReason,
                    IsDailyAttendancePeriod = classAttendance.IsDailyAttendancePeriod,
                    StudentAttendances = StudentClassAttendanceViewData.Create(classAttendance.StudentAttendances, attendanceReasons)
                };
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
            public bool IsDailyAttendancePeriod { get; set; }
        }

        public int ClassId { get; set; }
        public DateTime Date { get; set; }
        public IList<SetClassAttendanceViewDataItem> Items { get; set; }
    }
}