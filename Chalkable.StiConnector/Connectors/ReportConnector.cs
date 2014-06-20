using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using Chalkable.StiConnector.Connectors.Model;
using Newtonsoft.Json;

namespace Chalkable.StiConnector.Connectors
{
    public class ReportConnector : ConnectorBase
    {
        public ReportConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public byte[] ProgressReport(ProgressReportParams ps)
        {
            var url = string.Format(BaseUrl + "reports/progress");
            var res = Download(url, ps);
            return res;
        }

        public byte[] GradebookReport(GradebookReportParams ps)
        {
            var url = string.Format(BaseUrl + "reports/gradebook");
            return Download(url, ps);
        }

        public byte[] WorksheetReport(WorksheetReportParams ps)
        {
            var url = string.Format(BaseUrl + "reports/worksheet");
            return Download(url, ps);
        }

        public class ProgressReportParams
        {
            public int AcadSessionId { get; set; }
            public int GradingPeriodId { get; set; }
            public int IdToPrint { get; set; }
            public int SectionId { get; set; }
            public int[] StudentIds { get; set; }
        }
    }
}