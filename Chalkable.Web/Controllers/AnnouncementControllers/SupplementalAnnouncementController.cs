using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.ActionFilters;
using Chalkable.API.Models;

namespace Chalkable.Web.Controllers.AnnouncementControllers
{
    public class SupplementalAnnouncementController : AnnouncementController
    {
        [AuthorizationFilter("Teacher")]
        public ActionResult Create(int classId, DateTime expiresDate)
        {
            var res = SchoolLocator.SupplementalAnnouncementService.Create(classId, GenerateDefaultExpiresDate(expiresDate));
            return Json(PrepareCreateAnnouncementViewData(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Save(int supplementalAnnouncementPlanId, int classId, string title, string content, int? galleryCategoryId,
            DateTime? expiresDate, bool hideFromStudents, IList<AssignedAttributeInputModel> attributes)
        {
            SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.Supplemental, supplementalAnnouncementPlanId, attributes);
            var res = SchoolLocator.SupplementalAnnouncementService.Edit(supplementalAnnouncementPlanId, classId, galleryCategoryId, title, content, expiresDate, !hideFromStudents);

            if (res.SupplementalAnnouncementData?.GalleryCategoryRef != null)
            {
                MasterLocator.UserTrackingService.SavedSupplementalAnnouncementToGallery(Context.Login, title);
            }
            return Json(PrepareAnnouncmentViewDataForEdit(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Submit(int supplementalAnnouncementPlanId, int classId, string title, string content, int? galleryCategoryId,
            DateTime? expiresDate, bool hideFromStudents, IList<AssignedAttributeInputModel> attributes)
        {
            SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.LessonPlan, supplementalAnnouncementPlanId, attributes);
            var ann = SchoolLocator.SupplementalAnnouncementService.Edit(supplementalAnnouncementPlanId, classId, galleryCategoryId, title, content, expiresDate, !hideFromStudents);
            SchoolLocator.SupplementalAnnouncementService.Submit(supplementalAnnouncementPlanId);
            var lessonPlan = SchoolLocator.SupplementalAnnouncementService.GetSupplementalAnnouncementById(supplementalAnnouncementPlanId);
            //TODO delete old drafts 
            TrackNewItemCreate(ann, (s, appsCount, doscCount) => s.CreateNewLessonPlan(Context.Login, lessonPlan.ClassName, appsCount, doscCount));
            return Json(true, 5);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult SubmitRecipientsToSupplementalAnnouncement(int supplementalAnnouncementPlanId, IntList recipientsIds)
        {
            SchoolLocator.SupplementalAnnouncementService.SubmitRecipientsToSupplementalAnnouncement(supplementalAnnouncementPlanId, recipientsIds);
            return Json(true, 5);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult EditTitle(int supplementalAnnouncementPlanId, string title)
        {
            return EditTitle(supplementalAnnouncementPlanId, AnnouncementTypeEnum.Supplemental, title, t => SchoolLocator.LessonPlanService.ExistsInGallery(t, supplementalAnnouncementPlanId));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Exists(string title, int? excludeSupplementalAnnouncementPlanId)
        {
            return Json(SchoolLocator.SupplementalAnnouncementService.Exists(title, excludeSupplementalAnnouncementPlanId));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ExistsInGallery(string title, int? excludeSupplementalAnnouncementPlanId)
        {
            return Json(SchoolLocator.SupplementalAnnouncementService.ExistsInGallery(title, excludeSupplementalAnnouncementPlanId));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult MakeVisible(int supplementalAnnouncementPlanId)
        {
            SchoolLocator.SupplementalAnnouncementService.SetVisibleForStudent(supplementalAnnouncementPlanId, true);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ReplaceLessonPlanInGallery(int oldSupplementalAnnouncementId, int newSupplementalAnnouncementId)
        {
            SchoolLocator.SupplementalAnnouncementService.ReplaceSupplementalAnnouncementInGallery(oldSupplementalAnnouncementId, newSupplementalAnnouncementId);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult RemoveLessonPlanFromGallery(int supplementalAnnouncementId)
        {
            SchoolLocator.SupplementalAnnouncementService.RemoveFromGallery(supplementalAnnouncementId);
            return Json(true);
        }
    }
}