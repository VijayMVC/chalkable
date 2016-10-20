using System;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class LunchCountReportInputModel
    {
        public int IdToPrint { get; set; }
        public IntList StudentIds { get; set; }
        public IntList GroupIds { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int OrderBy { get; set; }
        public bool WithCountsOnly { get; set; }
        public IntList IncludeOptions { get; set; }
        private bool HasOption(LunchCountReportAddionalOptions option)
        {
            return IncludeOptions != null && IncludeOptions.Contains((int)option);
        }
        public bool IncludeGroupTotals => HasOption(LunchCountReportAddionalOptions.GroupTotals);
        public bool IncludeGrandTotals => HasOption(LunchCountReportAddionalOptions.GrandTotals);
        public bool IncludeStudentsOnly => HasOption(LunchCountReportAddionalOptions.StudentsOnly);
        public bool IncludeSummaryOnly => HasOption(LunchCountReportAddionalOptions.SummaryOnly);

    }

    public enum LunchCountReportAddionalOptions
    {
        GroupTotals = 1,
        GrandTotals = 2,
        StudentsOnly = 3,
        SummaryOnly = 4
    }
}
