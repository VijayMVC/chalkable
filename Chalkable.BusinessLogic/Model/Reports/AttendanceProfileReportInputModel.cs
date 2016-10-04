using System;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class AttendanceProfileReportInputModel : BaseReportInputModel
    {
        public IntList MarkingPeriodIds { get; set; }
        public IntList AbsenceReasons { get; set; }

        public bool DisplayPeriodAbsences { get; set; }
        public bool DisplayNote { get; set; }
        public bool DisplayReasonTotals { get; set; }
        public bool IncludeUnlisted { get; set; }
        public bool IncludeCheckInCheckOut { get; set; }
        ///<summary>
        /// enum GroupByMethod
        ///    BlankColumn = 0,
        ///    GradeLevelSequence = 1,
        ///    HomeroomName = 3
        ///</summary>
        public int GroupBy { get; set; }
        public bool DisplayWithdrawnStudents { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
                
    }
}
