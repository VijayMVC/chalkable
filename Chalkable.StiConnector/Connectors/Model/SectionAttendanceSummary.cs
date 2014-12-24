using System;
using System.Collections.Generic;


namespace Chalkable.StiConnector.Connectors.Model
{
    public class SectionAttendanceSummary
    {
        public IEnumerable<DailySectionAttendanceSummary> Days { get; set; }
        public int SectionId { get; set; }
        public IEnumerable<StudentSectionAttendanceSummary> Students { get; set; }
    }

    public class DailySectionAttendanceSummary
    {
        public decimal Absences { get; set; }
        public DateTime Date { get; set; }
        public int SectionId { get; set; }
        public int Tardies { get; set; }
    }

    public class StudentSectionAttendanceSummary
    {
        public decimal Absences { get; set; }
        public int SectionId { get; set; }
        public int StudentId { get; set; }
        public int Tardies { get; set; }
    }
}
