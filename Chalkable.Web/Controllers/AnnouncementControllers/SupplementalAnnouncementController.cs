using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.ActionFilters;
using Chalkable.API.Models;
using Chalkable.Common.Exceptions;

namespace Chalkable.Web.Controllers.AnnouncementControllers
{
    public class SupplementalAnnouncementController : AnnouncementController
    {
        [AuthorizationFilter("Teacher")]
        public ActionResult CreateSupplemental(int classId,  DateTime? expiresDate, int? classAnnouncementTypeId)
        {
            if (classAnnouncementTypeId == null)
            {
                var classAnnTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId);
                if (classAnnTypes.Count == 0)
                    throw new NoClassAnnouncementTypeException("Item can't be created. Current Class doesn't have categories(class announcement types)");
                classAnnouncementTypeId = classAnnTypes.First().Id;
            }

            var res = SchoolLocator.SupplementalAnnouncementService.Create(classId, GenerateDefaultExpiresDate(expiresDate), classAnnouncementTypeId.Value);
            return Json(PrepareCreateAnnouncementViewData(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Save(int supplementalAnnouncementPlanId, int classId, string title, string content, int? classAnnouncementTypeId,
            DateTime? expiresDate, bool hideFromStudents, IList<AssignedAttributeInputModel> attributes, IntList recipientsIds)
        {
            SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.Supplemental, supplementalAnnouncementPlanId, attributes);
            var res = SchoolLocator.SupplementalAnnouncementService.Edit(supplementalAnnouncementPlanId, classId, classAnnouncementTypeId, title, content, expiresDate, !hideFromStudents, recipientsIds);
            return Json(PrepareAnnouncmentViewDataForEdit(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Submit(int supplementalAnnouncementPlanId, int classId, string title, string content, int? classAnnouncementTypeId,
            DateTime? expiresDate, bool hideFromStudents, IList<AssignedAttributeInputModel> attributes, IntList recipientsIds)
        {
            SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.Supplemental, supplementalAnnouncementPlanId, attributes);
            var ann = SchoolLocator.SupplementalAnnouncementService.Edit(supplementalAnnouncementPlanId, classId, classAnnouncementTypeId, title, content, expiresDate, !hideFromStudents, recipientsIds);
            SchoolLocator.SupplementalAnnouncementService.Submit(supplementalAnnouncementPlanId);
            var supplementalAnnouncement = SchoolLocator.SupplementalAnnouncementService.GetSupplementalAnnouncementById(supplementalAnnouncementPlanId);
            //TODO delete old drafts 
            //TrackNewItemCreate(ann, (s, appsCount, doscCount) => s.CreateNewLessonPlan(Context.Login, supplementalAnnouncement.ClassName, appsCount, doscCount));
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
        public ActionResult MakeVisible(int supplementalAnnouncementPlanId)
        {
            SchoolLocator.SupplementalAnnouncementService.SetVisibleForStudent(supplementalAnnouncementPlanId, true);
            return Json(true);
        }
    }
}