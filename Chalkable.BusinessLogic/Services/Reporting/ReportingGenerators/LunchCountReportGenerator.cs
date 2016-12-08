using System;
using System.Diagnostics;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class LunchCountReportGenerator : InowReportGenerator<LunchCountReportInputModel, LunchCountReportParams>
    {
        public LunchCountReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator)
            : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override LunchCountReportParams CreateInowReportSettings(LunchCountReportInputModel settings)
        {
            Trace.Assert(ServiceLocator.Context.SchoolYearId.HasValue);
            return new LunchCountReportParams
            {
                StartDate = settings.StartDate,
                EndDate = settings.EndDate,
                IdToPrint = settings.IdToPrint,
                IncludeGrandTotals = settings.IncludeGrandTotals,
                IncludeGroupTotals = settings.IncludeGroupTotals,
                IncludeStudentsOnly = settings.IncludeStudentsOnly,
                IncludeSummaryOnly = settings.IncludeSummaryOnly,
                SortOption = settings.OrderBy,
                AcadSessionId = ServiceLocator.Context.SchoolYearId.Value,
                Title = settings.Title
            };
        }
        protected override Func<LunchCountReportParams, byte[]> InowGenerateReportFunc => ConnectorLocator.ReportConnector.LunchCountReport;
    }
}
