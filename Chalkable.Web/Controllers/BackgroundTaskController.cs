using System;
using System.Web.Mvc;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class BackgroundTaskController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin")]
        public ActionResult GetTasks(int? start, int? count, int? state)
        {
            BackgroundTaskStateEnum? st = (BackgroundTaskStateEnum?)state;
            var tasks = MasterLocator.BackgroundTaskService.Find(null, st, null, true, start ?? 0, count ?? 10);
            return Json(tasks.Transform(BackgroundTaskViewData.Create));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult GetTaskLogs(Guid taskId, int? start, int? count)
        {
            var logs = MasterLocator.BackgroundTaskService.GetLogItems(taskId, start ?? 0, count ?? 10);
            return Json(logs.Transform(BackgroundTaskLogViewData.Create));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Cancel(Guid taskId)
        {
            MasterLocator.BackgroundTaskService.Cancel(taskId);
            return Json(true);
        }
    }
}