using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Chalkable.Api.SampleApp.Models;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class TeacherController : BaseSampleAppController
    {
        public ActionResult Index()
        {
            PrepareBaseData(null);
            return View("App");
        }

        public async Task<ActionResult> Attach(int announcementApplicationId)
        {
            PrepareBaseData(announcementApplicationId);
            var annApp = await Connector.Announcement.GetAnnouncementApplicationById(announcementApplicationId);
            var updateAnnAppMeta = await Connector.Announcement.UpdateAnnouncementApplicationMeta(announcementApplicationId, annApp.AnnouncementType, "testForUpdateAnnAppMeta", "https://www.google.com");
            var ann = await Connector.Announcement.GetRead(annApp.AnnouncementId, annApp.AnnouncementType);

       //   var studentAnnouncementAppicationIds = await Connector.Announcement.StudentAnnouncementAppicationIds(179);
            var studentAnnouncementAppicationIdsForTeacher = await Connector.Announcement.StudentAnnouncementAppicationIdsForTeacher(3787, 179);
            return View("Attach", DefaultJsonViewData.Create(ann));
        }

        public ActionResult ViewMode(int announcementApplicationId)
        {
            return RedirectToAction("Attach", new RouteValueDictionary
            {
                {"announcementApplicationId", announcementApplicationId}
            });
        }

        public ActionResult GradingViewMode(int announcementApplicationId, int studentId)
        {
            throw new NotImplementedException();
        }
    }
}