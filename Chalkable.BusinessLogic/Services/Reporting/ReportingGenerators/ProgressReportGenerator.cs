using System;
using System.Linq;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class ProgressReportGenerator : InowReportGenerator<ProgressReportInputModel, ProgressReportParams>
    {
        public ProgressReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override ProgressReportParams CreateInowReportSettings(ProgressReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var stiModel = new ProgressReportParams
            {
                AcadSessionId = gp.SchoolYearRef,
                AbsenceReasonIds = inputModel.AbsenceReasonIds.ToArray(),
                AdditionalMailings = inputModel.AdditionalMailings,
                DailyAttendanceDisplayMethod = inputModel.DailyAttendanceDisplayMethod,
                DisplayCategoryAverages = inputModel.DisplayCategoryAverages,
                DisplayClassAverages = inputModel.DisplayClassAverages,
                DisplayLetterGrade = inputModel.DisplayLetterGrade,
                DisplayPeriodAttendance = inputModel.DisplayPeriodAttendance,
                DisplaySignatureLine = inputModel.DisplaySignatureLine,
                DisplayStudentComments = inputModel.DisplayStudentComments,
                DisplayStudentMailingAddress = inputModel.DisplayStudentMailingAddress,
                DisplayTotalPoints = inputModel.DisplayTotalPoints,
                SectionId = inputModel.ClassId,
                GoGreen = inputModel.GoGreen,
                GradingPeriodId = inputModel.GradingPeriodId,
                IdToPrint = inputModel.IdToPrint,
                PrintFromHomePortal = inputModel.PrintFromHomePortal,
                MaxCategoryClassAverage = inputModel.MaxCategoryClassAverage,
                MinCategoryClassAverage = inputModel.MinCategoryClassAverage,
                MaxStandardAverage = inputModel.MaxStandardAverage,
                MinStandardAverage = inputModel.MinStandardAverage,
                SectionComment = inputModel.ClassComment,
            };
            //TODO: maiby remove this later after inow fix
            if (inputModel.StudentIds == null)
            {
                var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef);
                stiModel.StudentIds = students.Select(x => x.Id).ToArray();
            }
            else stiModel.StudentIds = inputModel.StudentIds.ToArray();
            return stiModel;
        }

        protected override Func<ProgressReportParams, byte[]> InowGenerateReportFunc => ConnectorLocator.ReportConnector.ProgressReport;
    }
}
