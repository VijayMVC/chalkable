using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers.AnnouncementControllers
{
    [RequireHttps, TraceControllerFilter]
    public class LessonPlanController : AnnouncementController
    {
        [AuthorizationFilter("Teacher")]
        public ActionResult CreateLessonPlan(int classId)
        {
            var res = SchoolLocator.LessonPlanService.Create(classId, Context.NowSchoolTime, Context.NowSchoolTime);
            return Json(PrepareCreateAnnouncementViewData(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult LessonPlanTemplates(string filter, int? categoryType, int? sortType, int? state, int? start, int? count)
        {
            var st = start ?? 0;
            var cnt = count ?? 12;
            var sort = (AttachmentSortTypeEnum?) sortType ?? AttachmentSortTypeEnum.OldestUploaded;
            var lessonPlans = SchoolLocator.LessonPlanService.GetLessonPlansTemplates(categoryType, filter, null, sort, st, cnt, (AnnouncementState?)state);
            return Json(lessonPlans.Transform(LessonPlanViewData.Create));
        }
        
        [AuthorizationFilter("Teacher")]
        public ActionResult CreateFromTemplate(int lessonPlanTplId, int classId)
        {
            var res = SchoolLocator.LessonPlanService.CreateFromTemplate(lessonPlanTplId, classId);
            MasterLocator.UserTrackingService.CopiedLessonPlanFromGallery(Context.Login);
            return Json(PrepareCreateAnnouncementViewData(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Save(int lessonPlanId, int classId, string title, string content, int? galleryCategoryId,
            DateTime? startDate, DateTime? endDate, bool hideFromStudents, IList<AssignedAttributeInputModel> attributes)
        {
            SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.LessonPlan, lessonPlanId, attributes);
            var res = SchoolLocator.LessonPlanService.Edit(lessonPlanId, classId, galleryCategoryId, title, content, startDate, endDate, !hideFromStudents);

            if (res.LessonPlanData?.GalleryCategoryRef != null)
            {
                MasterLocator.UserTrackingService.SavedLessonPlanToGallery(Context.Login, title);
            }
            return Json(PrepareAnnouncmentViewDataForEdit(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Submit(int lessonPlanId, int classId, string title, string content, int? galleryCategoryId,
            DateTime? startDate, DateTime? endDate, bool hideFromStudents, IList<AssignedAttributeInputModel> attributes)
        {
            SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.LessonPlan, lessonPlanId, attributes);
            var ann = SchoolLocator.LessonPlanService.Edit(lessonPlanId, classId, galleryCategoryId, title, content, startDate, endDate, !hideFromStudents);
            SchoolLocator.LessonPlanService.Submit(lessonPlanId);
            var lessonPlan = SchoolLocator.LessonPlanService.GetLessonPlanById(lessonPlanId);
            //TODO delete old drafts 
            TrackNewItemCreate(ann, (s, appsCount, doscCount) => s.CreateNewLessonPlan(Context.Login, lessonPlan.ClassName, appsCount, doscCount));
            return Json(true, 5);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult EditTitle(int announcementId, string title)
        {
            return EditTitle(announcementId, AnnouncementTypeEnum.LessonPlan, title, t => SchoolLocator.LessonPlanService.ExistsInGallery(t, announcementId));              
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Exists(string title, int? excludeLessonPlanId)
        {
           return Json(SchoolLocator.LessonPlanService.Exists(title, excludeLessonPlanId));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ExistsInGallery(string title, int? excludedLessonPlanId)
        {
            return Json(SchoolLocator.LessonPlanService.ExistsInGallery(title, excludedLessonPlanId));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ListLast(int classId)
        {
            return Json(SchoolLocator.LessonPlanService.GetLastFieldValues(classId));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult MakeVisible(int lessonPlanId)
        {
            SchoolLocator.LessonPlanService.SetVisibleForStudent(lessonPlanId, true);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult DuplicateLessonPlan(int lessonPlanId, IntList classIds)
        {
            SchoolLocator.LessonPlanService.DuplicateLessonPlan(lessonPlanId, classIds);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ReplaceLessonPlanInGallery(int oldLessonPlanId, int newLessonPlanId)
        {
            SchoolLocator.LessonPlanService.ReplaceLessonPlanInGallery(oldLessonPlanId, newLessonPlanId);
            return Json(true);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult RemoveLessonPlanFromGallery(int lessonPlanId)
        {
            SchoolLocator.LessonPlanService.RemoveFromGallery(lessonPlanId);
            return Json(true);
        }
    }
}