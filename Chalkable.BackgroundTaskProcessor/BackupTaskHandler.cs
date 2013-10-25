using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Chalkable.BackgroundTaskProcessor.Parallel;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Backup;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class BackupTaskHandler : ITaskHandler
    {
        private bool backup;
        private string actionName;
        public BackupTaskHandler(bool backup)
        {
            this.backup = backup;
            actionName = backup ? "backup" : "restor";
        }

        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var data = task.GetData<DatabaseBackupRestoreTaskData>();

            if (data.BackupMaster)
            {
                var c = new SqlConnection(sl.Context.MasterConnectionString);
                var t = new AllSchoolRunner<long>.Task
                    {
                        Database = c.Database,
                        Server = c.DataSource,
                        Data = data.Time,
                        Success = false,
                        Completed = false
                    };
                if (backup)
                    AllSchoolRunner<long>.Run(new List<AllSchoolRunner<long>.Task> { t }, x=>BackupHelper.DoExport(x.Data, x.Server, x.Database), CheckStatus);
                else
                    AllSchoolRunner<long>.Run(new List<AllSchoolRunner<long>.Task> { t }, Restore, CheckStatus);
                if (!t.Success)
                {
                    log.LogError(string.Format("Db {0} error: Master Database", actionName));
                    if (t.ErrorMessage != null)
                        log.LogError(t.ErrorMessage);
                    return false;
                }
            }
            {
                var c = new SqlConnection(Settings.SchoolTemplateConnectionString);
                var t = new AllSchoolRunner<long>.Task
                {
                    Database = c.Database,
                    Server = c.DataSource,
                    Data = data.Time,
                    Success = false,
                    Completed = false
                };
                if (backup)
                    AllSchoolRunner<long>.Run(new List<AllSchoolRunner<long>.Task> { t }, x => BackupHelper.DoExport(x.Data, x.Server, x.Database), CheckStatus);
                else
                    AllSchoolRunner<long>.Run(new List<AllSchoolRunner<long>.Task> { t }, Restore, CheckStatus);
                if (!t.Success)
                {
                    log.LogError(string.Format("Db {0} error: Template Database", actionName));
                    if (t.ErrorMessage != null)
                        log.LogError(t.ErrorMessage);
                    return false;
                }
            }

            var schools = sl.DistrictService.GetDistricts(null, null, null);
            var runer = new AllSchoolRunner<long>();
            bool res;
            if (backup)
                res = runer.Run(schools, data.Time, log, t=>BackupHelper.DoExport(t.Data, t.Server, t.Database), CheckStatus);
            else
                res = runer.Run(schools, data.Time, log, Restore, CheckStatus);
            return res;
        }

        private static string Restore(AllSchoolRunner<long>.Task task)
        {
            string connectionString = string.Format(Settings.SchoolConnectionStringTemplate, task.Server,
                                                            "master", Settings.Configuration.SchoolDbUser,
                                                            Settings.Configuration.SchoolDbPassword);
            using (var uow = new UnitOfWork(connectionString, false))
            {
                var cmd = uow.GetTextCommandWithParams(string.Format("drop database [{0}]", task.Database),
                                                new Dictionary<string, object>());
                cmd.ExecuteNonQuery();
            }
            return BackupHelper.DoImport(task.Data, task.Server, task.Database);
        }

        private static AllSchoolRunner<long>.TaskStatusEnum CheckStatus(AllSchoolRunner<long>.Task task)
        {
            var statuses = BackupHelper.CheckRequestStatus(task.TrackingGuid, task.Server);
            if (statuses.Any(x => x.Status == "Failed"))
            {
                task.ErrorMessage = statuses.First(x => x.Status == "Failed").ErrorMessage;
                return AllSchoolRunner<long>.TaskStatusEnum.Failed;
            }
            if (statuses.Any(x => x.Status == "Completed"))
            {
                return AllSchoolRunner<long>.TaskStatusEnum.Completed;
            }
            return AllSchoolRunner<long>.TaskStatusEnum.Running;
        }
    }
}