using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.Common.Exceptions;

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

        public ActionResult GetPictureRootUrl()
        {
            if (Context.DistrictId == null)
                throw new ChalkableException("User is not authorized");
            return Json(new {url = PictureService.GetPicturesRelativeAddress() + Context.DistrictId});
        }
    }
}
