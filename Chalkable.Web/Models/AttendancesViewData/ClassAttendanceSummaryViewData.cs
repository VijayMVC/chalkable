namespace Chalkable.Web.Models.AttendancesViewData
{
    public class ClassAttendanceSummaryViewData
    {
        public ClassAttendanceSummaryHoverBox Absences { get; set; }
        public ClassAttendanceSummaryHoverBox Lates { get; set; }
        public ClassAttendanceSummaryHoverBox Presents { get; set; }
    }
    
    public class ClassAttendanceSummaryHoverBox : HoverBoxesViewData<ClassAttendnaceHoverBoxItem>
    {
        public bool IsPassing { get; set; }
    }
    
    public class ClassAttendnaceHoverBoxItem
    {
        public int AttendanceCount { get; set; }
        public string StudentName { get; set; }
    }
}