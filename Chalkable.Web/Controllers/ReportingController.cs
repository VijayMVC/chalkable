using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.PersonViewDatas;
using Newtonsoft.Json;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ReportingController : ChalkableController
    {
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
        public ActionResult ComprehensiveProgressReport(ComprehensiveProgressInputModel comprehensiveProgressInput)
        {
            return Report(comprehensiveProgressInput, SchoolLocator.ReportService.GetComprehensiveProgressReport, "ComprehensiveProgressReport");
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult MissingAssignmentsReport(MissingAssignmentsInputModel missingAssignmentsInput)
        {
            return Report(missingAssignmentsInput, SchoolLocator.ReportService.GetMissingAssignmentsReport, "MissingAssignmentsReport");
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult ProgressReport(ProgressReportInputModel progressReportInput)
        {
            //if(progressReportInput.StudentIds != null &&(studentComments == null || progressReportInput.StudentIds.Count != studentComments.Count))
            //    throw new ChalkableException("Invalid student comments param");
            //if (progressReportInput.StudentIds != null)
            //{
            //    progressReportInput.StudentComments = new List<StudentCommentInputModel>();
            //    for (int i = 0; i < progressReportInput.StudentIds.Count; i++)
            //    {
            //        progressReportInput.StudentComments.Add(new StudentCommentInputModel
            //        {
            //            StudentId = progressReportInput.StudentIds[i],
            //            Comment = studentComments[i]
            //        });
            //    }   
            //}
            return Report(progressReportInput, SchoolLocator.ReportService.GetProgressReport, "ProgressReport");
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult BirthdayReport(BirthdayReportInputModel birthdayReportInputModel)
        {
            return Report(birthdayReportInputModel, SchoolLocator.ReportService.GetBirthdayReport, "BirthdayReport");
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult AttendanceRegisterReport(AttendanceRegisterInputModel attendanceRegisterInputModel)
        {
            return Report(attendanceRegisterInputModel, SchoolLocator.ReportService.GetAttendanceRegisterReport, "AttendanceRegisterReport");
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult AttendanceProfileReport(AttendanceProfileReportInputModel attendanceProfileReportInputModel)
        {
            return Report(attendanceProfileReportInputModel, SchoolLocator.ReportService.GetAttendanceProfileReport, "AttendanceProfileReport");
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult SeatingChartReport(SeatingChartReportInputModel seatingChartReportInputModel)
        {
            return Report(seatingChartReportInputModel, SchoolLocator.ReportService.GetSeatingChartReport, "SeatingChartReport");
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult GradeVerificationReport(GradeVerificationInputModel gradeVerificationInputModel)
        {
            return Report(gradeVerificationInputModel, SchoolLocator.ReportService.GetGradeVerificationReport, "GradeVerificationReport");
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult LessonPlanReport(LessonPlanReportInputModel lessonPlanReportInputModel)
        {
            return Report(lessonPlanReportInputModel, SchoolLocator.ReportService.GetLessonPlanReport, "LessonPlanReport");
        }

        private ActionResult Report<TReport>(TReport reportInputModel
            , Func<TReport, byte[]> reportAction, string reportFileName) where TReport : BaseReportInputModel
        {
            var res = reportAction(reportInputModel);
            var extension = reportInputModel.FormatTyped.AsFileExtension();
            var fileName = string.Format("{0}.{1}", reportFileName, extension);
            MasterLocator.UserTrackingService.CreatedReport(Context.Login, reportFileName);
            return File(res, MimeHelper.GetContentTypeByExtension(extension), fileName);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult SetStudentProgressReportComments(int classId, int gradingPeriodId, string studentComments)
        {
            var stComments = JsonConvert.DeserializeObject<IList<StudentCommentInputModel>>(studentComments);
            SchoolLocator.ReportService.SetProgressReportComments(classId, gradingPeriodId, stComments);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult GetStudentProgressReportComments(int classId, int gradingPeriodId)
        {
            var studentComments = SchoolLocator.ReportService.GetProgressReportComments(classId, gradingPeriodId);
            return Json(StudentCommentViewData.Create(studentComments));
        }
    }
}