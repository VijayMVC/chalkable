using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.Reporting;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.JsonContractTools;
using Chalkable.Common.Web;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Common;
using Chalkable.Web.Models;
using Chalkable.Web.Models.PersonViewDatas;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ReportingController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult GradeBookReport(GradebookReportInputModel gradebookReportInput)
        {
            return Report(gradebookReportInput, SchoolLocator.ReportService.GetGradebookReport, "GradeBookReport");
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult WorksheetReport(WorksheetReportInputModel worksheetReportInput)
        {
            return Report(worksheetReportInput, SchoolLocator.ReportService.GetWorksheetReport, "WorksheetReport");
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult ComprehensiveProgressReport(ComprehensiveProgressInputModel comprehensiveProgressInput)
        {
            return Report(comprehensiveProgressInput, SchoolLocator.ReportService.GetComprehensiveProgressReport, "ComprehensiveProgressReport");
        }

        [AuthorizationFilter("Teacher, Student")]
        public ActionResult StudentComprehensiveProgressReport(int gradingPeriodId, int studentId)
        {
            var report = SchoolLocator.ReportService.GetStudentComprehensiveReport(studentId, gradingPeriodId);
            return DownloadReportFile(report, "StudentComprehensiveProgressReport", ReportingFormat.Pdf);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult MissingAssignmentsReport(MissingAssignmentsInputModel missingAssignmentsInput)
        {
            return Report(missingAssignmentsInput, SchoolLocator.ReportService.GetMissingAssignmentsReport, "MissingAssignmentsReport");
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult ProgressReport(ProgressReportInputModel progressReportInput)
        {
            return Report(progressReportInput, SchoolLocator.ReportService.GetProgressReport, "ProgressReport");
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult BirthdayReport(BirthdayReportInputModel birthdayReportInputModel)
        {
            return Report(birthdayReportInputModel, SchoolLocator.ReportService.GetBirthdayReport, "BirthdayReport");
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult AttendanceRegisterReport(AttendanceRegisterInputModel attendanceRegisterInputModel)
        {
            return Report(attendanceRegisterInputModel, SchoolLocator.ReportService.GetAttendanceRegisterReport, "AttendanceRegisterReport");
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult AttendanceProfileReport(AttendanceProfileReportInputModel attendanceProfileReportInputModel)
        {
            return Report(attendanceProfileReportInputModel, SchoolLocator.ReportService.GetAttendanceProfileReport, "AttendanceProfileReport");
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult SeatingChartReport(SeatingChartReportInputModel seatingChartReportInputModel)
        {
            return Report(seatingChartReportInputModel, SchoolLocator.ReportService.GetSeatingChartReport, "SeatingChartReport");
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult GradeVerificationReport(GradeVerificationInputModel gradeVerificationInputModel)
        {
            return Report(gradeVerificationInputModel, SchoolLocator.ReportService.GetGradeVerificationReport, "GradeVerificationReport");
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult LessonPlanReport(LessonPlanReportInputModel lessonPlanReportInputModel)
        {
            return Report(lessonPlanReportInputModel, SchoolLocator.ReportService.GetLessonPlanReport, "LessonPlanReport");
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult ReportCards(ReportCardsInputModel inputModel)
        {
            var path = Server.MapPath(ApplicationPath).Replace("/", "\\");
            inputModel.DefaultDataPath = path;
            //IList<string> htmls = new List<string>();
            //htmls.Add(LoadDemoView(Path.Combine(path, "DemoReportView.html")));
            //var header = LoadDemoView(Path.Combine(path, "DemoReportHeader.html"));
            //var footer = LoadDemoView(Path.Combine(path, "DemoReportFooter.html"));


            ////for (int i = 0; i < 3; i++)
            ////{
            ////    var view = BuildReportView(inputModel);
            ////    var html = RenderViewToString(view.ViewName, view.Model);
            ////    htmls.Add(html);
            ////}
            return Report(()=> SchoolLocator.ReportService.GetReportCards(inputModel, path), "Report Cards", ReportingFormat.Pdf, DownloadReportFile);
        }

        private string LoadDemoView(string path)
        {
            using (var file = System.IO.File.OpenRead(path))
            {
                var reader = new StreamReader(file);
                var res = reader.ReadToEnd();
                reader.Close();
                return res;
            }
        }

        //private ViewResult BuildReportView(ReportCardsInputModel inputModel)
        //{
        //    var template = MasterLocator.CustomReportTemplateService.GetById(inputModel.CustomReportTemplateId);
        //    ViewBag.JadeTpl = template.Layout;
        //    ViewBag.Style = template.Style;
        //    var data = SchoolLocator.ReportService.BuildCustomReportCardsExportModel(inputModel);
        //    ViewData[ViewConstants.REPORT_CARDS] = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
        //    {
        //        ContractResolver = new LowercaseContractResolver()
        //    });
        //    return View("ReportCards");
        //}

        //private string RenderViewToString(string viewName, object model)
        //{
        //    ViewData.Model = model;
        //    using (var sw = new StringWriter())
        //    {
        //        var viewResult = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
        //        var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}


        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult FeedReport(FeedReportSettingsInfo settings, int? classId, int? format, bool? complete, int? announcementType)
        {
            //TODO: save report settings 
            SchoolLocator.ReportService.SetFeedReportSettings(settings);

            var path = Server.MapPath(ApplicationPath).Replace("/", "\\");
            var formatType = (ReportingFormat?) format ?? ReportingFormat.Pdf;
            var reportInput = new FeedReportInputModel
            {
                ClassId = classId,
                Format = format,
                Settings = settings,
                Complete = complete,
                AnnouncementType = announcementType
            };
            return Report(() => SchoolLocator.ReportService.GetFeedReport(reportInput, path), "Feed Report", formatType, DownloadReportFile);
        }

        private ActionResult Report<TReport>(TReport reportInputModel
            , Func<TReport, byte[]> reportAction, string reportFileName) where TReport : BaseReportInputModel
        {
            return Report(() => reportAction(reportInputModel), reportFileName, reportInputModel.FormatTyped, DownloadReportFile);
        }

        private ActionResult Report(Func<byte[]> reportAction, string reportFileName, ReportingFormat formatType
            , Func<byte[], string, ReportingFormat, ActionResult> loadAction)
        {
            try
            {
                var res = reportAction();
                return loadAction(res, reportFileName, formatType);
            }
            catch (LocalProcessingException exception)
            {
                return HandlReportProcessingException(exception);
            }
            catch (Exception exception)
            {
                return HandleAttachmentException(exception);
            }
         }
        
        private ActionResult DownloadReportFile(byte[] report, string reportFileName, ReportingFormat formatType)
        {
            Response.AppendCookie(new HttpCookie("chlk-iframe-ready", Guid.NewGuid().ToString()));
            var extension = formatType.AsFileExtension();
            var fileName = $"{reportFileName}.{extension}";
            MasterLocator.UserTrackingService.CreatedReport(Context.Login, reportFileName);
            return File(report, MimeHelper.GetContentTypeByExtension(extension), fileName);
        }
        
        private ActionResult HandlReportProcessingException(LocalProcessingException exception)
        {
            //TODO: remove this later 
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = 500;
            Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);

            var errorMessages = new List<string>() { exception.Message };
            var ex = exception.InnerException;
            while (ex != null)
            {
                errorMessages.Add(ex.Message);
                ex = ex.InnerException;
            }

            return new ChalkableJsonResult(false)
            {
                Data = new ChalkableJsonResponce(new
                {
                    Exception = ExceptionViewData.Create(exception),
                    InnerException = ExceptionViewData.Create(exception.InnerException),
                    Messages = errorMessages
                })
                {
                    Success = false
                },
                ContentType = HTML_CONTENT_TYPE,
                SerializationDepth = 4
            };
        }


        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult SetStudentProgressReportComments(int classId, int gradingPeriodId, string studentComments)
        {
            var stComments = JsonConvert.DeserializeObject<IList<StudentCommentInputModel>>(studentComments);
            SchoolLocator.ReportService.SetProgressReportComments(classId, gradingPeriodId, stComments);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult GetStudentProgressReportComments(int classId, int gradingPeriodId)
        {
            var studentComments = SchoolLocator.ReportService.GetProgressReportComments(classId, gradingPeriodId);
            return Json(StudentCommentViewData.Create(studentComments));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult FeedReportSettings()
        {
            var feedSettings = SchoolLocator.AnnouncementFetchService.GetSettingsForFeed();
            var feedReportSettings = SchoolLocator.ReportService.GetFeedReportSettings();
            return Json(FeedReportSettingsViewData.Create(feedReportSettings, feedSettings));
        }


        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult ListReportCardsLogo()
        {
            return Json(GetListOfReportCardsLogo());
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult UpdateReportCardsLogo(int? schoolId)
        {
            byte[] icon;
            string filename;
            GetFileFromRequest(out icon, out filename);
            SchoolLocator.ReportService.UpdateReportCardsLogo(schoolId, icon);
            return Json(GetListOfReportCardsLogo());
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DeleteReportCardsLogo(int id)
        {
            SchoolLocator.ReportService.DeleteReportCardsLogo(id);
            return Json(GetListOfReportCardsLogo());
        }

        private IList<ReportCardsLogoViewData> GetListOfReportCardsLogo()
        {
            var schools = SchoolLocator.SchoolService.GetSchools();
            return ReportCardsLogoViewData.Create(SchoolLocator.ReportService.GetReportCardsLogos(), schools);
        }

        
    }
}