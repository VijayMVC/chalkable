using System;
using System.Linq;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class MissingAssignmentsReportGenerator : InowReportGenerator<MissingAssignmentsInputModel, MissingAssignmentsParams>
    {
        public MissingAssignmentsReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override MissingAssignmentsParams CreateInowReportSettings(MissingAssignmentsInputModel missingAssignmentsInput)
        {
            bool? isEnrolled = missingAssignmentsInput.IncludeWithdrawn ? (bool?)null : true;
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(missingAssignmentsInput.GradingPeriodId);
            var stiModel = new MissingAssignmentsParams
            {
                AcadSessionId = gradingPeriod.SchoolYearRef,
                AlternateScoreIds = missingAssignmentsInput.AlternateScoreIds?.ToArray(),
                AlternateScoresOnly = missingAssignmentsInput.AlternateScoresOnly,
                EndDate = missingAssignmentsInput.EndDate,
                ConsiderZerosAsMissingGrades = missingAssignmentsInput.ConsiderZerosAsMissingGrades,
                IdToPrint = missingAssignmentsInput.IdToPrint,
                IncludeWithdrawn = missingAssignmentsInput.IncludeWithdrawn,
                OnePerPage = missingAssignmentsInput.OnePerPage,
                OrderBy = missingAssignmentsInput.OrderBy,
                SectionId = missingAssignmentsInput.ClassId,
                StartDate = missingAssignmentsInput.StartDate,
                SuppressStudentName = missingAssignmentsInput.SuppressStudentName,
            };
            if (missingAssignmentsInput.StudentIds == null)
            {
                var students = ServiceLocator.StudentService.GetClassStudents(missingAssignmentsInput.ClassId, gradingPeriod.MarkingPeriodRef, isEnrolled);
                stiModel.StudentIds = students.Select(x => x.Id).ToArray();
            }
            else stiModel.StudentIds = missingAssignmentsInput.StudentIds.ToArray();
            return stiModel;
        }

        protected override Func<MissingAssignmentsParams, byte[]> InowGenerateReportFunc=> ConnectorLocator.ReportConnector.MissingAssignmentsReport;
    }
}
