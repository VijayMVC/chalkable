﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Web;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.PersonViewDatas;

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
        public ActionResult ProgressReport(ProgressReportInputModel progressReportInput, IntList studentIds, StringList comments)
        {
            if (studentIds != null && comments != null && studentIds.Count == comments.Count)
            {
                progressReportInput.StudentComments = new List<StudentCommentInputModel>();
                for (int i = 0; i < studentIds.Count; i++)
                {
                    progressReportInput.StudentComments.Add(new StudentCommentInputModel
                        {
                            Comment = comments[i],
                            StudentId = studentIds[i]
                        });
                }
            }
            return Report(progressReportInput, SchoolLocator.ReportService.GetProgressReport, "ProgressReport");
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
        public ActionResult GetStudentProgressReportComments(int classId, int gradingPeriodId)
        {
            var studentComments = SchoolLocator.ReportService.GetProgressReportComments(classId, gradingPeriodId);
            return Json(StudentCommentViewData.Create(studentComments));
        }
    }
}