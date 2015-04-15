﻿using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class GradeVerificationInputModel : BaseReportInputModel
    {
        public int? TeacherId { get; set; }
        public int? StaffId { get; set; }
        public int? StaffFilterId { get; set; }
        public string StartClassNumber { get; set; }
        public string EndClassNumber { get; set; }

        public int ClassOrder { get; set; }
        public int StudentOrder { get; set; }
        public IntList GradingPeriodIds { get; set; }
        public IntList GradedItemId { get; set; }

        public int GradeType { get; set; }
        public bool IncludeNotes { get; set; }
        public bool IncludeComments { get; set; }
        public bool IncludeLegend { get; set; }
        public bool IncludeSignature { get; set; }
        public bool IncludeWithdrawn { get; set; }
        public int NumberToDisplay { get; set; }

    }
}
