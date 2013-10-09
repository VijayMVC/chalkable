using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
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
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementController : AnnouncementBaseController
    {

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Create(int? announcementTypeId, Guid? classId)
        {
            if (!SchoolLocator.Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            if (classId.HasValue && !announcementTypeId.HasValue)
                throw new ChalkableException("Invalid method parameters");

            if (!announcementTypeId.HasValue)
            {
                if (SchoolLocator.Context.Role.Id == CoreRoles.TEACHER_ROLE.Id)
                {
                    var lastAnnouncement = SchoolLocator.AnnouncementService.GetAnnouncements(0, 1, true).FirstOrDefault();
                    announcementTypeId = lastAnnouncement != null ? lastAnnouncement.AnnouncementTypeRef : (int)SystemAnnouncementType.HW;   
                }
                if (BaseSecurity.IsAdminViewer(SchoolLocator.Context))
                    announcementTypeId = (int)SystemAnnouncementType.Admin;
            }
            else
            {
                var draft = SchoolLocator.AnnouncementService.GetLastDraft();
                if (draft != null && draft.AnnouncementTypeRef != announcementTypeId &&
                    !BaseSecurity.IsAdminViewer(SchoolLocator.Context))
                {
                    draft.AnnouncementTypeRef = announcementTypeId.Value;
                    Guid? markingPeriodId = null;
                    if (draft.MarkingPeriodClassRef.HasValue)
                        markingPeriodId = SchoolLocator.MarkingPeriodService.GetMarkingPeriodClass(draft.MarkingPeriodClassRef.Value).MarkingPeriodRef;
                    SchoolLocator.AnnouncementService.EditAnnouncement(AnnouncementInfo.Create(draft), markingPeriodId, classId);
                }
            }
            var annDetails = SchoolLocator.AnnouncementService.CreateAnnouncement(announcementTypeId.Value, classId);
            var attachments = AttachmentLogic.PrepareAttachmentsInfo(annDetails.AnnouncementAttachments);
            var avd = AnnouncementDetailedViewData.Create(annDetails, null, SchoolLocator.Context.UserId, attachments);
            var res = new CreateAnnouncementViewData
                {
                    Announcement = avd,
                    IsDraft = annDetails.IsDraft
                };
            return Json(res, 7);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Delete(Guid announcementId)
        {
            SchoolLocator.AnnouncementService.DeleteAnnouncement(announcementId);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult DeleteDrafts(Guid personId)
        {
            SchoolLocator.AnnouncementService.DeleteAnnouncements(personId);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Edit(Guid announcementId)
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

        private Announcement Save(AnnouncementInfo announcementInfo, Guid? markingPeriodId, Guid? classId, IList<RecipientInfo> recipientInfos = null)
        {
            if(!announcementInfo.ExpiresDate.HasValue)
                announcementInfo.ExpiresDate =  SchoolLocator.Context.NowSchoolTime;
            return SchoolLocator.AnnouncementService.EditAnnouncement(announcementInfo, markingPeriodId, classId, recipientInfos);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ANNOUNCEMENT_READ, true, CallType.Get, new[] { AppPermissionType.Announcement })]
        public ActionResult Read(Guid announcementId)
        {
            var res = PrepareFullAnnouncementViewData(announcementId, true, true);
            //if (res.SystemType != SystemAnnouncementType.Admin)
            //    MixPanelService.OpenedAnnouncement(SchoolLocator.Context., res.AnnouncementTypeName, res.Title, res.SchoolPersonName);
            return Json(res, 7);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Star(Guid announcementId, bool? star)
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
        public ActionResult SaveAnnouncement(AnnouncementInfo announcementInfo, Guid markingPeriodId, Guid? classId)
        {
            Save(announcementInfo, markingPeriodId, classId);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult SaveForAdmin(AnnouncementInfo announcement, ListOfStringList annRecipients)
        {
            var recipients = annRecipients != null ? RecipientInfo.Create(annRecipients) : null;
            Save(announcement,  null, null, recipients);
            return Json(true);
        }

        private MarkingPeriod GetMarkingPeriod(DateTime expiresDate, Guid? markingPeriodId)
        {
            if (!SchoolLocator.Context.SchoolId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CONTEXT_NO_SCHOOL_INFO_ID);
            if (markingPeriodId.HasValue)
            {
                var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId.Value);
                return markingPeriod.StartDate <= expiresDate && markingPeriod.EndDate >= expiresDate ? markingPeriod : null;
            }
            return SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(expiresDate);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult SubmitAnnouncement(AnnouncementInfo announcement, Guid classId, Guid? markingPeriodId)
        {
            var mp = GetMarkingPeriod(announcement.ExpiresDate ?? SchoolLocator.Context.NowSchoolTime, markingPeriodId);
            if (mp == null)
                throw new NoMarkingPeriodException();

            var mpClass = SchoolLocator.MarkingPeriodService.GetMarkingPeriodClass(classId, mp.Id);
            if (mpClass == null)
                return Json(new ChalkableException(string.Format(ChlkResources.ERR_MARKING_PERIOD_HAS_NO_CLASS, mp.Name, classId)));

            var res = Save(announcement, mp.Id, classId);
            SchoolLocator.AnnouncementService.SubmitAnnouncement(res.Id, classId, mp.Id);
            SchoolLocator.AnnouncementService.DeleteAnnouncements(classId, res.AnnouncementTypeRef, AnnouncementState.Draft);
            

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
            if (annRecipients.Count == 0)
                throw new ChalkableException();
            var recipientInfos = RecipientInfo.Create(annRecipients);
            var res = Save(announcement, null, null, recipientInfos);
            SchoolLocator.AnnouncementService.SubmitForAdmin(res.Id);
            SchoolLocator.AnnouncementService.DeleteAnnouncements(SchoolLocator.Context.UserId);
            return Json(true, 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult ListLast(Guid personId, Guid classId, int announcementTypeId)
        {
            var res = SchoolLocator.AnnouncementService.GetLastFieldValues(personId, classId, announcementTypeId);
            return Json(res.GroupBy(x => x).Select(x => x.Key));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult DropAnnouncement(Guid announcementId)
        {
            SchoolLocator.AnnouncementService.DropUnDropAnnouncement(announcementId, true);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult UndropAnnouncement(Guid announcementId)
        {
            SchoolLocator.AnnouncementService.DropUnDropAnnouncement(announcementId, false);
            return Json(true);
        }
    }
}