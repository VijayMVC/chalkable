using System;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class CustomReportTemplateController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher")]
        public ActionResult List()
        {
            var res = MasterLocator.CustomReportTemplateService.GetList();
            return Json(ShortCustomReportTemplateViewData.Create(res));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Create(string name, string layout, string style)
        {

            byte[] icon;
            string filename;
            GetFileFromRequest(out icon, out filename);
            var res = MasterLocator.CustomReportTemplateService.Add(name, layout, style, icon);
            return Json(CustomReportTemplateViewData.Create(res));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Update(Guid templateId, string name, string layout, string style)
        {
            byte[] icon;
            string filename;
            GetFileFromRequest(out icon, out filename);
            if(icon == null)
                icon = MasterLocator.CustomReportTemplateService.GetIcon(templateId);
            var res = MasterLocator.CustomReportTemplateService.Edit(templateId, name, layout, style, icon);
            return Json(CustomReportTemplateViewData.Create(res));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Delete(Guid templateId)
        {
            MasterLocator.CustomReportTemplateService.Delete(templateId);
            return Json(true);
        }
    }
}