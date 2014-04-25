using System;
using System.Collections.Generic;
using Chalkable.BackgroundTaskProcessor.Parallel;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class DatabaseUpdateTaskHandler : ITaskHandler
    {
        private bool BackupAll(DateTime now, BackgroundTaskService.BackgroundTaskLog log)
        {
            var backuper = new BackupTaskHandler(true);
            var time = now.Ticks;
            return backuper.Handle(new BackgroundTask
                {
                    Created = now,
                    Scheduled = now,
                    Type = BackgroundTaskTypeEnum.BackupDatabases,
                    Data = (new DatabaseBackupRestoreTaskData(time, true)).ToString()
                }, log);
        }

        private bool RestoreAll(DateTime now, BackgroundTaskService.BackgroundTaskLog log)
        {
            var restorer = new BackupTaskHandler(false);
            var time = now.Ticks;
            return restorer.Handle(new BackgroundTask
                {
                    Created = now,
                    Scheduled = now,
                    Type = BackgroundTaskTypeEnum.BackupDatabases,
                    Data = (new DatabaseBackupRestoreTaskData(time, true)).ToString()
                }, log);
        }

        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var now = DateTime.UtcNow;
            if (!BackupAll(now, log))
            {
                log.LogError("Database wasn't updated because backup fails");
                return false;
            }
            bool res = true;
            try
            {
                var data = task.GetData<DatabaseUpdateTaskData>();
                var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
                var schools = sl.DistrictService.GetDistricts();
                foreach (var updateSql in data.Sqls)
                {
                    if (updateSql.RunOnMaster)
                    {
                        using (var uow = new UnitOfWork(Settings.MasterConnectionString, false))
                        {
                            var cmd = uow.GetTextCommandWithParams(updateSql.Sql, new Dictionary<string, object>());
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var uow = new UnitOfWork(Settings.SchoolTemplateConnectionString, false))
                        {
                            var cmd = uow.GetTextCommandWithParams(updateSql.Sql, new Dictionary<string, object>());
                            cmd.ExecuteNonQuery();
                        }
                        var runer = new AllSchoolRunner<string>();
                        if (
                            !runer.Run(schools, updateSql.Sql, log, ExecSql,
                                       task1 => AllSchoolRunner<string>.TaskStatusEnum.Completed))
                        {
                            res = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    log.LogError("Error during database update");
                    log.LogError(ex.Message);
                    log.LogError(ex.StackTrace);
                    ex = ex.InnerException;
                }
                res = false;
            }
            if (!res)
            {
                if (!RestoreAll(now, log))
                    log.LogError("Database wasn't restored after update fails");
            }
            return res;

        }

        private string ExecSql(AllSchoolRunner<string>.Task task)
        {
            string connectionString = string.Format(Settings.SchoolConnectionStringTemplate, task.Server,
                                                            task.Database, Settings.Configuration.SchoolDbUser,
                                                            Settings.Configuration.SchoolDbPassword);
            using (var uow = new UnitOfWork(connectionString, false))
            {
                var cmd = uow.GetTextCommandWithParams(task.Data, new Dictionary<string, object>());
                cmd.ExecuteNonQuery();
            }
            return string.Empty;
        }
    }
}