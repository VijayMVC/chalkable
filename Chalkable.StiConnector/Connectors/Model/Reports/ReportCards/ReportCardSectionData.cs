using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model.Reports.ReportCards
{
    public class ReportCardSectionData
    {
        public ReportCardSectionData()
        {
            GradingPeriods = new List<StudentGradingPeriod>();
        }

        public string Name { get; set; }
        public string SectionNumber { get; set; }
        public string Teacher { get; set; }
        public decimal? TimesTardy { get; set; }
        public List<StudentGradingPeriod> GradingPeriods { get; set; }
    }
}
