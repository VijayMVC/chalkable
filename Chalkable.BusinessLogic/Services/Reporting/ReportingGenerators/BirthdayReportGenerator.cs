using System;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class BirthdayReportGenerator : InowReportGenerator<BirthdayReportInputModel, BirthdayReportParams>
    {
        public BirthdayReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override BirthdayReportParams CreateInowReportSettings(BirthdayReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            return new BirthdayReportParams
            {
                AcadSessionId = gp.SchoolYearRef,
                EndDate = inputModel.EndDate,
                StartDate = inputModel.StartDate,
                EndMonth = inputModel.EndMonth,
                StartMonth = inputModel.StartMonth,
                GroupBy = inputModel.GroupBy,
                IncludePhoto = inputModel.IncludePhoto,
                IncludeWithdrawn = inputModel.IncludeWithdrawn,
                SectionId = inputModel.ClassId
            };
        }

        protected override Func<BirthdayReportParams, byte[]> InowGenerateReportFunc
            => ConnectorLocator.ReportConnector.BirthdayReport;
    }
}
