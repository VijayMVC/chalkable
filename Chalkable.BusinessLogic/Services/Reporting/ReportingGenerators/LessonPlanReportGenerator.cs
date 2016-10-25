using System;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class LessonPlanReportGenerator : InowReportGenerator<LessonPlanReportInputModel, LessonPlanReportParams>
    {
        public LessonPlanReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator)
            : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override LessonPlanReportParams CreateInowReportSettings(LessonPlanReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            return new LessonPlanReportParams
            {
                AcadSessionId = gp.SchoolYearRef,
                StartDate = inputModel.StartDate,
                EndDate = inputModel.EndDate,
                IncludeActivities = inputModel.IncludeAnnouncements,
                IncludeStandards = inputModel.IncludeStandards,
                PublicPrivateText = inputModel.PublicPrivateText,
                SectionId = inputModel.ClassId,
                SortActivities = inputModel.SortItems,
                SortSections = inputModel.SortClasses,
                ActivityAttributeIds = inputModel.AnnouncementAttributes?.ToArray(),
                ActivityCategoryIds = inputModel.AnnouncementTypes?.ToArray(),
                MaxCount = inputModel.MaxCount
            };
        }

        protected override Func<LessonPlanReportParams, byte[]> InowGenerateReportFunc => ConnectorLocator.ReportConnector.LessonPlanReport;
    }
}
