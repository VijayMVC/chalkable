using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class SectionPanorama
    {
        public IEnumerable<StudentAbsence> Absences { get; set; }
        public IEnumerable<StudentGrade> Grades { get; set; }
        public IEnumerable<StudentInfraction> Infractions { get; set; }
        public IEnumerable<StudentStandardizedTest> StandardizedTests { get; set; }
    }

    public class StudentAbsence
    {
        public decimal NumberOfDaysEnrolled { get; set; }
        public decimal NumberOfAbsences { get; set; }
        public int StudentId { get; set; }
    }

    public class StudentGrade
    {
        public decimal AverageGrade { get; set; }
        public int StudentId { get; set; }
    }

    public class StudentInfraction
    {
        public int NumberOfInfractions { get; set; }
        public int StudentId { get; set; }
    }

    public class StudentStandardizedTest
    {
        public DateTime Date { get; set; }
        public string Score { get; set; }
        public int StandardizedTestComponentId { get; set; }
        public int StandardizedTestId { get; set; }
        public int StandardizedTestScoreTypeId { get; set; }
        public int StudentId { get; set; }
    }
}
