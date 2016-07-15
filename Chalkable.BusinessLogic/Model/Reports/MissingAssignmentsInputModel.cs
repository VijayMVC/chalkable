using System;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class MissingAssignmentsInputModel : BaseReportInputModel
    {
        public IntList AlternateScoreIds { get; set; }
        public bool AlternateScoresOnly { get; set; }
        public bool ConsiderZerosAsMissingGrades { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }

        public bool IncludeWithdrawn { get; set; }
        public bool OnePerPage { get; set; }
        public int OrderBy { get; set; }
        public bool SuppressStudentName { get; set; }
    }
}
