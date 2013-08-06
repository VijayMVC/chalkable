using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.Data.Common.Backup;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    public class DbMaintenanceController : ChalkableController
    {
        [AuthorizationFilter("System Admin")]
        public ActionResult Backup()
        {
            var data = new DatabaseBackupRestoreTaskData(DateTime.UtcNow.Ticks, true);
            MasterLocator.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.BackupDatabases, DateTime.UtcNow, null, data.ToString());
            return Json(true);
        }

        [AuthorizationFilter("System Admin")]
        public ActionResult Restore(long time)
        {
            var sl = SchoolLocator.ServiceLocatorMaster;
            var data = new DatabaseBackupRestoreTaskData(time, true);
            sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.RestoreDatabases, DateTime.UtcNow, null, data.ToString());
            return Json(true);
        }

        [AuthorizationFilter("System Admin")]
        public ActionResult DatabaseUpdate(string masterSql, string schoolSql)
        {
            var sqls = new List<UpdateSql>();
            var mSqls = SplitSql(masterSql, true);
            var sSqls = SplitSql(schoolSql, false);
            sqls.AddRange(mSqls);
            sqls.AddRange(sSqls);
            var data = new DatabaseUpdateTaskData(sqls);
            MasterLocator.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.DatabaseUpdate, DateTime.UtcNow, null, data.ToString());
            return Json(true);

        }

        [AuthorizationFilter("System Admin")]
        public ActionResult ListBackups(int start, int count)
        {
            var res = MasterLocator.StorageBlobService.GetBlobs(BackupHelper.BACKUP_CONTAINER, null, start, count);
            return Json(res.Transform(BlobViewData.Create));
        }

        private static IEnumerable<UpdateSql> SplitSql(string sql, bool onMaster)
        {
            var sqls = new List<UpdateSql>();
            string[] sl = sql.Split('\n');
            string query = "";
            for (int i = 0; i < sl.Length; i++)
            {
                string s = sl[i].Trim();
                if (s.ToLower() == "go")
                {
                    sqls.Add(new UpdateSql { Sql = query, RunOnMaster = onMaster });
                    query = "";
                }
                else
                    query += s + "\n";
            }
            if (!String.IsNullOrEmpty(query.Trim()))
            {
                sqls.Add(new UpdateSql { Sql = query, RunOnMaster = onMaster });
            }
            return sqls;
        }
    }
}