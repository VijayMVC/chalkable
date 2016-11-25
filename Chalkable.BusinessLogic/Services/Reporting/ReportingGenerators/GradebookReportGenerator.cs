using System;
using System.Linq;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class GradebookReportGenerator : InowReportGenerator<GradebookReportInputModel, GradebookReportParams>
    {
        public GradebookReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) : base(serviceLocatorSchool, connectorLocator)
        {
        }
        protected override GradebookReportParams CreateInowReportSettings(GradebookReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var stiModel = new GradebookReportParams
            {
                AcadSessionId = gp.SchoolYearRef,
                StartDate = inputModel.StartDate,
                EndDate = inputModel.EndDate,
                DisplayStudentAverage = inputModel.DisplayStudentAverage,
                DisplayLetterGrade = inputModel.DisplayLetterGrade,
                DisplayTotalPoints = inputModel.DisplayTotalPoints,
                IncludeWithdrawnStudents = inputModel.IncludeWithdrawnStudents,
                IncludeNonGradedActivities = inputModel.IncludeNonGradedActivities,
                SuppressStudentName = inputModel.SuppressStudentName,
                OrderBy = inputModel.OrderBy,
                GroupBy = inputModel.GroupBy,
                IdToPrint = inputModel.IdToPrint,
                ReportType = inputModel.ReportType,
                GradingPeriodId = inputModel.GradingPeriodId,
                SectionId = inputModel.ClassId
            };
            if (inputModel.StudentIds == null)
            {
                var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef);
                stiModel.StudentIds = students.Select(x => x.Id).ToArray();
            }
            else stiModel.StudentIds = inputModel.StudentIds.ToArray();
            if (CoreRoles.TEACHER_ROLE == ServiceLocator.Context.Role)
                stiModel.StaffId = ServiceLocator.Context.PersonId;
            return stiModel;
        }
        protected override Func<GradebookReportParams, byte[]> InowGenerateReportFunc => ConnectorLocator.ReportConnector.GradebookReport;
    }

}
