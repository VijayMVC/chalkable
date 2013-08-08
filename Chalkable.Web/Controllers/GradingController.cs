using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class GradingController : ChalkableController
    {
        [AuthorizationFilter("Teacher")]
        public ActionResult TeacherSummary(Guid teacherId)
        {
            var schoolYearId = GetCurrentSchoolYearId();
            var gradingStats = SchoolLocator.GradingStatisticService.GetStudentGradePerClass(teacherId, schoolYearId);
            gradingStats = gradingStats.Where(x => x.Avg.HasValue).ToList();
            var classes = SchoolLocator.ClassService.GetClasses(null, null, teacherId);
            return Json(GradingTeacherClassSummaryViewData.Create(gradingStats, classes), 6);
        }

        [AuthorizationFilter("Teacher", Preference.API_DESCR_GRADING_CLASS_SUMMARY, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Class })]
        public ActionResult ClassSummary(Guid classId)
        {
            if (!SchoolLocator.Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            var teacherId = SchoolLocator.Context.UserId;
            return Json(ClassLogic.GetGradingSummary(SchoolLocator, classId, GetCurrentSchoolYearId(), teacherId), 7);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult StudentSummary(Guid studentId, Guid? classId)
        {
            var announcements = SchoolLocator.AnnouncementService.GetAnnouncements(30, true);
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(Context.NowSchoolTime.Date, true);
            var gradingStats = SchoolLocator.GradingStatisticService.GetStudentGradeStats(studentId, mp.Id, classId);
            return Json(GradingStudentSummaryViewData.Create(announcements, gradingStats));
        }

        //TODO: duplicate part of announcement/read data. for API compatibility only
        [AuthorizationFilter("Teacher", Preference.API_DESCR_GRADE_LIST_ITEMS, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Announcement })]
        public ActionResult ItemGradesList(Guid announcementId)
        {
            var annView = SchoolLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var attachmentsInfo = AttachmentLogic.PrepareAttachmentsInfo(annView.AnnouncementAttachments);
            var res = StudentAnnouncementLogic.ItemGradesList(SchoolLocator, annView, attachmentsInfo);
            return Json(res);
        }

        public ActionResult ApplyAutoGrade(Guid announcementId)
        {
            SchoolLocator.StudentAnnouncementService.ResolveAutoGrading(announcementId, true);
            return ItemGradesList(announcementId);
        }

        public ActionResult ApplyManualGrade(Guid announcementId)
        {
            SchoolLocator.StudentAnnouncementService.ResolveAutoGrading(announcementId, false);
            return ItemGradesList(announcementId);
        }

        //TODO: do we need this in API still?
        [RequireRequestValue("studentAnnouncementId")]
        [AuthorizationFilter("Teacher", Preference.API_DESCR_GRADE_UPDATE_ITEM, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Announcement })]
        public ActionResult UpdateItem(Guid studentAnnouncementId, int? gradeValue, string extraCredits, string comment, bool dropped)
        {
            var studentAnn = SchoolLocator.StudentAnnouncementService.SetGrade(studentAnnouncementId, gradeValue, extraCredits, comment, dropped, (int)GradingStyleEnum.Numeric100);
            return PrepareStudentAnnouncementResponse(studentAnn);
        }

        [AuthorizationFilter("Teacher ,AdminGrade, Student", Preference.API_DESCR_SET_AUTO_GRADE, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Announcement })]
        public ActionResult SetAutoGrade(Guid studentAnnouncementId, int value, Guid applicationId)
        {
            var res = SchoolLocator.StudentAnnouncementService.SetAutoGrade(studentAnnouncementId, value, applicationId);
            return PrepareStudentAnnouncementResponse(res);
        }

        private ActionResult PrepareStudentAnnouncementResponse(StudentAnnouncement studentAnn)
        {
            var studentAnnsInfo = SchoolLocator.StudentAnnouncementService.GetStudentAnnouncements(studentAnn.AnnouncementRef);
            var res = studentAnnsInfo.First(x => x.Id == studentAnn.Id);
            var attachments = SchoolLocator.AnnouncementAttachmentService.GetAttachments(res.AnnouncementRef, 0, 1000).ToList();
            var attc = AttachmentLogic.PrepareAttachmentsInfo(attachments.Where(x => x.PersonRef == res.Id).ToList());
            return Json(StudentAnnouncementViewData.Create(res, attc));
        }
    }
}