﻿using System;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ChalkableDepartmentController : ChalkableController
    {
        private const string contentType = "text/html";
        [AuthorizationFilter("System Admin")]
        public ActionResult Create(string name, StringList keywords)
        {
            byte[] icon;
            string filename;
            GetFileFromRequest(out icon, out filename);
            var res = MasterLocator.ChalkableDepartmentService.Add(name, keywords, icon);
            var actionResult = Json(ChalkableDepartmentViewData.Create(res));
            if (actionResult is JsonResult)
            {
                (actionResult as JsonResult).ContentType = contentType;
            }
            return actionResult;
        }

        [AuthorizationFilter("System Admin")]
        public ActionResult Update(Guid id, string name, StringList keywords)
        {
            byte[] icon;
            string filename;
            GetFileFromRequest(out icon, out filename);
            if (icon == null)
                icon = MasterLocator.DepartmentIconService.GetPicture(id, null, null);
            var res = MasterLocator.ChalkableDepartmentService.Edit(id, name, keywords, icon);
            var actionResult = Json(ChalkableDepartmentViewData.Create(res));
            if (actionResult is JsonResult)
            {
                (actionResult as JsonResult).ContentType = contentType;
            }
            return actionResult;
        }

        [AuthorizationFilter("System Admin")]
        public ActionResult Delete(Guid id)
        {
            MasterLocator.ChalkableDepartmentService.Delete(id);
            return Json(true);
        }
        [Authorize]
        public ActionResult List()
        {
            var res = MasterLocator.ChalkableDepartmentService.GetChalkableDepartments();
            return Json(ChalkableDepartmentViewData.Create(res));
        }       
    }
}