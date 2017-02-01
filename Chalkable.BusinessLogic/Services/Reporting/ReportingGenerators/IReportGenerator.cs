using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public interface IReportGenerator<in TReportSettings>
    {
        byte[] GenerateReport(TReportSettings settings);
    }

    public abstract class BaseReportGenerator<TReportSettings> : IReportGenerator<TReportSettings>
    {
        protected ConnectorLocator ConnectorLocator { get; }
        protected IServiceLocatorSchool ServiceLocator { get; }

        protected BaseReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator)
        {
            ConnectorLocator = connectorLocator;
            ServiceLocator = serviceLocatorSchool;
        }
        public abstract byte[] GenerateReport(TReportSettings settings);
    }
}
