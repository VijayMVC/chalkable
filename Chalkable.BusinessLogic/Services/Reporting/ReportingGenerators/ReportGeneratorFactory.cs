using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class ReportGeneratorFactory
    {
        private static IDictionary<Type, Object> _generatorDictionary;

        private static IDictionary<Type, Object> BuildGaneratorDictionary(
            IServiceLocatorSchool serviceLocatorSchool,
            ConnectorLocator connectorLocator)
        {
            return new Dictionary<Type, object>
            {
                {typeof(GradebookReportInputModel), new GradebookReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(WorksheetReportInputModel), new WorksheetReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(ProgressReportInputModel), new ProgressReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(ComprehensiveProgressInputModel), new ComprehensiveProgressReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(StudentComprehensiveReportInputModel), new StudentComprehensiveReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(MissingAssignmentsInputModel), new MissingAssignmentsReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(AttendanceProfileReportInputModel), new AttendanceProfileReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(AttendanceRegisterInputModel), new AttendanceRegisterReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(BirthdayReportInputModel), new BirthdayReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(SeatingChartReportInputModel), new SeatingChartReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(GradeVerificationInputModel), new GradeVerificationReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(LessonPlanReportInputModel), new LessonPlanReportGenerator(serviceLocatorSchool, connectorLocator)},
                {typeof(FeedReportInputModel), new FeedReportGenerator(serviceLocatorSchool, connectorLocator) },
                {typeof(ReportCardsInputModel), new ReportCardGenerator(serviceLocatorSchool, connectorLocator) },
                {typeof(LunchCountReportInputModel), new LunchCountReportGenerator(serviceLocatorSchool, connectorLocator) }
            };
        }

        public static IReportGenerator<TReportSettings> CreateGenerator<TReportSettings>(
            IServiceLocatorSchool serviceLocatorSchool,
            ConnectorLocator connectorLocator
        )
            where TReportSettings : class
        {
            if (_generatorDictionary == null)
                _generatorDictionary = BuildGaneratorDictionary(serviceLocatorSchool, connectorLocator);
            if (!_generatorDictionary.ContainsKey(typeof(TReportSettings)))
                throw new ChalkableException("Could not find report generator for such input data");
            var res = _generatorDictionary[typeof(TReportSettings)] as IReportGenerator<TReportSettings>;
            if (res == null)
                throw new ChalkableException("Current report generator doesn't implement IReportGenerator");
            return res;
        }
    }


}
