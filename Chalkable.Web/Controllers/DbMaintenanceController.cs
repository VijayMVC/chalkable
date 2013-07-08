using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Controllers
{
    public class DbMaintenanceController : ChalkableController
    {
        public ActionResult Backup()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var data = new DatabaseBackupRestoreTaskData(DateTime.UtcNow.Ticks, true);
            sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.BackupDatabases, DateTime.UtcNow, null, data.ToString());
            return Json(true);
        }

        public ActionResult Restore(long time)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var data = new DatabaseBackupRestoreTaskData(time, true);
            sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.RestoreDatabases, DateTime.UtcNow, null, data.ToString());
            return Json(true);
        }



        public ActionResult DatabaseUpdate(string masterSql, string schoolSql)
        {
            var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var sqls = new List<UpdateSql>();
            var mSqls = SplitSql(masterSql, true);
            var sSqls = SplitSql(schoolSql, false);
            sqls.AddRange(mSqls);
            sqls.AddRange(sSqls);
            var data = new DatabaseUpdateTaskData(sqls);
            serviceLocator.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.RestoreDatabases, DateTime.UtcNow, null, data.ToString());
            return Json(true);

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