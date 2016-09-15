using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model.Reports.ReportCards
{
    public class ReportCardAttendanceData
    {
        public decimal DaysEnrolled { get; set; }
        public decimal ExcusedAbsences { get; set; }
        public decimal ExcusedTardies { get; set; }
        public int GradingPeriodId { get; set; }
        public decimal UnexcusedAbsences { get; set; }
        public decimal UnexcusedTardies { get; set; }
    }
}
