using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Common.Backup;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Microsoft.SqlServer.Dac;

namespace Chalkable.Web.Controllers
{
    public class DbMaintenanceController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin")]
        public ActionResult Backup()
        {
            var data = new DatabaseBackupRestoreTaskData(DateTime.UtcNow.Ticks, true);
            MasterLocator.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.BackupDatabases, DateTime.UtcNow, null, data.ToString(), BackgroundTask.GLOBAL_DOMAIN);
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Restore(long time)
        {
            var sl = SchoolLocator.ServiceLocatorMaster;
            var data = new DatabaseBackupRestoreTaskData(time, true);
            sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.RestoreDatabases, DateTime.UtcNow, null, data.ToString(), BackgroundTask.GLOBAL_DOMAIN);
            return Json(true);
        }

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

        [AuthorizationFilter("SysAdmin")]
        public ActionResult DatabaseDeployDacpac(string key)
        {
            var dbDeployOptions = new DacDeployOptions
            {
                BlockOnPossibleDataLoss = true
            };

            var masterDacpac = DacPackage.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Chalkable.Database.Master.dacpac"));
            var schoolDacpac = DacPackage.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Chalkable.Database.School.dacpac"));
            
            var masterDeployScript = DeployDacPackAndGenerateDeployScript(
                masterDacpac, 
                Settings.MasterConnectionString, 
                "ChalkableMaster", 
                dbDeployOptions);

            var serversDeployments = new Dictionary<string, string>();
            foreach (var server in Settings.ChalkableSchoolDbServers)
            {
                try
                {
                    var templateDeployScript = DeployDacPackAndGenerateDeployScript(
                        schoolDacpac,
                        Settings.GetSchoolTemplateConnectionString(server),
                        Settings.SchoolTemplateDbName,
                        dbDeployOptions);
                    serversDeployments.Add(server, templateDeployScript);
                }
                catch (Exception e)
                {
                    serversDeployments.Add(server, e.ToString());
                }
            }

            var schoolDeployScripts = new Dictionary<string, string>();
            foreach (var district in MasterLocator.DistrictService.GetDistricts())
            {
                try
                {
                    var schoolDeployScript = DeployDacPackAndGenerateDeployScript(
                        schoolDacpac,
                        Settings.GetSchoolConnectionString(district.ServerUrl, district.Name),
                        district.Name,
                        dbDeployOptions);

                    schoolDeployScripts.Add(district.Name, schoolDeployScript);
                }
                catch (Exception e)
                {
                    schoolDeployScripts.Add(district.Name, e.ToString());
                }
            }

            return Json(new 
            {
                masterDeployScript,
                serversDeployments,
                schoolDeployScripts
            });

        }

        private static string DeployDacPackAndGenerateDeployScript(DacPackage dp, string conStr, string targetDb,
            DacDeployOptions dbDeployOptions)
        {
            var dbServices = new DacServices(conStr);

            var deployScript = dbServices.GenerateDeployScript(dp, targetDb, dbDeployOptions);
            //var deployReport = dbServices.GenerateDeployReport(dp, targetDb, dbDeployOptions);

            dbServices.Deploy(dp, targetDb, true, dbDeployOptions);

            //var driftReport = dbServices.GenerateDriftReport(targetDb);
            return deployScript;
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