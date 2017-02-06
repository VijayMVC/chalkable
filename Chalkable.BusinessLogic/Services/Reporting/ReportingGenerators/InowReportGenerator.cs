using System;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public abstract class  InowReportGenerator<TReportSettings, TInowReportSettings> : BaseReportGenerator<TReportSettings>
    {
        protected InowReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) 
            : base(serviceLocatorSchool, connectorLocator)
        {
        }
        public override byte[] GenerateReport(TReportSettings settings)
        {
            var inowSettings = CreateInowReportSettings(settings);
            return InowGenerateReportFunc(inowSettings);
        }

        protected abstract TInowReportSettings CreateInowReportSettings(TReportSettings settings);
        protected abstract Func<TInowReportSettings, byte[]> InowGenerateReportFunc { get; }
        
    }

}
