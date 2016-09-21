using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class AttendanceRegisterInputModel : BaseReportInputModel
    {
        public IntList AbsenceReasonIds { get; set; }
        public bool IncludeTardies { get; set; }
        public int MonthId { get; set; }
        public int ReportType { get; set; }
        public bool ShowLocalReasonCode { get; set; }
    }
}
