using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class StudentComprehensiveReportGenerator : IReportGenerator<StudentComprehensiveReportInputModel>
    {
        protected ConnectorLocator ConnectorLocator { get; }
        protected IServiceLocatorSchool ServiceLocator { get; }

        public StudentComprehensiveReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator)
        {
            ConnectorLocator = connectorLocator;
            ServiceLocator = serviceLocatorSchool;
        }
        public byte[] GenerateReport(StudentComprehensiveReportInputModel settings)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var ps = new StudentComprehensiveProgressParams
            {
                AcadSessionId = syId,
                GradingPeriodIds = new[] { settings.GradingPeriodId }
            };
            return ConnectorLocator.ReportConnector.StudentComprehensiveProgressReport(settings.StudentId, ps);
        }
    }
}
