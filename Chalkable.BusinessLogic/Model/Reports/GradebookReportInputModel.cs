using System;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class GradebookReportInputModel : BaseReportInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int ReportType { get; set; }
        public int OrderBy { get; set; }
        public int GroupBy { get; set; }

        public bool IncludeNonGradedActivities { get; set; }
        public bool IncludeWithdrawnStudents { get; set; }

        public bool DisplayLetterGrade { get; set; }
        public bool DisplayStudentAverage { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool SuppressStudentName { get; set; }
    }
}
