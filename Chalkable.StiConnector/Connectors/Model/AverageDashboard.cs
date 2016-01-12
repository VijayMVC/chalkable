using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class AverageDashboard
    {
        public IList<StudentTotalSectionAttendance> AttendanceSummary { get; set; }
        public IList<StudentDisciplineSummary> DisciplineSummary { get; set; }
        public IList<StudentAverage> StudentAverages { get; set; } 
    }

    public class StudentTotalSectionAttendance
    {
        public decimal Absences { get; set; }
        public int SectionId { get; set; }
        public int StudentId { get; set; }
        public int Tardies { get; set; }
        public decimal DaysPresent { get; set; }
    }

    public class StudentDisciplineSummary
    {
        public int Occurrences { get; set; }
        public int InfractionId { get; set; }
        public int StudentId { get; set; }
    }
}
