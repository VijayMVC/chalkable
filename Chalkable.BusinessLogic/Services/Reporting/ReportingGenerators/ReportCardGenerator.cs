using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports.ReportCards;

namespace Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators
{
    public class ReportCardGenerator : BaseReportGenerator<ReportCardsInputModel>
    {
        public ReportCardGenerator(IServiceLocatorSchool serviceLocatorSchool, ConnectorLocator connectorLocator) : base(serviceLocatorSchool, connectorLocator)
        {
        }

        public override byte[] GenerateReport(ReportCardsInputModel settings)
        {
            Trace.Assert(ServiceLocator.Context.SchoolLocalId.HasValue);
            var inowReportCardTask = Task.Run(() => GetInowReportData(settings));

            var logo = ServiceLocator.ReportService.GetLogoBySchoolId(ServiceLocator.Context.SchoolLocalId.Value)
                      ?? ServiceLocator.ReportService.GetDistrictLogo();
            var template = ServiceLocator.ServiceLocatorMaster.CustomReportTemplateService.GetById(settings.CustomReportTemplateId);
            var templateRenderer = new TemplateRenderer(settings.DefaultDataPath);
            var listOfReportCards = BuildReportCardsData(inowReportCardTask.Result, logo?.LogoAddress, settings);
            IList<byte[]> listOfpdf = new List<byte[]>();
            string headerTpl = null, footerTpl = null;
            foreach (var data in listOfReportCards)
            {
                var bodyTpl = templateRenderer.Render(template.Layout, data);
                if (template.Header != null)
                    headerTpl = templateRenderer.Render(template.Header.Layout, data);
                if (template.Footer != null)
                    footerTpl = templateRenderer.Render(template.Footer.Layout, data);
                var report = DocumentRenderer.RenderToPdf(settings.DefaultDataPath, settings.ContentUrl, bodyTpl, template.Style, headerTpl,
                    template.Header?.Style, footerTpl, template.Footer?.Style);
                listOfpdf.Add(report);
            }
            return DocumentRenderer.MergePdfDocuments(listOfpdf);
        }

        private IList<CustomReportCardsExportModel> BuildReportCardsData(ReportCard inowData, string logoAddress, ReportCardsInputModel inputModel)
        {
            var res = new List<CustomReportCardsExportModel>();
            var currentDate = ServiceLocator.Context.NowSchoolTime;
            foreach (var student in inowData.Students)
            {
                res.AddRange(student.Recipients.Select(recipient => CustomReportCardsExportModel.Create(inowData, student, recipient, logoAddress, currentDate, inputModel)));
            }
            return res;
        }

        private async Task<ReportCard> GetInowReportData(ReportCardsInputModel inputModel)
        {
            Trace.Assert(ServiceLocator.Context.SchoolYearId.HasValue);
            Trace.Assert(ServiceLocator.Context.SchoolLocalId.HasValue);
            BaseSecurity.EnsureDistrictAdmin(ServiceLocator.Context);
            if (inputModel == null)
                throw new ArgumentNullException(nameof(ReportCardsInputModel));
            var studentIds = inputModel.StudentIds ?? new List<int>();
            if (inputModel.GroupIds != null && inputModel.GroupIds.Count > 0)
            {
                studentIds.AddRange(ServiceLocator.GroupService.GetStudentIdsByGroups(inputModel.GroupIds));
                studentIds = studentIds.Distinct().ToList();
            }
            var options = new ReportCardOptions
            {
                AbsenceReasonIds = inputModel.AttendanceReasonIds,
                GradingPeriodId = inputModel.GradingPeriodId,
                AcadSessionId = ServiceLocator.Context.SchoolYearId.Value,
                IncludeAttendance = inputModel.IncludeAttendance,
                IncludeGradingPeriodNotes = inputModel.IncludeGradingPeriodNotes,
                IncludeComments = inputModel.IncludeComments,
                IncludeMeritDemerit = inputModel.IncludeMeritDemerit,
                IncludeWithdrawnStudents = inputModel.IncludeWithdrawnStudents,
                IncludePromotionStatus = inputModel.IncludePromotionStatus,
                IncludeYearToDateInformation = inputModel.IncludeYearToDateInformation,
                IncludeGradingScales = inputModel.IncludeGradingScaleStandards || inputModel.IncludeGradingScaleTraditional,
                StudentIds = studentIds,
                Recipient = inputModel.RecipientStr,
                IncludeStandards = inputModel.StandardTypeEnum != StandardsType.None
            };
            return await ConnectorLocator.ReportConnector.GetReportCardData(options);
        }


    }
}
