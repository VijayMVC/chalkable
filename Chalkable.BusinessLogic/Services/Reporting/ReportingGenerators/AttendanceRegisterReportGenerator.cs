using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class AttendanceRegisterReportGenerator : InowReportGenerator<AttendanceRegisterInputModel, AttendanceRegisterReportParams>
    {
        public AttendanceRegisterReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) 
            : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override AttendanceRegisterReportParams CreateInowReportSettings(AttendanceRegisterInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            return new AttendanceRegisterReportParams
            {
                AcadSessionId = gp.SchoolYearRef,
                AbsenceReasonIds = inputModel.AbsenceReasonIds?.ToArray(),
                IdToPrint = inputModel.IdToPrint,
                IncludeTardies = inputModel.IncludeTardies,
                MonthId = inputModel.MonthId,
                ReportType = inputModel.ReportType,
                SectionId = inputModel.ClassId,
                ShowLocalReasonCode = inputModel.ShowLocalReasonCode
            };
        }

        protected override Func<AttendanceRegisterReportParams, byte[]> InowGenerateReportFunc
            => ConnectorLocator.ReportConnector.AttendnaceRegisterReport;
    }
}
