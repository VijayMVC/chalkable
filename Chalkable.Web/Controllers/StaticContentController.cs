using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Web.Tools;

namespace Chalkable.Web.Controllers
{
    public class StaticContentController : ChalkableController
    {
        public ActionResult GetRootUrl()
        {
            #if DEBUG
                return Json(new { url = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~/") });
            #else
                return Json(new {url = CompilerHelper.ScriptsRoot});
#endif
        }

    }
}
