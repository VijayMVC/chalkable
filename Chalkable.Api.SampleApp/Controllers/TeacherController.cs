using System;
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

        public ActionResult Attach(int announcementApplicationId)
        {
            PrepareBaseData(announcementApplicationId);
            throw new NotImplementedException();
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