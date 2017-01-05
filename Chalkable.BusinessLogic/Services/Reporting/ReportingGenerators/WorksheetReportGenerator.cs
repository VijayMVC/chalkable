using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class WorksheetReportGenerator : InowReportGenerator<WorksheetReportInputModel, WorksheetReportParams>
    {
        public WorksheetReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override WorksheetReportParams CreateInowReportSettings(WorksheetReportInputModel inputModel)
        {
            int[] activityIds = new int[10];
            if (inputModel.AnnouncementIds != null && inputModel.AnnouncementIds.Count > 0)
            {
                var anns = ServiceLocator.ClassAnnouncementService.GetClassAnnouncements(inputModel.StartDate, inputModel.EndDate, null, null, null);
                anns = anns.Where(x => x.SisActivityId.HasValue && inputModel.AnnouncementIds.Contains(x.Id)).ToList();
                activityIds = anns.Select(x => x.SisActivityId.Value).ToArray();
            }

            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var stiModel = new WorksheetReportParams
            {
                ActivityIds = activityIds,
                StartDate = inputModel.StartDate,
                EndDate = inputModel.EndDate,
                Header = inputModel.Header,
                IdToPrint = inputModel.IdToPrint,
                PrintAverage = inputModel.PrintAverage,
                PrintLetterGrade = inputModel.PrintLetterGrade,
                PrintScores = inputModel.PrintScores,
                PrintStudent = inputModel.PrintStudent,
                Title1 = inputModel.Title1,
                Title2 = inputModel.Title2,
                Title3 = inputModel.Title3,
                Title4 = inputModel.Title4,
                Title5 = inputModel.Title5,
                SectionId = inputModel.ClassId,
                GradingPeriodId = inputModel.GradingPeriodId,
            };
            if (inputModel.StudentIds == null)
            {
                var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef);
                stiModel.StudentIds = students.Select(x => x.Id).ToArray();
            }
            else stiModel.StudentIds = inputModel.StudentIds.ToArray();

            if (CoreRoles.TEACHER_ROLE == ServiceLocator.Context.Role)
                stiModel.StaffId = ServiceLocator.Context.PersonId;
            stiModel.AcadSessionId = gp.SchoolYearRef;
            return stiModel;
        }

        protected override Func<WorksheetReportParams, byte[]> InowGenerateReportFunc
            => ConnectorLocator.ReportConnector.WorksheetReport;
    }
}
