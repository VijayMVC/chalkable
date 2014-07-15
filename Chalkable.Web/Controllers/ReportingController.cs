using System;
using System.IO;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Web;
using Chalkable.Web.ActionFilters;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ReportingController : ChalkableController
    {
        private const string headerFormat = "inline; filename={0}";
        private const string CONTENT_DISPOSITION = "Content-Disposition";

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult GradeBookReport(GradebookReportInputModel gradebookReportInput)
        {
            return Report(gradebookReportInput, SchoolLocator.ReportService.GetGradebookReport, "GradeBookReport");
        }
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult WorksheetReport(WorksheetReportInputModel worksheetReportInput)
        {
            return Report(worksheetReportInput, SchoolLocator.ReportService.GetWorksheetReport, "WorksheetReport");
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult ProgressReport(ProgressReportInputModel progressReportInput)
        {
            return Report(progressReportInput, SchoolLocator.ReportService.GetProgressReport, "ProgressReport");
        }

        private ActionResult Report<TReport>(TReport reportInputModel
            , Func<TReport, byte[]> reportAction, string reportFileName) where TReport : BaseReportInputModel
        {
            var res = reportAction(reportInputModel);
            var extension = reportInputModel.FormatTyped.AsFileExtension();
            var fileName = string.Format("{0}.{1}", reportFileName, extension);
            return File(res, MimeHelper.GetContentTypeByExtension(extension), fileName);
        }
    }
}