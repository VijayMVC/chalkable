using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DistrictController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin")]
        public ActionResult List(int? start, int? count)
        {
            count = count ?? 10;
            start = start ?? 0;
            return Json(new {success = true});
        }

    }
}
