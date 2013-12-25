using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementReminderController : AnnouncementBaseController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult AddReminder(int announcementId, int? before)
        {
            SchoolLocator.AnnouncementReminderService.AddReminder(announcementId, before);
            var view = PrepareFullAnnouncementViewData(announcementId);
            return Json(view, 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult EditReminder(int announcementReminderId, int? before)
        {
            AnnouncementReminder res = SchoolLocator.AnnouncementReminderService.EditReminder(announcementReminderId, before);
            var view = PrepareFullAnnouncementViewData(res.AnnouncementRef);
            return Json(view, 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult DeleteReminder(int announcementReminderId)
        {
            Announcement res = SchoolLocator.AnnouncementReminderService.DeleteReminder(announcementReminderId);
            var view = PrepareFullAnnouncementViewData(res.Id);
            return Json(view, 5);
        }
    }
}