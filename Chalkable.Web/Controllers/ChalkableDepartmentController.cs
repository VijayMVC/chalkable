using System;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ChalkableDepartmentController : ChalkableController
    {
        private const string contentType = "text/html";
        [AuthorizationFilter("SysAdmin")]
        public ActionResult Create(string name, StringList keywords)
        {
            byte[] icon;
            string filename;
            GetFileFromRequest(out icon, out filename);
            var res = MasterLocator.ChalkableDepartmentService.Add(name, keywords, icon);
            return Json(ChalkableDepartmentViewData.Create(res), contentType);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Update(Guid chalkableDepartmentId, string name, StringList keywords)
        {
            byte[] icon;
            string filename;
            GetFileFromRequest(out icon, out filename);
            if (icon == null)
                icon = MasterLocator.DepartmentIconService.GetPicture(chalkableDepartmentId.ToString(), null, null);
            var res = MasterLocator.ChalkableDepartmentService.Edit(chalkableDepartmentId, name, keywords, icon);
            return Json(ChalkableDepartmentViewData.Create(res), contentType);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Delete(Guid chalkableDepartmentId)
        {
            MasterLocator.ChalkableDepartmentService.Delete(chalkableDepartmentId);
            return Json(true);
        }
        [Authorize]
        public ActionResult List()
        {
            var res = MasterLocator.ChalkableDepartmentService.GetChalkableDepartments();
            return Json(ChalkableDepartmentViewData.Create(res));
        }
        
        [Authorize]
        public ActionResult Info(Guid chalkableDepartmentId)
        {
            var res = MasterLocator.ChalkableDepartmentService.GetChalkableDepartmentById(chalkableDepartmentId);
            return Json(ChalkableDepartmentViewData.Create(res));
        }

    }
}