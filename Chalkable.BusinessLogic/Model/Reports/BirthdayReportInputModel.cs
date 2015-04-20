using System;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class BirthdayReportInputModel : BaseReportInputModel
    {
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public int? StartMonth { get; set; }
        public int? EndMonth { get; set; }
        public int GroupBy { get; set; }
        public string Header { get; set; }
        public bool IncludePhoto { get; set; }
        public bool IncludeWithdrawn { get; set; }
    }
}
