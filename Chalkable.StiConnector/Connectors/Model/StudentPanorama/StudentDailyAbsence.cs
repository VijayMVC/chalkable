using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model.StudentPanorama
{
    public class StudentDailyAbsence
    {
        public int AcadSessionId { get; set; }
        public string AbsenceCategory { get; set; }
        public string AbsenceLevel { get; set; }
        public short AbsenceReasonId { get; set; }
        public string AbsenceReasonName { get; set; }
        public DateTime Date { get; set; }
        public int StudentId { get; set; }
    }
}
