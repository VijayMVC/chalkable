using System;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class WorksheetReportInputModel : BaseReportInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public IntList AnnouncementIds { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Title3 { get; set; }
        public string Title4 { get; set; }
        public string Title5 { get; set; }
        public string Header { get; set; }
        public bool PrintAverage { get; set; }
        public bool PrintLetterGrade { get; set; }
        public bool PrintScores { get; set; }
        public bool PrintStudent { get; set; }
    }

}
