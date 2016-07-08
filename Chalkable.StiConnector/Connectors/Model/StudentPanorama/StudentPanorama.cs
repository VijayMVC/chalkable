using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model.SectionPanorama;

namespace Chalkable.StiConnector.Connectors.Model.StudentPanorama
{
    public class StudentPanorama
    {
        public IEnumerable<StudentDailyAbsence> DailyAbsences { get; set; }
        public IEnumerable<StudentInfraction> Infractions { get; set; }
        public IEnumerable<StudentPeriodAbsence> PeriodAbsences { get; set; }
        public IEnumerable<StudentStandardizedTest> StandardizedTests { get; set; }

    }
}
