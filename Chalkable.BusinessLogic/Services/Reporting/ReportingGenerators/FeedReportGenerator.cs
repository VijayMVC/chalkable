using System;
using System.IO;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class FeedReportGenerator : BaseReportGenerator<FeedReportInputModel>
    {
        public FeedReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator)
            : base(serviceLocatorSchool, connectorLocator)
        {
        }

        public override byte[] GenerateReport(FeedReportInputModel inputModel)
        {
            if (inputModel.Settings == null)
                throw new ChalkableException("Empty report settings parameter");

            ValidateDateRange(inputModel.Settings.StartDate, inputModel.Settings.EndDate);

            IReportHandler<FeedReportInputModel> handler;
            if (inputModel.Settings.IncludeDetails)
                handler = new FeedDetailsReportHandler();
            else
                handler = new ShortFeedReportHandler();

            var format = inputModel.FormatTyped ?? ReportingFormat.Pdf;
            var dataSet = handler.PrepareDataSource(inputModel, format, ServiceLocator, ServiceLocator.ServiceLocatorMaster);
            var definition = Path.Combine(inputModel.DefaultPath, handler.ReportDefinitionFile);
            if (!File.Exists(definition))
                throw new ChalkableException(string.Format(ChlkResources.ERR_REPORT_DEFINITION_FILE_NOT_FOUND, definition));

            return new DefaultRenderer().Render(dataSet, definition, format, null);
        }

        private static void ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new ChalkableException("Invalid date range. Start date is greater than end date");
        }
    }
}
