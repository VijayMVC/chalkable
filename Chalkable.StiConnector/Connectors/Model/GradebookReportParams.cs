using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{

    public class GradebookReportParams
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ReportType { get; set; }
        public int OrderBy { get; set; }
        public int IdToPrint { get; set; }
        public int GroupBy { get; set; }

        public bool IncludeNonGradedActivities { get; set; }
        public bool IncludeWithdrawnStudents { get; set; }

        public bool DisplayLetterGrade { get; set; }
        public bool DisplayStudentAverage { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool SuppressStudentName { get; set; }
        public int? SectionId { get; set; }
        public int? StaffId { get; set; }
        public int? GradingPeriodId { get; set; }
        public int StudentId { get; set; }
    }
}
