using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.Common.Exceptions;
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

        public ActionResult GetPictureRootUrl(bool includeDistrictId = true)
        {
            if (Context == null || Context.DistrictId == null)
                throw new ChalkableException("User is not authorized");
            var districtId = includeDistrictId ? Context.DistrictId.ToString() : "";
            return Json(new {url = PictureService.GetPicturesRelativeAddress() + districtId});
        }
    }
}
