using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.Attendances;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class StudentPeriodAttendanceViewData : StudentClassAttendanceViewData
    {
        public string ClassName { get; set; }
        public int? TeacherId { get; set; }
        public int PeriodOrder { get; set; }
        public int PeriodId { get; set; }

        protected StudentPeriodAttendanceViewData(StudentPeriodAttendance studentAttendance)
            : base(studentAttendance)
        {
            ClassName = studentAttendance.Class.Name;
            PeriodId = studentAttendance.Period.Id;
            PeriodOrder = studentAttendance.Period.Order;
            TeacherId = studentAttendance.Class.PrimaryTeacherRef;
        }

        public static IList<StudentPeriodAttendanceViewData> Create(IList<StudentPeriodAttendance> studentPeriodAttendances)
        {
            return studentPeriodAttendances.Select(x => new StudentPeriodAttendanceViewData(x)).ToList();
        }
    }
}