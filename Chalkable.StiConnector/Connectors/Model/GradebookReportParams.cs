using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public enum OrderByMethod
    {
        StudentDisplayName = 0,
        StudentIdentifier = 1,
        SectionAverage = 2
    }
    public enum ReportFormat
    {
        Summary = 0,
        Detail = 1
    }
    public enum StudentIdentifier
    {
        None = 0,
        StudentNumber = 1,
        StateIdNumber = 2,
        AltStudentNumber = 3,
        SocialSecurityNumber = 4
    }
    public enum GroupByMethod
    {
        None = 0,
        FullSectionNumber = 1
    }

    public class GradebookReportParams
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ReportFormat ReportType { get; set; }
        public OrderByMethod OrderBy { get; set; }
        public StudentIdentifier IdToPrint { get; set; }
        public GroupByMethod GroupBy { get; set; }

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
