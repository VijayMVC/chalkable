using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class CheckInCheckOutViewData
    {
        public bool IsCheckIn { get; set; }
        public int? CheckInReasonId { get; set; }
        public int? CheckInTime { get; set; }

        public static IList<CheckInCheckOutViewData> Create(IList<CheckInCheckOut> checkIns)
        {
            if (checkIns == null)
                return new List<CheckInCheckOutViewData>();

            return checkIns.Select(checkIn => new CheckInCheckOutViewData()
            {
                CheckInReasonId = checkIn.AttendanceReasonId,
                CheckInTime = checkIn.Time,
                IsCheckIn = checkIn.IsCheckIn,
               
            }).ToList();
        }
    }

    public class StudentDateAttendanceViewData
    {
        public DateTime Date { get; set; }
        public StudentViewData Student { get; set; }
       

        public IList<StudentPeriodAttendanceViewData> PeriodAttendances { get; set; }
        public IList<CheckInCheckOutViewData>  CheckInCheckOuts { get; set; }

        public static StudentDateAttendanceViewData Create(StudentDateAttendance attendance, IList<AttendanceReason> reasons)
        {
            var res = new StudentDateAttendanceViewData
            {
                Date = attendance.Date,
                Student = StudentViewData.Create(attendance.Student),
                PeriodAttendances = StudentPeriodAttendanceViewData.Create(attendance.StudentPeriodAttendances, reasons),
                CheckInCheckOuts = CheckInCheckOutViewData.Create(attendance.CheckInCheckOuts)
            };
            
            return res;
        }
    }
}