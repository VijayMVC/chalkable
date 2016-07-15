using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    interface IReportHandler<in TSettings>
    {
        object PrepareDataSource(TSettings settings, ReportingFormat format, IServiceLocatorSchool serviceLocator, IServiceLocatorMaster masterLocator);
        string ReportDefinitionFile { get; }
    }
}
