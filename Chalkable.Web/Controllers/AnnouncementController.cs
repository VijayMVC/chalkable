using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services;
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
        public ActionResult Create(int? classAnnouncementTypeId, int? classId, DateTime? expiresDate)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            //if (classId.HasValue && !classAnnouncementTypeId.HasValue)
            //    throw new ChalkableException("Invalid method parameters");

            var draft = SchoolLocator.AnnouncementService.GetLastDraft();
            if (draft != null && !classId.HasValue && !classAnnouncementTypeId.HasValue)
            {
                classAnnouncementTypeId = draft.ClassAnnouncementTypeRef;
                classId = draft.ClassRef;
            }

            var classAnnType = classAnnouncementTypeId.HasValue
                ? SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementType(classAnnouncementTypeId.Value)
                : null;

            if (classId.HasValue && classAnnType != null && classAnnType.ClassRef != classId.Value)
            {
                classAnnType = null;
            }

            if (classAnnType == null && classId.HasValue)
            {
                var classAnnTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId.Value);
                if (classAnnTypes.Count == 0)
                    throw new NoClassAnnouncementTypeException("Item can't be created. Current Class doesn't have classAnnouncementTypes");

                classAnnType = classAnnTypes.First();
            }

            if (classAnnType != null && draft != null && (draft.ClassAnnouncementTypeRef != classAnnType.Id || draft.ClassRef != classId))
            {
                draft.ClassAnnouncementTypeRef = classAnnType.Id;
                SchoolLocator.AnnouncementService.EditAnnouncement(AnnouncementInfo.Create(draft), classId);
            }

            if (classId.HasValue && classAnnType != null)
            {
                var annExpiredDate = expiresDate.HasValue ? expiresDate.Value : 
                                    Context.SchoolYearEndDate.HasValue ? Context.SchoolYearEndDate.Value : 
                                    DateTime.MaxValue;
                var annDetails = SchoolLocator.AnnouncementService.CreateAnnouncement(classAnnType, classId.Value, annExpiredDate);

                // annExporedDate was assigned to have a correct announcements orderings, lets clear it if not user-provided
                if (!expiresDate.HasValue)
                    annDetails.Expires = DateTime.MinValue;

                var teachersIds = SchoolLocator.ClassService.GetClassTeachers(annDetails.ClassRef, null).Select(x => x.PersonRef).ToList();
                var attachments = AttachmentLogic.PrepareAttachmentsInfo(annDetails.AnnouncementAttachments, teachersIds);
                Debug.Assert(Context.PersonId.HasValue);
                var avd = AnnouncementDetailedViewData.Create(annDetails, null, Context.PersonId.Value, attachments);
                avd.CanAddStandard = SchoolLocator.AnnouncementService.CanAddStandard(annDetails.Id);
                avd.Applications = ApplicationLogic.PrepareAnnouncementApplicationInfo(SchoolLocator, MasterLocator, annDetails.Id);
                avd.SuggestedApps = PrepareSuggestedAppsForAnnouncementViewData(annDetails);
                return Json(new CreateAnnouncementViewData
                {
                    Announcement = avd,
                    IsDraft = annDetails.IsDraft
                });
            }

            return Json(null, 7);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Delete(int announcementId)
        {
            SchoolLocator.AnnouncementService.DeleteAnnouncement(announcementId);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult DuplicateAnnouncement(int announcementId, IntList classIds)
        {
            SchoolLocator.AnnouncementService.CopyAnnouncement(announcementId, classIds);
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
            var ann = SchoolLocator.AnnouncementService.GetAnnouncementById(announcementId);
            if (!SchoolLocator.AnnouncementService.Exists(title, ann.ClassRef, ann.Expires, announcementId) && !string.IsNullOrEmpty(title))
            {
                SchoolLocator.AnnouncementService.EditTitle(announcementId, title);
                return Json(true);
            }
            return Json(false);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Exists(string title, int classId, DateTime? expiresDate, int? excludeAnnouncementId)
        {
            return Json(!expiresDate.HasValue || (SchoolLocator.AnnouncementService.Exists(title, classId, expiresDate.Value, excludeAnnouncementId) && !string.IsNullOrEmpty(title)));
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

        private AnnouncementDetails Save(AnnouncementInfo announcementInfo, int? classId)
        {
            // get announcement to ensure it exists
            SchoolLocator.AnnouncementService.GetAnnouncementById(announcementInfo.AnnouncementId);

            return SchoolLocator.AnnouncementService.EditAnnouncement(announcementInfo, classId);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student",
            Preference.API_DESCR_ANNOUNCEMENT_READ, true, CallType.Get, new[] {AppPermissionType.Announcement})]
        public ActionResult Read(int announcementId)
        {
            var res = PrepareFullAnnouncementViewData(announcementId, true, true);
            //if (res.SystemType != SystemAnnouncementType.Admin)
            MasterLocator.UserTrackingService.OpenedAnnouncement(Context.Login, res.AnnouncementTypeName, res.Title, res.PersonName);
            return Json(res, 7);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Complete(int announcementId, bool? complete)
        {
            if (!complete.HasValue)
            {
                var prev = SchoolLocator.AnnouncementService.GetAnnouncementDetails(announcementId).Complete;
                complete = !prev;
            }
            SchoolLocator.AnnouncementService.SetComplete(announcementId, complete.Value);
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
            if(!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException();
            var ann = Save(announcementInfo, classId);
            var res = AnnouncementDetailedViewData.Create(ann
                , SchoolLocator.GradingStyleService.GetMapper(), Context.SchoolLocalId.Value);
            res.CanAddStandard = SchoolLocator.AnnouncementService.CanAddStandard(ann.Id);
            res.Applications = ApplicationLogic.PrepareAnnouncementApplicationInfo(SchoolLocator, MasterLocator, ann.Id);
            return Json(res);
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
            SchoolLocator.AnnouncementService.DeleteAnnouncements(classId, res.ClassAnnouncementTypeRef, AnnouncementState.Draft);

            MasterLocator.UserTrackingService.CreatedNewItem(Context.Login, res.ClassAnnouncementTypeName, res.ClassName, res.ApplicationCount, res.AttachmentsCount);

            if (res.ApplicationCount > 0)
            {
                var apps = res.AnnouncementApplications.Select(x => x.Id.ToString()).ToList();
                MasterLocator.UserTrackingService.AttachedApp(Context.Login, apps);
            }
            if (res.AttachmentsCount > 0)
            {
                var docs = res.AnnouncementAttachments.Select(x => x.Name).ToList();
                MasterLocator.UserTrackingService.AttachedDocument(Context.Login, docs);
            }
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
        public ActionResult AddStandard(int announcementId, int standardId)
        {
            SchoolLocator.AnnouncementService.AddAnnouncementStandard(announcementId, standardId);
            return Json(PrepareFullAnnouncementViewData(announcementId, true, false));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult RemoveStandard(int announcementId, int standardId)
        {
            SchoolLocator.AnnouncementService.RemoveStandard(announcementId, standardId);
            return Json(PrepareFullAnnouncementViewData(announcementId, true, false));
        }

    }

}