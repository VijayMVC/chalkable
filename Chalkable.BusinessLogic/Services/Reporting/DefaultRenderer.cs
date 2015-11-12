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
              "  <PageWidth>8.5in</PageWidth>" +
              "  <PageHeight>11in</PageHeight>" +
              "  <MarginTop>0.25in</MarginTop>" +
              "  <MarginLeft>0.25in</MarginLeft>" +
              "  <MarginRight>0.25in</MarginRight>" +
              "  <MarginBottom>0.25in</MarginBottom>" +
              "</DeviceInfo>";
            string fmt = format.AsString();
            var res = report.Render(fmt, deviceInfo);
            return res;
        }
    }
}
