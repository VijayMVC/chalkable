using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.ActionFilters;

namespace Chalkable.Web.Controllers.AnnouncementControllers
{
    [RequireHttps, TraceControllerFilter]
    public class ClassAnnouncementController : AnnouncementController
    {
        [AuthorizationFilter("Teacher")]
        public ActionResult CreateClassAnnouncement(int? classAnnouncementTypeId, int classId, DateTime? expiresDate)
        {
            //TODO : need to refactor all announcemnt cerate , edit logic later))
            Trace.Assert(Context.PersonId.HasValue);

            var draft = SchoolLocator.ClassAnnouncementService.GetLastDraft();
            var classAnnType = classAnnouncementTypeId.HasValue
                ? SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypeById(classAnnouncementTypeId.Value)
                : null;

            if (classAnnType != null && classAnnType.ClassRef != classId) classAnnType = null;
            
            if (classAnnType == null)
            {
                var classAnnTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId);
                if (classAnnTypes.Count == 0)
                    throw new NoClassAnnouncementTypeException("Item can't be created. Current Class doesn't have classAnnouncementTypes");
                classAnnType = classAnnTypes.First();
            }
            if (classAnnType != null && draft != null && (draft.ClassAnnouncementTypeRef != classAnnType.Id || draft.ClassRef != classId))
            {
                draft.ClassAnnouncementTypeRef = classAnnType.Id;
                var classAnnInfo = ClassAnnouncementInfo.Create(draft);
                classAnnInfo.ClassId = classId;
                SchoolLocator.ClassAnnouncementService.Edit(classAnnInfo);
            }

            if (classAnnType != null)
            {
                var annExpiredDate = GenerateDefaultExpiresDate(expiresDate);
                var annDetails = SchoolLocator.ClassAnnouncementService.Create(classAnnType, classId, annExpiredDate);
                var classAnn = annDetails.ClassAnnouncementData;
                
                // annExporedDate was assigned to have a correct announcements orderings, lets clear it if not user-provided
                if (!expiresDate.HasValue && classAnn.Expires == annExpiredDate)
                    classAnn.Expires = DateTime.MinValue;

                Debug.Assert(Context.PersonId.HasValue);
                return Json(PrepareCreateAnnouncementViewData(annDetails));
            }
            return Json(null, 7);
        }


        [AuthorizationFilter("Teacher")]
        public ActionResult SaveAnnouncement(ClassAnnouncementInfo classAnnouncementInfo, int? classId, IList<AssignedAttributeInputModel> attributes)
        {
            Trace.Assert(Context.PersonId.HasValue);
            SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.Class, classAnnouncementInfo.AnnouncementId, attributes);
            var ann = SchoolLocator.ClassAnnouncementService.Edit(classAnnouncementInfo);
            return Json(PrepareAnnouncmentViewDataForEdit(ann));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ListLast(int classId, int classAnnouncementTypeId)
        {
            return Json(SchoolLocator.ClassAnnouncementService.GetLastFieldValues(classId, classAnnouncementTypeId));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult SubmitAnnouncement(ClassAnnouncementInfo classAnnouncementInfo, IList<AssignedAttributeInputModel> attributes)
        {
            SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.Class, classAnnouncementInfo.AnnouncementId, attributes);
            var annDetails = SchoolLocator.ClassAnnouncementService.Edit(classAnnouncementInfo);
            SchoolLocator.ClassAnnouncementService.Submit(annDetails.Id);
            SchoolLocator.ClassAnnouncementService.DeleteAnnouncements(classAnnouncementInfo.ClassId, annDetails.ClassAnnouncementData.ClassAnnouncementTypeRef, AnnouncementState.Draft);
            TrackNewItemCreate(annDetails, (s, appsCount, doscCount)=> s.CreatedNewItem(Context.Login, annDetails.ClassAnnouncementData.ClassAnnouncementTypeName, annDetails.ClassAnnouncementData.ClassName, appsCount, doscCount));
            return Json(true, 5);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult DuplicateAnnouncement(int announcementId, IntList classIds)
        {
            SchoolLocator.ClassAnnouncementService.CopyAnnouncement(announcementId, classIds);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Drop(int announcementId)
        {
            SchoolLocator.ClassAnnouncementService.DropUnDropAnnouncement(announcementId, true);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Undrop(int announcementId)
        {
            SchoolLocator.ClassAnnouncementService.DropUnDropAnnouncement(announcementId, false);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult MakeVisible(int announcementId)
        {
            SchoolLocator.ClassAnnouncementService.SetVisibleForStudent(announcementId, true);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult EditTitle(int announcementId, string title)
        {
            var ann = SchoolLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);
            return EditTitle(announcementId, AnnouncementTypeEnum.Class, title, t => AnnouncementExists(t, ann.ClassRef, ann.Expires, announcementId));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult Exists(string title, int classId, DateTime? expiresDate, int? excludeAnnouncementId)
        {
            return Json(!expiresDate.HasValue || AnnouncementExists(title, classId, expiresDate.Value, excludeAnnouncementId));
        }

        protected bool AnnouncementExists(string title, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            return SchoolLocator.ClassAnnouncementService.Exists(title, classId, expiresDate, excludeAnnouncementId)
                   && !string.IsNullOrEmpty(title);
        }
    }
}