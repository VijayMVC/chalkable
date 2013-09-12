﻿using System;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementQnAController : AnnouncementBaseController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult List(Guid announcementId)
        {
            var res = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnAs(announcementId);
            return Json(AnnouncementQnAViewData.Create(res), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Ask(Guid announcementId, string question)
        {
            SchoolLocator.AnnouncementQnAService.AskQuestion(announcementId, question);
            var res = PrepareFullAnnouncementViewData(announcementId, true, true);
            return Json(res, 7);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Answer(Guid announcementQnAId, string question, string answer)
        {
            var announcementQn = SchoolLocator.AnnouncementQnAService.Answer(announcementQnAId, question, answer);
            var res = PrepareFullAnnouncementViewData(announcementQn.AnnouncementRef, true, true);
            return Json(res, 7);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult EditAnswer(Guid announcementQnAId, string answer)
        {
            var annQnA = SchoolLocator.AnnouncementQnAService.EditAnswer(announcementQnAId, answer);
            var res = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnAs(annQnA.AnnouncementRef);
            return Json(AnnouncementQnAViewData.Create(res), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult EditQuestion(Guid announcementQnAId, string question)
        {
            var annQnA = SchoolLocator.AnnouncementQnAService.EditQuestion(announcementQnAId, question);
            var res = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnAs(annQnA.AnnouncementRef);
            return Json(AnnouncementQnAViewData.Create(res), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Delete(Guid announcementQnAId)
        {
            var annQnA = SchoolLocator.AnnouncementQnAService.GetAnnouncmentQnA(announcementQnAId);
            SchoolLocator.AnnouncementQnAService.Delete(announcementQnAId);
            var res = SchoolLocator.AnnouncementQnAService.GetAnnouncementQnAs(annQnA.AnnouncementRef);
            return Json(AnnouncementQnAViewData.Create(res), 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult MarkUnanswered(Guid announcementQnAId)
        {
            var res = SchoolLocator.AnnouncementQnAService.MarkUnanswered(announcementQnAId);
            return Json(true);
        }

    }
}