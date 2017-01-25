using System;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class ComprehensiveProgressReportGenerator : InowReportGenerator<ComprehensiveProgressInputModel, ComprehensiveProgressParams>
    {
        public ComprehensiveProgressReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override ComprehensiveProgressParams CreateInowReportSettings(ComprehensiveProgressInputModel comprehensiveProgressInput)
        {
            var defaultGp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(comprehensiveProgressInput.GradingPeriodId);
            return new ComprehensiveProgressParams
            {
                EndDate = comprehensiveProgressInput.EndDate,
                StartDate = comprehensiveProgressInput.StartDate,
                AbsenceReasonIds = comprehensiveProgressInput.AbsenceReasonIds?.ToArray(),
                IdToPrint = comprehensiveProgressInput.IdToPrint,
                AcadSessionId = defaultGp.SchoolYearRef,
                GradingPeriodIds = comprehensiveProgressInput.GradingPeriodIds.ToArray(),
                AdditionalMailings = comprehensiveProgressInput.AdditionalMailings,
                ClassAverageOnly = comprehensiveProgressInput.ClassAverageOnly,
                DailyAttendanceDisplayMethod = comprehensiveProgressInput.DailyAttendanceDisplayMethod,
                DisplayCategoryAverages = comprehensiveProgressInput.DisplayCategoryAverages,
                DisplayClassAverage = comprehensiveProgressInput.DisplayClassAverage,
                DisplayPeriodAttendance = comprehensiveProgressInput.DisplayPeriodAttendance,
                DisplaySignatureLine = comprehensiveProgressInput.DisplaySignatureLine,
                DisplayStudentComment = comprehensiveProgressInput.DisplayStudentComment,
                DisplayStudentMailingAddress = comprehensiveProgressInput.DisplayStudentMailingAddress,
                DisplayTotalPoints = comprehensiveProgressInput.DisplayTotalPoints,
                MaxStandardAverage = comprehensiveProgressInput.MaxStandardAverage,
                MinStandardAverage = comprehensiveProgressInput.MinStandardAverage,
                GoGreen = comprehensiveProgressInput.GoGreen,
                OrderBy = comprehensiveProgressInput.OrderBy,
                SectionId = comprehensiveProgressInput.ClassId,
                WindowEnvelope = comprehensiveProgressInput.WindowEnvelope,
                StudentFilterId = comprehensiveProgressInput.StudentFilterId,
                StudentIds = comprehensiveProgressInput.StudentIds?.ToArray(),
                IncludePicture = comprehensiveProgressInput.IncludePicture,
                IncludeWithdrawn = comprehensiveProgressInput.IncludeWithdrawn,
            };
        }

        protected override Func<ComprehensiveProgressParams, byte[]> InowGenerateReportFunc => ConnectorLocator.ReportConnector.ComprehensiveProgressReport;
    }
}
