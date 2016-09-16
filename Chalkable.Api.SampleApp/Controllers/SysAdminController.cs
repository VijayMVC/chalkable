using System.Web.Mvc;
using Chalkable.Api.SampleApp.Logic;
using Chalkable.Api.SampleApp.Models;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class SysAdminController : BaseSampleAppController
    {
        public ActionResult Index()
        {
            var history = AnnAppHistoryStorage.GetStorage().GetHistory(null);
            var res = DefaultJsonViewData.Create(new
            {
                History = history
            });
            return View("Index", res);
        }
    }
}