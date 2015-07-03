using System;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AnnouncementsViewData;

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
        public ActionResult SearchLessonPlansTemplates(int? galleryCategoryId, int? classId, string filter)
        {
            var lessonPlans = SchoolLocator.LessonPlanService.GetLessonPlansTemplates(galleryCategoryId, filter, classId);
            return Json(LessonPlanViewData.Create(lessonPlans));
        }
        
        [AuthorizationFilter("Teacher")]
        public ActionResult CreateFromTemplate(int lessonPlanTplId, int classId)
        {
            var res = SchoolLocator.LessonPlanService.CreateFromTemplate(lessonPlanTplId, classId);
            return Json(PrepareCreateAnnouncementViewData(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Save(int lessonPlanId, int classId, string title, string content, int? galleryCategoryId, DateTime? startDate, DateTime? endDate, bool hideFromStudents)
        {
            var res = SchoolLocator.LessonPlanService.Edit(lessonPlanId, classId, galleryCategoryId, title, content, startDate, endDate, !hideFromStudents);
            return Json(PrepareAnnouncmentViewDataForEdit(res));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Submit(int lessonPlanId, int classId, string title, string content, int? galleryCategoryId, DateTime? startDate, DateTime? endDate, bool hideFromStudents)
        {
            var ann = SchoolLocator.LessonPlanService.Edit(lessonPlanId, classId, galleryCategoryId, title, content, startDate, endDate, !hideFromStudents);
            SchoolLocator.LessonPlanService.Submit(lessonPlanId);
            var lessonPlan = SchoolLocator.LessonPlanService.GetLessonPlanById(lessonPlanId);
            //TODO delete old drafts 
            TrackNewItemCreate(ann, (s, appsCount, doscCount) => s.CreateNewLessonPlan(Context.Login, lessonPlan.ClassName, appsCount, doscCount));
            return Json(true, 5);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult Exists(string title, int? excludeLessonPlanId)
        {
           return Json(SchoolLocator.LessonPlanService.Exists(title, excludeLessonPlanId));
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
    }
}