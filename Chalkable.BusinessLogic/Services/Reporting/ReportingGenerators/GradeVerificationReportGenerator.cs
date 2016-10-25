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
    public class GradeVerificationReportGenerator : InowReportGenerator<GradeVerificationInputModel, GradeVerificationReportParams>
    {
        public GradeVerificationReportGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) : base(serviceLocatorSchool, connectorLocator)
        {
        }

        protected override GradeVerificationReportParams CreateInowReportSettings(GradeVerificationInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            return new GradeVerificationReportParams
            {
                AcadSessionId = gp.SchoolYearRef,
                GradeType = inputModel.GradeType,
                SectionId = inputModel.ClassId,
                GradedItemIds = inputModel.GradedItemId?.ToArray(),
                GradingPeriodIds = inputModel.GradingPeriodIds?.ToArray() ?? new[] { gp.Id },
                IncludeComments = inputModel.IncludeCommentsAndLegend,
                IncludeSignature = inputModel.IncludeSignature,
                IncludeNotes = inputModel.IncludeNotes,
                IncludeWithdrawn = inputModel.IncludeWithdrawn,
                IdToPrint = inputModel.IdToPrint,
                StudentOrder = inputModel.StudentOrder,
                StudentIds = inputModel.StudentIds?.ToArray()
            };
        }
        protected override Func<GradeVerificationReportParams, byte[]> InowGenerateReportFunc => ConnectorLocator.ReportConnector.GradeVerificationReport;
    }
}
