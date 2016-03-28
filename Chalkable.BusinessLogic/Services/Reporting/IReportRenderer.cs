using Chalkable.BusinessLogic.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public interface IReportRenderer
    {
        byte[] Render(object dataSet, string reportDefinition, ReportingFormat format, int? page);
    }
}
