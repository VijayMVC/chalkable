﻿using System;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class BackgroundTaskController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin")]
        public ActionResult GetTasks(int? start, int? count)
        {
            var tasks = MasterLocator.BackgroundTaskService.GetTasks(start ?? 0, count ?? 10);
            return Json(tasks.Transform(BackgroundTaskViewData.Create));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult GetTaskLogs(Guid taskId, int? start, int? count)
        {
            var logs = MasterLocator.BackgroundTaskService.GetLogItems(taskId, start ?? 0, count ?? 10);
            return Json(logs.Transform(BackgroundTaskLogViewData.Create));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Delete(Guid taskId)
        {
            MasterLocator.BackgroundTaskService.Delete(taskId);
            return Json(true);
        }
    }
}