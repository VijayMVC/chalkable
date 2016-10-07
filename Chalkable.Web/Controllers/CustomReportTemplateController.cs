using System;
using System.Web.Mvc;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class CustomReportTemplateController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher")]
        public ActionResult List(int? type)
        {
            var res = MasterLocator.CustomReportTemplateService.GetList((TemplateType?) type);
            return Json(ShortCustomReportTemplateViewData.Create(res));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Create(string name, string layout, string style, Guid? headerId, Guid? footerId, int type)
        {
            byte[] icon;
            string filename;
            GetFileFromRequest(out icon, out filename);
            var res = MasterLocator.CustomReportTemplateService.Add(name, layout, style, icon, headerId, footerId, (TemplateType)type);
            return Json(CustomReportTemplateViewData.Create(res));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Update(Guid templateId, string name, string layout, string style, Guid? headerId, Guid? footerId, int type)
        {
            byte[] icon;
            string filename;
            GetFileFromRequest(out icon, out filename);
            if(icon == null)
                icon = MasterLocator.CustomReportTemplateService.GetIcon(templateId);
            var res = MasterLocator.CustomReportTemplateService.Edit(templateId, name, layout, style, icon, headerId, footerId, (TemplateType) type);
            return Json(CustomReportTemplateViewData.Create(res));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Delete(Guid templateId)
        {
            MasterLocator.CustomReportTemplateService.Delete(templateId);
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Info(Guid templateId)
        {
            var res = MasterLocator.CustomReportTemplateService.GetById(templateId);
            return Json(CustomReportTemplateViewData.Create(res));
        }
    }
}