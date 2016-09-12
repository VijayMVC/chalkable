using Chalkable.BusinessLogic.Model.Reports;
using Microsoft.Reporting.WebForms;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public class DefaultRenderer : IReportRenderer
    {
        public byte[] Render(object dataSet, string reportDefinition, ReportingFormat format, int? page)
        {
            var report = new LocalReport {ReportPath = reportDefinition};
            var dataSource = new ReportDataSource("MainDataSet", dataSet);
            report.DataSources.Add(dataSource);

            string deviceInfo =
              "<DeviceInfo>" +
              "  <PageWidth>9in</PageWidth>" +
              "  <PageHeight>12in</PageHeight>" +
              "  <MarginTop>0.05in</MarginTop>" +
              "  <MarginLeft>0.05in</MarginLeft>" +
              "  <MarginRight>0.05in</MarginRight>" +
              "  <MarginBottom>0.05in</MarginBottom>" +
              "</DeviceInfo>";
            string fmt = format.AsString();
            var res = report.Render(fmt, deviceInfo);
            return res;
        }
    }
}
