using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class ReportMailDelivery
    {
        public Guid Id { get; set; }
        public ReportType ReportType { get; set; }
        public int Format { get; set; }
        public int Frequency { get; set; }
        public Guid PersonRef { get; set; }
        public int? SendHour { get; set; }
        public int? SendDay { get; set; }
        public Guid? LastSentMarkingPeriodRef { get; set; }
        public DateTime? LastSentTime { get; set; }
    }
}
