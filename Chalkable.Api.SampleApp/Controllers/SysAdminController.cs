using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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