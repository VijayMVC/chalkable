using System;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class BackgroundTaskController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin")]
        public ActionResult GetTasks(int? start, int? count, int? state, int? type, Guid? districtId, bool allDistricts = true)
        {
            var st = (BackgroundTaskStateEnum?)state;
            var tp = (BackgroundTaskTypeEnum?) type;
            var tasks = MasterLocator.BackgroundTaskService.Find(districtId, st, tp, allDistricts, start ?? 0, count ?? 10);
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

        [AuthorizationFilter("SysAdmin")]
        public ActionResult RerunTasks(GuidList taskIds)
        {
            MasterLocator.BackgroundTaskService.RerunTasks(taskIds);
            return Json(true);
        }
    }
}