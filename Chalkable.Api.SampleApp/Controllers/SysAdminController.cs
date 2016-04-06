using System.Web.Mvc;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class SysAdminController : BaseSampleAppController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}