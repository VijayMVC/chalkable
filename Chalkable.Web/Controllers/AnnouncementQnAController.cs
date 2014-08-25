using System;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementQnAController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult List(int announcementId)
        {
            var res = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnAs(announcementId);
            return Json(AnnouncementQnAViewData.Create(res), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Ask(int announcementId, string question)
        {
            var qna = SchoolLocator.AnnouncementQnAService.AskQuestion(announcementId, question);
            var res = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnA(qna.Id);
            return Json(AnnouncementQnAViewData.Create(res), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Answer(int announcementQnAId, string question, string answer)
        {
            var qna = SchoolLocator.AnnouncementQnAService.Answer(announcementQnAId, question, answer);
            var res = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnA(qna.Id);
            return Json(AnnouncementQnAViewData.Create(res), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult EditAnswer(int announcementQnAId, string answer)
        {
            var annQnA = SchoolLocator.AnnouncementQnAService.EditAnswer(announcementQnAId, answer);
            var res = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnAs(annQnA.AnnouncementRef);
            return Json(AnnouncementQnAViewData.Create(res), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult EditQuestion(int announcementQnAId, string question)
        {
            var annQnA = SchoolLocator.AnnouncementQnAService.EditQuestion(announcementQnAId, question);
            var res = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnAs(annQnA.AnnouncementRef);
            return Json(AnnouncementQnAViewData.Create(res), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Delete(int announcementQnAId)
        {
            var annQnA = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnA(announcementQnAId);
            SchoolLocator.AnnouncementQnAService.Delete(announcementQnAId);
            var res = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnAs(annQnA.AnnouncementRef);
            return Json(AnnouncementQnAViewData.Create(res), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult MarkUnanswered(int announcementQnAId)
        {
            SchoolLocator.AnnouncementQnAService.MarkUnanswered(announcementQnAId);
            return Json(true);
        }

    }
}