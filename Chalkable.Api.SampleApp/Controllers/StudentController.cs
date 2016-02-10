using System;
using System.Web.Mvc;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class StudentController : BaseSampleAppController
    {
        public ActionResult Index()
        {
            PrepareBaseData(null);
            return View("App");
        }

        public ActionResult ViewMode(int announcementApplicationId)
        {
            throw new NotImplementedException();
        }

        public ActionResult Practice(string standardId, string commonCoreStandard, string standardName)
        {
            throw new NotImplementedException();
        }
    }
}