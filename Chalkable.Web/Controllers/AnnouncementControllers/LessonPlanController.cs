using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers.AnnouncementControllers
{
    [RequireHttps, TraceControllerFilter]
    public class LessonPlanController : AnnouncementController
    {
        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult CreateLessonPlan(int? classId)
        {
            var res = SchoolLocator.LessonPlanService.Create(classId, Context.NowSchoolTime, Context.NowSchoolTime);
            return Json(PrepareCreateAnnouncementViewData(res));
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
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

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult Save(int lessonPlanId, int? classId, string title, string content, int? lpGalleryCategoryId,
            DateTime? startDate, DateTime? endDate, bool hideFromStudents, bool inGallery, IList<AssignedAttributeInputModel> attributes)
        {
            SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.LessonPlan, lessonPlanId, attributes);
            var res = SchoolLocator.LessonPlanService.Edit(lessonPlanId, classId, lpGalleryCategoryId, title, content, startDate, endDate, !hideFromStudents, inGallery);

            //if (res.LessonPlanData?.LpGalleryCategoryRef != null)
            //{
            //    MasterLocator.UserTrackingService.SavedLessonPlanToGallery(Context.Login, title);
            //}
            return Json(PrepareAnnouncmentViewDataForEdit(res));
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult Submit(int lessonPlanId, int? classId, string title, string content, int? lpGalleryCategoryId,
            DateTime? startDate, DateTime? endDate, bool hideFromStudents, bool inGallery, IList<AssignedAttributeInputModel> attributes)
        {
            if (inGallery)
            {
                if (!Context.SCEnabled)
                    throw new ChalkableException("Cannot create lesson plan template, Study Center disabled!");
                if (lpGalleryCategoryId == null)
                    throw new ChalkableException("Cannot create lesson plan template without category!");
            }

            if (Context.Role == CoreRoles.TEACHER_ROLE)
            {
                SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.LessonPlan, lessonPlanId, attributes);
                var ann = SchoolLocator.LessonPlanService.Edit(lessonPlanId, classId, lpGalleryCategoryId, title, content, startDate, endDate, !hideFromStudents, false);
                SchoolLocator.LessonPlanService.Submit(lessonPlanId);
                var lessonPlan = SchoolLocator.LessonPlanService.GetLessonPlanById(lessonPlanId);
                //TODO delete old drafts 
                TrackNewItemCreate(ann, (s, appsCount, doscCount) => s.CreateNewLessonPlan(Context.Login, lessonPlan.ClassName, appsCount, doscCount));
            }

            if (inGallery)
            {
                var lpGalleryId = Context.Role == CoreRoles.DISTRICT_ADMIN_ROLE ? lessonPlanId : SchoolLocator.LessonPlanService.Create(null, startDate, endDate).LessonPlanData.Id;

                SchoolLocator.AnnouncementAssignedAttributeService.Edit(AnnouncementTypeEnum.LessonPlan, lpGalleryId, attributes);
                SchoolLocator.LessonPlanService.Edit(lpGalleryId, null, lpGalleryCategoryId, title + " Template", content, startDate, endDate, !hideFromStudents, true);
                SchoolLocator.LessonPlanService.Submit(lpGalleryId);

                if (Context.Role == CoreRoles.TEACHER_ROLE)
                {
                    SchoolLocator.LessonPlanService.CopyToGallery(lessonPlanId, lpGalleryId);
                    MasterLocator.UserTrackingService.SavedLessonPlanToGallery(Context.Login, title);
                }
            }
                

            return Json(true, 5);
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult EditTitle(int announcementId, string title)
        {
            return EditTitle(announcementId, AnnouncementTypeEnum.LessonPlan, title, t => SchoolLocator.LessonPlanService.ExistsInGallery(t, announcementId));              
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult Exists(string title, int? excludeLessonPlanId)
        {
           return Json(SchoolLocator.LessonPlanService.Exists(title, excludeLessonPlanId));
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
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

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult ReplaceLessonPlanInGallery(int oldLessonPlanId, int newLessonPlanId)
        {
            SchoolLocator.LessonPlanService.ReplaceLessonPlanInGallery(oldLessonPlanId, newLessonPlanId);
            return Json(true);
        }

        [AuthorizationFilter("Teacher, DistrictAdmin")]
        public ActionResult RemoveLessonPlanFromGallery(int lessonPlanId)
        {
            SchoolLocator.LessonPlanService.RemoveFromGallery(lessonPlanId);
            return Json(true);
        }
    }
}