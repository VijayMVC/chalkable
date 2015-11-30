using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.Reporting;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.ActionResults;
using Chalkable.Web.Models;
using Chalkable.Web.Models.PersonViewDatas;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;

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

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult FeedReport(FeedReportSettingsInfo settings, int? classId, int? format)
        {
            //TODO: save report settings 
            SchoolLocator.ReportService.SetFeedReportSettings(settings);

            var path = Server.MapPath(ApplicationPath).Replace("/", "\\");
            var formatType = (ReportingFormat?) format ?? ReportingFormat.Pdf;
            var reportInput = new FeedReportInputModel {ClassId = classId, Format = format, Settings = settings};
            return Report(() => SchoolLocator.ReportService.GetFeedReport(reportInput, path), "Feed Report", formatType);
        }

        public ActionResult FakeReport()
        {
            try
            {
                var report = new LocalReport { ReportPath = string.Empty };
                var dataSource = new ReportDataSource("MainDataSet", new {});
                report.DataSources.Add(dataSource);

                string deviceInfo =
                  "<DeviceInfo>" +
                  "  <PageWidth>9in</PageWidth>" +
                  "  <PageHeight>12in</PageHeight>" +
                  "  <MarginTop>0.05in</MarginTop>" +
                  "  <MarginLeft>0.05in</MarginLeft>" +
                  "  <MarginRight>0.05in</MarginRight>" +
                  "  <MarginBottom>0.05in</MarginBottom>" +
                  "</DeviceInfo>";
                var res = report.Render("html", deviceInfo);
                return File(res, "application/octetstrem");
            }
            catch (Exception e)
            {
                return HandleAttachmentException(e);                
            }
        }

        private ActionResult Report<TReport>(TReport reportInputModel
            , Func<TReport, byte[]> reportAction, string reportFileName) where TReport : BaseReportInputModel
        {
            return Report(() => reportAction(reportInputModel), reportFileName, reportInputModel.FormatTyped);
        }

        private ActionResult Report(Func<byte[]> reportAction, string reportFileName, ReportingFormat formatType)
        {
            try
            {
                var res = reportAction();
                return DownloadReportFile(res, reportFileName, formatType);
            }
            catch (LocalProcessingException exception)
            {
                //TODO: remove this later 
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(Response.StatusCode);
                
                return new ChalkableJsonResult(false)
                {
                    Data = new ChalkableJsonResponce(ExceptionViewData.Create(exception, exception.InnerException))
                    {
                        Success = false
                    },
                    ContentType = HTML_CONTENT_TYPE,
                    SerializationDepth = 4
                };
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
    }
}