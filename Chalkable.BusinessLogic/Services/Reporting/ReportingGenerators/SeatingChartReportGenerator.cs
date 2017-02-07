using System;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class SeatingChartReportGenerator : InowReportGenerator<SeatingChartReportInputModel, SeatingChartReportPrams>
    {
        public SeatingChartReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override SeatingChartReportPrams CreateInowReportSettings(SeatingChartReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var c = ServiceLocator.ClassService.GetById(inputModel.ClassId);
            return new SeatingChartReportPrams
            {
                AcadSessionId = gp.SchoolYearRef,
                SectionId = c.Id,
                TermId = gp.MarkingPeriodRef,
                IncludeStudentPhoto = inputModel.DisplayStudentPhoto
            };
        }

        protected override Func<SeatingChartReportPrams, byte[]> InowGenerateReportFunc => ConnectorLocator.ReportConnector.SeatingChartReport;
    }
}
