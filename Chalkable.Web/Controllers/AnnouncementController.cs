using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementController : AnnouncementBaseController
    {

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Create(int? classAnnouncementTypeId, int? classId)
        {
            if (!SchoolLocator.Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            if (classId.HasValue && !classAnnouncementTypeId.HasValue)
                throw new ChalkableException("Invalid method parameters");

            if (!classAnnouncementTypeId.HasValue)
            {
                if (SchoolLocator.Context.Role.Id == CoreRoles.TEACHER_ROLE.Id)
                {
                    var lastAnnouncement =
                        SchoolLocator.AnnouncementService.GetAnnouncements(0, 1, true).FirstOrDefault();
                    if (lastAnnouncement != null)
                        classAnnouncementTypeId = lastAnnouncement.ClassAnnouncementTypeRef;
                }
            }
            else
            {
                var draft = SchoolLocator.AnnouncementService.GetLastDraft();
                if (draft != null && draft.ClassAnnouncementTypeRef != classAnnouncementTypeId &&
                    !BaseSecurity.IsAdminViewer(SchoolLocator.Context))
                {
                    draft.ClassAnnouncementTypeRef = classAnnouncementTypeId.Value;
                    SchoolLocator.AnnouncementService.EditAnnouncement(AnnouncementInfo.Create(draft), classId);
                }
            }
            var annDetails = SchoolLocator.AnnouncementService.CreateAnnouncement(classAnnouncementTypeId, classId);
            var attachments = AttachmentLogic.PrepareAttachmentsInfo(annDetails.AnnouncementAttachments);
            var avd = AnnouncementDetailedViewData.Create(annDetails, null, Context.UserLocalId.Value, attachments);
            var res = new CreateAnnouncementViewData
                {
                    Announcement = avd,
                    IsDraft = annDetails.IsDraft
                };
            return Json(res, 7);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Delete(int announcementId)
        {
            SchoolLocator.AnnouncementService.DeleteAnnouncement(announcementId);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult DeleteDrafts(int personId)
        {
            SchoolLocator.AnnouncementService.DeleteAnnouncements(personId);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult EditTitle(int announcementId, string title)
        {
            if (!SchoolLocator.AnnouncementService.Exists(title) && !string.IsNullOrEmpty(title))
            {
                SchoolLocator.AnnouncementService.EditTitle(announcementId, title);
                return Json(true);
            }
            return Json(false);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Exists(string title)
        {
            return Json(SchoolLocator.AnnouncementService.Exists(title));
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Edit(int announcementId)
        {
            var viewData = PrepareFullAnnouncementViewData(announcementId, false);
            var res = new CreateAnnouncementViewData
                {
                    Announcement = viewData,
                };
            if (BaseSecurity.IsAdminViewer(SchoolLocator.Context))
            {
                var announcementRecipients = SchoolLocator.AnnouncementService.GetAnnouncementRecipients(announcementId);
                res.Recipients = announcementRecipients.Select(AnnouncementRecipientViewData.Create).ToList();
            }
            return Json(res, 6);
        }

        private AnnouncementComplex Save(AnnouncementInfo announcementInfo, int? classId,
                                         IList<RecipientInfo> recipientInfos = null)
        {
            if (!announcementInfo.ExpiresDate.HasValue)
                announcementInfo.ExpiresDate = SchoolLocator.Context.NowSchoolTime;
            return SchoolLocator.AnnouncementService.EditAnnouncement(announcementInfo, classId, recipientInfos);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student",
            Preference.API_DESCR_ANNOUNCEMENT_READ, true, CallType.Get, new[] {AppPermissionType.Announcement})]
        public ActionResult Read(int announcementId)
        {
            var res = PrepareFullAnnouncementViewData(announcementId, true, true);
            //if (res.SystemType != SystemAnnouncementType.Admin)
            //    MixPanelService.OpenedAnnouncement(SchoolLocator.Context., res.AnnouncementTypeName, res.Title, res.SchoolPersonName);
            return Json(res, 7);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Star(int announcementId, bool? star)
        {
            if (!star.HasValue)
            {
                var prev = SchoolLocator.AnnouncementService.GetAnnouncementDetails(announcementId).Starred ?? false;
                star = !prev;
            }
            SchoolLocator.AnnouncementService.Star(announcementId, star.Value);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult MakeVisible(int announcementId)
        {
            SchoolLocator.AnnouncementService.SetVisibleForStudent(announcementId, true);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult SaveAnnouncement(AnnouncementInfo announcementInfo, int? classId)
        {
            var ann = Save(announcementInfo, classId);
            return Json(AnnouncementViewData.Create(ann));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult SaveForAdmin(AnnouncementInfo announcement, ListOfStringList annRecipients)
        {
            var recipients = annRecipients != null ? RecipientInfo.Create(annRecipients) : null;
            Save(announcement, null, recipients);
            return Json(true);
        }

        private MarkingPeriod GetMarkingPeriod(DateTime expiresDate, int? markingPeriodId)
        {
            if (!SchoolLocator.Context.SchoolId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CONTEXT_NO_SCHOOL_INFO_ID);
            if (markingPeriodId.HasValue)
            {
                var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId.Value);
                return markingPeriod.StartDate <= expiresDate && markingPeriod.EndDate >= expiresDate
                           ? markingPeriod
                           : null;
            }
            return SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(expiresDate);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult SubmitAnnouncement(AnnouncementInfo announcement, int classId)
        {
            //var mp = GetMarkingPeriod(announcement.ExpiresDate ?? SchoolLocator.Context.NowSchoolTime, null);
            //if (mp == null)
            //    throw new NoMarkingPeriodException();

            //var mpClass = SchoolLocator.MarkingPeriodService.GetMarkingPeriodClass(classId, mp.Id);
            //if (mpClass == null)
            //    return Json(new ChalkableException(string.Format(ChlkResources.ERR_MARKING_PERIOD_HAS_NO_CLASS, mp.Name, classId)));

            var res = Save(announcement, classId);
            SchoolLocator.AnnouncementService.SubmitAnnouncement(res.Id, classId);
            SchoolLocator.AnnouncementService.DeleteAnnouncements(classId, res.ClassAnnouncementTypeRef.Value,
                                                                  AnnouncementState.Draft);


            //TODO: mixpanelService
            // var submittedAnn = SchoolLocator.AnnouncementService.GetAnnouncementDetails(res.Id);
            //if (res.StateTyped == AnnouncementState.Created)
            //    MixPanelService.CreatedNewItem(SchoolLocator.Context.UserName, submittedAnn.AnnouncementTypeName, submittedAnn.ClassName, submittedAnn.ApplicationCount.Value, submittedAnn.AttachmentsCount.Value);

            //if (submittedAnn.ApplicationCount.Value > 0)
            //{
            //    var apps = res.AnnouncementApplications.Select(x => x.Application.Name).ToList();
            //    MixPanelService.AttachedApp(SchoolLocator.Context.UserName, apps);
            //}
            //if (submittedAnn.AttachmentsCount.Value > 0)
            //{
            //    var docs = res.AnnouncementAttachments.Select(x => x.Name).ToList();
            //    MixPanelService.AttachedDocument(ServiceLocator.Context.UserName, docs);
            //}
            return Json(true, 5);
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult SubmitForAdmin(AnnouncementInfo announcement, ListOfStringList annRecipients)
        {
            if (!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            if (annRecipients.Count == 0)
                throw new ChalkableException();
            var recipientInfos = RecipientInfo.Create(annRecipients);
            var res = Save(announcement, null, recipientInfos);
            SchoolLocator.AnnouncementService.SubmitForAdmin(res.Id);
            SchoolLocator.AnnouncementService.DeleteAnnouncements(Context.UserLocalId.Value);
            return Json(true, 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult ListLast(int personId, int classId, int classAnnouncementTypeId)
        {
            var res = SchoolLocator.AnnouncementService.GetLastFieldValues(personId, classId, classAnnouncementTypeId);
            return Json(res.GroupBy(x => x).Select(x => x.Key));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult DropAnnouncement(int announcementId)
        {
            SchoolLocator.AnnouncementService.DropUnDropAnnouncement(announcementId, true);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult UndropAnnouncement(int announcementId)
        {
            SchoolLocator.AnnouncementService.DropUnDropAnnouncement(announcementId, false);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult AddStandard(int announcemntId, int standardId)
        {
            SchoolLocator.AnnouncementService.AddAnnouncementStandard(announcemntId, standardId);
            return Json(PrepareFullAnnouncementViewData(announcemntId, true, true));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult RemoveStandard(int announcemntId, int standardId)
        {
            SchoolLocator.AnnouncementService.AddAnnouncementStandard(announcemntId, standardId);
            return Json(PrepareFullAnnouncementViewData(announcemntId, true, true));
        }
    }

}