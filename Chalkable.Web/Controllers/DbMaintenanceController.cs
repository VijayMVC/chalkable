using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
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
            var delim = new []{"/" + BackupHelper.BACKUP_CONTAINER + "/"};
            var names = MasterLocator.StorageBlobService.GetBlobNames(BackupHelper.BACKUP_CONTAINER).Select(x=>x.Uri.ToString()).ToList();
            names = names.Select(x => x.Split(delim, StringSplitOptions.None)[1]).ToList();
            long l;
            names = names.Where(x => long.TryParse(x.Substring(0, 18), out l)).ToList();
            var ticks = names.Select(x=>x.Substring(0, 18)).Distinct();
            var res = ticks.ToDictionary(x=>x, x => new BackupViewData { Ticks = long.Parse(x), DateTime = new DateTime(long.Parse(x)) });
            foreach (var name in names)
            {
                var t = name.Substring(0, 18);
                var vd = res[t];
                if (name.Contains("ChalkableMaster"))
                {
                    vd.HasMaster = true;
                }
                else if (name.Contains("ChalkableSchoolTemplate"))
                {
                    vd.HasSchoolTemplate = true;
                }
                else
                    vd.SchoolCount++;

            }
            return Json(new PaginatedList<BackupViewData>(res.Values.OrderBy(x => x.Ticks), start / count, count));
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