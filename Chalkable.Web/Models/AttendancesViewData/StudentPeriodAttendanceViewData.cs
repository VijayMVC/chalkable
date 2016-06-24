using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class StudentPeriodAttendanceViewData : StudentClassAttendanceViewData
    {
        public string ClassName { get; set; }
        public int? TeacherId { get; set; }
        public int PeriodOrder { get; set; }
        public int PeriodId { get; set; }

        protected StudentPeriodAttendanceViewData(StudentPeriodAttendance studentAttendance, AttendanceReason reason)
            : base(studentAttendance, reason)
        {
            ClassName = studentAttendance.Class.Name;
            PeriodId = studentAttendance.Period.Id;
            PeriodOrder = studentAttendance.Period.Order;
            TeacherId = studentAttendance.Class.PrimaryTeacherRef;
        }
        

        public static IList<StudentPeriodAttendanceViewData> Create(IList<StudentPeriodAttendance> studentPeriodAttendances, IList<AttendanceReason> reasons)
        {
            return studentPeriodAttendances.Select(att =>
                {
                    var reason = reasons.FirstOrDefault(r => r.Id == att.AttendanceReasonId);
                    return new StudentPeriodAttendanceViewData(att, reason);
                }).ToList();
        }
    }
}