using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
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
        public ActionResult TeacherSummary(int teacherId)
        {
            return FakeJson("~/fakeData/gradingSummary.json");
            //var schoolYearId = GetCurrentSchoolYearId();
            //var gradingStats = SchoolLocator.GradingStatisticService.GetStudentsGradePerClass(teacherId, schoolYearId);
            //gradingStats = gradingStats.Where(x => x.Avg.HasValue).ToList();
            //var classes = SchoolLocator.ClassService.GetClasses(null, null, teacherId);
            //return Json(GradingTeacherClassSummaryViewData.Create(gradingStats, classes), 6);
        }

        [AuthorizationFilter("Teacher", Preference.API_DESCR_GRADING_CLASS_SUMMARY_GRID, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Class })]
        public ActionResult ClassSummaryGrid(int classId)
        {
            return FakeJson("~/fakeData/teacherGradingGrid.json");
        }

        [AuthorizationFilter("Teacher", Preference.API_DESCR_GRADING_CLASS_SUMMARY, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Class })]
        public ActionResult ClassSummary(int classId)
        {
            return FakeJson("~/fakeData/gradingClassSummary.json");
            //if (!SchoolLocator.Context.SchoolId.HasValue)
            //    throw new UnassignedUserException();
            //var teacherId = Context.UserLocalId;
            //return Json(ClassLogic.GetGradingSummary(SchoolLocator, classId, GetCurrentSchoolYearId(), teacherId), 7);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_GRADING_CLASS_SUMMARY, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Class })]
        public ActionResult ItemGradingStat(int announcementId)
        {
            return FakeJson("~/fakeData/itemGradingStat.json");
            //var ann = SchoolLocator.AnnouncementService.GetAnnouncementById(announcementId);
            //var studentAnns = SchoolLocator.StudentAnnouncementService.GetStudentAnnouncements(announcementId);
            //var mapper = SchoolLocator.GradingStyleService.GetMapper();
            //return Json(ItemGradigStatViewData.Create(studentAnns, ann, mapper));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult StudentSummary(int studentId, int? classId)
        {
            throw new NotImplementedException();
            //var announcements = SchoolLocator.AnnouncementService.GetAnnouncements(30, true);
            //var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(Context.NowSchoolTime.Date, true);
            //var gradingStats = SchoolLocator.GradingStatisticService.GetStudentGradePerDate(studentId, mp.Id, classId);
            //return Json(GradingStudentSummaryViewData.Create(announcements, gradingStats));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult StudentClassSummary(int studentId, int classId)
        {
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(Context.NowSchoolTime.Date, true);
            var gradingStats = SchoolLocator.GradingStatisticService.GetStudentClassGradeStats(mp.Id, classId, studentId);
            var gradingPerMp = ClassLogic.GetGradingSummary(SchoolLocator, classId, mp.SchoolYearRef, null, studentId, false);
            return Json(GradingStudentClassSummaryViewData.Create(gradingStats.FirstOrDefault(), mp, gradingPerMp));
        }

        //TODO: duplicate part of announcement/read data. for API compatibility only
        [AuthorizationFilter("Teacher", Preference.API_DESCR_GRADE_LIST_ITEMS, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Announcement })]
        public ActionResult ItemGradesList(int announcementId)
        {
            var annView = SchoolLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var attachmentsInfo = AttachmentLogic.PrepareAttachmentsInfo(annView.AnnouncementAttachments);
            var res = StudentAnnouncementLogic.ItemGradesList(SchoolLocator, annView, attachmentsInfo);
            return Json(res);
        }

        public ActionResult ApplyAutoGrade(int announcementId)
        {
            SchoolLocator.StudentAnnouncementService.ResolveAutoGrading(announcementId, true);
            return ItemGradesList(announcementId);
        }

        public ActionResult ApplyManualGrade(int announcementId)
        {
            SchoolLocator.StudentAnnouncementService.ResolveAutoGrading(announcementId, false);
            return ItemGradesList(announcementId);
        }

        //TODO: do we need this in API still?
        [RequireRequestValue("studentAnnouncementId")]
        [AuthorizationFilter("Teacher", Preference.API_DESCR_GRADE_UPDATE_ITEM, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Announcement })]
        public ActionResult UpdateItem(int studentAnnouncementId, string gradeValue, string extraCredits
            , string comment, bool dropped, bool? exempt, bool? incomplete, bool? late, bool? absent)
        {
            var studentAnn = SchoolLocator.StudentAnnouncementService.SetGrade(studentAnnouncementId, gradeValue, extraCredits
                , comment, dropped, exempt ?? false, incomplete ?? false, late ?? false, absent ?? false
                , (int)GradingStyleEnum.Numeric100);
            return PrepareStudentAnnouncementResponse(studentAnn);
        }

        [AuthorizationFilter("Teacher ,AdminGrade, Student", Preference.API_DESCR_SET_AUTO_GRADE, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Announcement })]
        public ActionResult SetAutoGrade(int studentAnnouncementId, int value, Guid applicationId)
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