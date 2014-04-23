using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Web;
using Chalkable.Web.ActionFilters;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ReportingController : ChalkableController
    {

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult GradeBookReport(GradebookReportInputModel gradebookReportInput)
        {
            var res = SchoolLocator.ReportService.GetGradebookReport(gradebookReportInput);
            return File(res, MimeHelper.GetContentTypeByExtension(gradebookReportInput.FormatTyped.AsFileExtension()));
        }
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult WorksheetReport(WorksheetReportInputModel worksheetReportInput)
        {
            var res = SchoolLocator.ReportService.GetWorksheetReport(worksheetReportInput);
            return File(res, MimeHelper.GetContentTypeByExtension(worksheetReportInput.FormatTyped.AsFileExtension()));
        }
    }
}