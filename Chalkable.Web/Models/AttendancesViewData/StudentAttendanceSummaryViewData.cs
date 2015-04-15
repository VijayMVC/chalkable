using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class StudentAttendanceSummaryViewData : StudentViewData
    {
        public MarkingPeriodViewData MarkingPeriod { get; set; }
        public StudentAttendanceHoverBox Absences { get; set; }
        public StudentAttendanceHoverBox Lates { get; set; }
        public StudentAttendanceHoverBox Presents { get; set; }

        protected StudentAttendanceSummaryViewData(StudentDetails student) : base(student)
        {
        }

    }

    public class StudentAttendanceHoverBox : HoverBoxesViewData<StudentAttendnaceHoverBoxItemViewData>
    {
        public bool IsPassing { get; set; }
    }

    public class StudentAttendnaceHoverBoxItemViewData
    {
        public int AttendnaceCount { get; set; }
        public string ClassName { get; set; }
    }
}