using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Common.Backup;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Tools;
using Microsoft.SqlServer.Dac;

namespace Chalkable.Web.Controllers
{
    public class DbMaintenanceController : ChalkableController
    {
        [HttpPost]
        [AuthorizationFilter("SysAdmin")]
        public ActionResult Backup()
        {
            var data = new DatabaseBackupRestoreTaskData(DateTime.UtcNow.Ticks, true);
            MasterLocator.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.BackupDatabases, DateTime.UtcNow, null, data.ToString(), BackgroundTask.GLOBAL_DOMAIN);
            return Json(true);
        }

        [HttpPost]
        [AuthorizationFilter("SysAdmin")]
        public ActionResult Restore(long time)
        {
            var sl = SchoolLocator.ServiceLocatorMaster;
            var data = new DatabaseBackupRestoreTaskData(time, true);
            sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.RestoreDatabases, DateTime.UtcNow, null, data.ToString(), BackgroundTask.GLOBAL_DOMAIN);
            return Json(true);
        }

        [HttpPost]
        [AuthorizationFilter("SysAdmin")]
        public ActionResult DatabaseUpdate(string masterSql, string schoolSql)
        {
            var sqls = new List<UpdateSql>();
            var mSqls = SplitSql(masterSql, true);
            var sSqls = SplitSql(schoolSql, false);
            sqls.AddRange(mSqls);
            sqls.AddRange(sSqls);
            var data = new DatabaseUpdateTaskData(sqls);
            MasterLocator.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.DatabaseUpdate, DateTime.UtcNow, null, data.ToString(), BackgroundTask.GLOBAL_DOMAIN);
            return Json(true);

        }

        [HttpPost]
        [AuthorizationFilter("SysAdmin")]
        public ActionResult DatabaseDeploy(string key)
        {
            var dacPacContainer = ConfigurationManager.AppSettings["DatabaseDacPacContainer"];
            var dacPacName = CompilerHelper.IsProduction ? CompilerHelper.Version : "0-7-84da44f664c1-2270";
            var dacPacMasterUri = dacPacContainer + dacPacName + "/Chalkable.Database.Master.dacpac";
            var dacPacSchoolUri = dacPacContainer + dacPacName + "/Chalkable.Database.School.dacpac";

            var data = new DatabaseDacPacUpdateTaskData()
            {
                ServerName = ConfigurationManager.AppSettings["AzureSqlJobs:ServerName"],
                DatabaseName = ConfigurationManager.AppSettings["AzureSqlJobs:DatabaseName"],
                Username = ConfigurationManager.AppSettings["AzureSqlJobs:Username"],
                Password = ConfigurationManager.AppSettings["AzureSqlJobs:Passwd"],
                DacPacName = dacPacName,
                MasterDacPacUri = dacPacMasterUri,
                SchoolDacPacUri = dacPacSchoolUri
            };

            MasterLocator.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.DatabaseDacPacUpdate, DateTime.UtcNow, null, data.ToString(), BackgroundTask.GLOBAL_DOMAIN);

            return Json(true);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult ListBackups(int start, int count)
        {
            var delim = new []{"/" + BackupHelper.BACKUP_CONTAINER + "/"};
            var names = MasterLocator.StorageBlobService.GetBlobNames(BackupHelper.BACKUP_CONTAINER).Select(x=>x.Uri.ToString()).ToList();
            names = names.Select(x => x.Split(delim, StringSplitOptions.None)[1]).ToList();
            long l;
            names = names.Where(x => long.TryParse(x.Substring(0, 18), out l)).ToList();
            var ticks = names.Select(x=>x.Substring(0, 18)).Distinct();
            var res = ticks.ToDictionary(x => x, x => new BackupViewData { Ticks = x, Created = new DateTime(long.Parse(x)) });
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