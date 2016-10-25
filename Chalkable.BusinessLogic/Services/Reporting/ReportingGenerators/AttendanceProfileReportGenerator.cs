using System;
using System.Linq;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class AttendanceProfileReportGenerator : InowReportGenerator<AttendanceProfileReportInputModel, AttendanceProfileReportParams>
    {
        public AttendanceProfileReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) 
            : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override AttendanceProfileReportParams CreateInowReportSettings(AttendanceProfileReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var ps = new AttendanceProfileReportParams
            {
                AbsenceReasonIds = inputModel.AbsenceReasons?.ToArray(),
                AcadSessionId = gp.SchoolYearRef,
                IncludeNote = inputModel.DisplayNote,
                IncludePeriodAbsences = inputModel.DisplayPeriodAbsences,
                IncludeReasonTotals = inputModel.DisplayReasonTotals,
                IncludeWithdrawnStudents = inputModel.DisplayWithdrawnStudents,
                StartDate = inputModel.StartDate,
                EndDate = inputModel.EndDate,
                GroupBy = inputModel.GroupBy,
                SectionId = inputModel.ClassId,
                IdToPrint = inputModel.IdToPrint,
                IncludeUnlisted = inputModel.IncludeUnlisted,
                IncludeCheckInCheckOut = inputModel.IncludeCheckInCheckOut,
                TermIds = inputModel.MarkingPeriodIds?.ToArray(),
            };
            if (inputModel.StudentIds == null)
            {
                var isEnrolled = inputModel.DisplayWithdrawnStudents ? (bool?)null : true;
                var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef, isEnrolled);
                ps.StudentIds = students.Select(x => x.Id).ToArray();
            }
            else
                ps.StudentIds = inputModel.StudentIds.ToArray();
            return ps;
        }

        protected override Func<AttendanceProfileReportParams, byte[]> InowGenerateReportFunc
            => ConnectorLocator.ReportConnector.AttendanceProfileReport;
    }
}
