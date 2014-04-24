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
            var res = SchoolLocator.ReportService.GetGradebookReport(gradebookReportInput);
            var extension = gradebookReportInput.FormatTyped.AsFileExtension();
            var fileName = "GradeBookReport." + extension;
            return File(res, MimeHelper.GetContentTypeByExtension(extension), fileName);
        }
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult WorksheetReport(WorksheetReportInputModel worksheetReportInput)
        {
            var res = SchoolLocator.ReportService.GetWorksheetReport(worksheetReportInput);
            var extension = worksheetReportInput.FormatTyped.AsFileExtension();
            var fileName = "WorksheetReport." + extension;
            return File(res, MimeHelper.GetContentTypeByExtension(extension), fileName);
        }
    }
}