using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class StudentDateAttendanceViewData
    {
        public DateTime Date { get; set; }
        public StudentViewData Student { get; set; }
        public bool IsCheckIn { get; set; }
        public int? CheckInReasonId { get; set; }
        public int? CheckInTime { get; set; }
        public int? ArrivalTime { get; set; }
        public IList<StudentPeriodAttendanceViewData> PeriodAttendances { get; set; }

        public static StudentDateAttendanceViewData Create(StudentDateAttendance attendance, IList<AttendanceReason> reasons)
        {
            var res = new StudentDateAttendanceViewData
            {
                Date = attendance.Date,
                Student = StudentViewData.Create(attendance.Student),
                PeriodAttendances = StudentPeriodAttendanceViewData.Create(attendance.StudentPeriodAttendances, reasons),

            };
            if (attendance.CheckInCheckOut != null)
            {
                res.IsCheckIn = true;
                res.CheckInTime = attendance.CheckInCheckOut.Time;
                res.CheckInReasonId = attendance.CheckInCheckOut.AttendanceReasonId;
            }
            return res;
        }
    }
}