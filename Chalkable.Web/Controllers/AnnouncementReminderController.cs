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
        public ActionResult AddReminder(Guid announcementId, int? before)
        {
            AnnouncementReminder res = SchoolLocator.AnnouncementReminderService.AddReminder(announcementId, before);
            var view = PrepareFullAnnouncementViewData(res.Id);
            return Json(view, 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult EditReminder(Guid announcementRemiderId, int? before)
        {
            AnnouncementReminder res = SchoolLocator.AnnouncementReminderService.EditReminder(announcementRemiderId, before);
            var view = PrepareFullAnnouncementViewData(res.AnnouncementRef);
            return Json(view, 5);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult DeleteReminder(Guid announcementReminderId)
        {
            Announcement res = SchoolLocator.AnnouncementReminderService.DeleteReminder(announcementReminderId);
            var view = PrepareFullAnnouncementViewData(res.Id);
            return Json(view, 5);
        }
    }
}