using System;
using System.Threading.Tasks;
using System.Web.Mvc;

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
            var ann = await Connector.Announcement.GetRead(annApp.AnnouncementId, annApp.AnnouncementType);
            return View("Attach", ann);
        }

        public ActionResult ViewMode(int announcementApplicationId)
        {
            throw new NotImplementedException();
        }

        public ActionResult GradingViewMode(int announcementApplicationId, int studentId)
        {
            throw new NotImplementedException();
        }
    }
}