using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model.Reports.ReportCards
{
    public class Student
    {
        public Student()
        {
            Sections = new List<ReportCardSectionData>();
            Recipients = new List<ReportCardAddressData>();
        }
        public string AltStudentNumber { get; set; }
        public IEnumerable<ReportCardAddressData> Attendance { get; set; }
        public int? ClassRank { get; set; }
        public string Counselor { get; set; }
        public decimal? CumulativeCredit { get; set; }
        public decimal? CumulativeGPA { get; set; }
        public int Demerits { get; set; }
        public string GradeLevel { get; set; }
        public decimal GPA { get; set; }
        public string HomeroomName { get; set; }
        public string HomeroomTeacher { get; set; }
        public int Merits { get; set; }
        public string Name { get; set; }
        public bool Promoted { get; set; }
        public IEnumerable<ReportCardAddressData> Recipients { get; set; }
        public IEnumerable<ReportCardSectionData> Sections { get; set; }
        public string StateIDNumber { get; set; }
        public int StudentId { get; set; }
        public string StudentNumber { get; set; }
        public int? GradingScaleId { get; set; }
        public int? StandardGradingScaleId { get; set; }
    }
}
