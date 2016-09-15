using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model.Reports.ReportCards
{
    public class GradingScale
    {
        public GradingScale()
        {
            Ranges = new List<GradingScaleRange>();
        }
        public string Description { get; set; }
        public string Name { get; set; }
        public List<GradingScaleRange> Ranges { get; set; }

    }

    public class GradingScaleRange
    {
        public string AlphaGrade { get; set; }
        public short AveragingEquivalent { get; set; }
        public bool AwardGradeCredit { get; set; }
        public decimal HighValue { get; set; }
        public bool IsPassing { get; set; }
        public decimal LowValue { get; set; }
    }
}
