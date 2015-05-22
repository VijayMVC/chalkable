using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementController : AnnouncementBaseController
    {
        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult CreateForAdmin(DateTime? expiresDate)
        {
            var annDetails = SchoolLocator.AnnouncementService.CreateAdminAnnouncement(GenerateDefaultExpiresDate(expiresDate));
            return Json(PrepareCreateAnnouncementViewData(annDetails));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Create(int? classAnnouncementTypeId, int? classId, DateTime? expiresDate)
        {
            //TODO : need to refactor all announcemnt cerate , edit logic later))
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
          
            var draft = SchoolLocator.AnnouncementService.GetLastDraft();
            if (draft != null && !classId.HasValue && !classAnnouncementTypeId.HasValue)
            {
                classAnnouncementTypeId = draft.ClassAnnouncementTypeRef;
                classId = draft.ClassRef;
            }
            var classAnnType = classAnnouncementTypeId.HasValue
                ? SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypeById(classAnnouncementTypeId.Value)
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
                var annExpiredDate = GenerateDefaultExpiresDate(expiresDate);
                var annDetails = SchoolLocator.AnnouncementService.CreateAnnouncement(classAnnType, classId.Value, annExpiredDate);
                
                // annExporedDate was assigned to have a correct announcements orderings, lets clear it if not user-provided
                if (!expiresDate.HasValue && annDetails.Expires == annExpiredDate)
                    annDetails.Expires = DateTime.MinValue;

                Debug.Assert(Context.PersonId.HasValue);
                return Json(PrepareCreateAnnouncementViewData(annDetails));
            }
            return Json(null, 7);
        }

        private CreateAnnouncementViewData PrepareCreateAnnouncementViewData(AnnouncementDetails annDetails)
        {
            return new CreateAnnouncementViewData
                {
                    Announcement = PrepareAnnouncmentViewDataForEdit(annDetails),
                    IsDraft = annDetails.IsDraft,
                };
        }
        
        private DateTime GenerateDefaultExpiresDate(DateTime? expiresDate)
        {
            return expiresDate.HasValue ? expiresDate.Value :
                                    Context.SchoolYearEndDate.HasValue ? Context.SchoolYearEndDate.Value :
                                    DateTime.MaxValue;
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
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
        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult DeleteDrafts(int personId)
        {
            SchoolLocator.AnnouncementService.DeleteAnnouncements(personId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult EditTitle(int announcementId, string title)
        {
            var ann = SchoolLocator.AnnouncementService.GetAnnouncementById(announcementId);
            if (ann.ClassRef.HasValue && !Exists(title, ann.ClassRef.Value, ann.Expires, announcementId))
            {
                SchoolLocator.AnnouncementService.EditTitle(announcementId, title);
                return Json(true);
            }
            return Json(false);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult Exists(string title, int classId, DateTime? expiresDate, int? excludeAnnouncementId)
        {
            return Json(!expiresDate.HasValue || Exists(title, classId, expiresDate.Value, excludeAnnouncementId));
        }

        private bool Exists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            return SchoolLocator.AnnouncementService.Exists(title, classId, expiresDate, excludeAnnouncementId)
                   && !string.IsNullOrEmpty(title);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult Edit(int announcementId)
        {
            var viewData = PrepareFullAnnouncementViewData(announcementId);
            var res = new CreateAnnouncementViewData {Announcement = viewData};
            return Json(res, 6);
        }

        private AnnouncementDetails Save(AnnouncementInfo announcementInfo, int? classId, IList<RecipientInfo> recipientInfos = null)
        {
            // get announcement to ensure it exists
            SchoolLocator.AnnouncementService.GetAnnouncementById(announcementInfo.AnnouncementId);
            return SchoolLocator.AnnouncementService.EditAnnouncement(announcementInfo, classId);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult Read(int announcementId)
        {
            var res = PrepareFullAnnouncementViewData(announcementId, true);
            MasterLocator.UserTrackingService.OpenedAnnouncement(Context.Login, res.AnnouncementTypeName, res.Title, res.PersonName);
            return Json(res, 7);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
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

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult MakeVisible(int announcementId)
        {
            SchoolLocator.AnnouncementService.SetVisibleForStudent(announcementId, true);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult SaveAnnouncement(AnnouncementInfo announcementInfo, int? classId, ListOfStringList annRecipients)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var recipients = annRecipients != null ? RecipientInfo.Create(annRecipients) : null;
            var ann = Save(announcementInfo, classId, recipients);
            return Json(PrepareAnnouncmentViewDataForEdit(ann));
        }
        
        [AuthorizationFilter("Teacher")]
        public ActionResult SubmitAnnouncement(AnnouncementInfo announcement, int classId)
        {
            var res = Save(announcement, classId);
            SchoolLocator.AnnouncementService.SubmitAnnouncement(res.Id, classId);
            SchoolLocator.AnnouncementService.DeleteAnnouncements(classId, res.ClassAnnouncementTypeRef, AnnouncementState.Draft);
            TrackNewItemCreate(res);
            return Json(true, 5);
        }

        private void TrackNewItemCreate(AnnouncementDetails ann)
        {
            MasterLocator.UserTrackingService.CreatedNewItem(Context.Login, ann.ClassAnnouncementTypeName, ann.ClassName, ann.ApplicationCount, ann.AttachmentsCount);   
            if (ann.ApplicationCount > 0)
            {
                var apps = ann.AnnouncementApplications.Select(x => x.Id.ToString()).ToList();
                MasterLocator.UserTrackingService.AttachedApp(Context.Login, apps);
            }
            if (ann.AttachmentsCount > 0)
            {
                var docs = ann.AnnouncementAttachments.Select(x => x.Name).ToList();
                MasterLocator.UserTrackingService.AttachedDocument(Context.Login, docs);
            }
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult SubmitForAdmin(AnnouncementInfo announcement, ListOfStringList annRecipients, DateTime expiresDate)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            if (annRecipients.Count == 0)
                throw new ChalkableException("Announcement Recipient param is empty. You can't sumbit announcement without selected recipients");
            var recipientInfos = RecipientInfo.Create(annRecipients);
            var res = Save(announcement, null, recipientInfos);
            SchoolLocator.AnnouncementService.SubmitForAdmin(res.Id);
            SchoolLocator.AnnouncementService.DeleteAnnouncements(Context.PersonId.Value);
            TrackNewItemCreate(res);
            return Json(true, 5);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult ListLast(int personId, int classId, int classAnnouncementTypeId)
        {
            var res = SchoolLocator.AnnouncementService.GetLastFieldValues(personId, classId, classAnnouncementTypeId);
            return Json(res.GroupBy(x => x).Select(x => x.Key));
        }


        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult DropAnnouncement(int announcementId)
        {
            SchoolLocator.AnnouncementService.DropUnDropAnnouncement(announcementId, true);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult UndropAnnouncement(int announcementId)
        {
            SchoolLocator.AnnouncementService.DropUnDropAnnouncement(announcementId, false);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult AddStandard(int announcementId, int standardId)
        {
            SchoolLocator.AnnouncementService.AddAnnouncementStandard(announcementId, standardId);
            return Json(PrepareFullAnnouncementViewData(announcementId));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult RemoveStandard(int announcementId, int standardId)
        {
            SchoolLocator.AnnouncementService.RemoveStandard(announcementId, standardId);
            return Json(PrepareFullAnnouncementViewData(announcementId));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult AnnouncementAttributesList(bool? activeOnly)
        {
            var res = SchoolLocator.AnnouncementAttributeService.GetList(activeOnly);
            return Json(AnnouncementAttributeViewData.Create(res));
        }
    }

}