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
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            bool res = true;
            try
            {
                var data = task.GetData<DatabaseUpdateTaskData>();
                var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
                var districts = sl.DistrictService.GetDistricts();
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
                        foreach (var dbServer in Settings.ChalkableSchoolDbServers)
                        {
                            using (var uow = new UnitOfWork(Settings.GetSchoolTemplateConnectionString(dbServer), false))
                            {
                                var cmd = uow.GetTextCommandWithParams(updateSql.Sql, new Dictionary<string, object>());
                                cmd.ExecuteNonQuery();
                            }
                        }
                        var runer = new AllSchoolRunner<string>();
                        if (!runer.Run(districts, updateSql.Sql, log, ExecSql, task1 => AllSchoolRunner<string>.TaskStatusEnum.Completed))
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
            return res;

        }

        private string ExecSql(AllSchoolRunner<string>.Task task)
        {
            string connectionString = Settings.GetSchoolConnectionString(task.Server, task.Database);
            using (var uow = new UnitOfWork(connectionString, false))
            {
                var cmd = uow.GetTextCommandWithParams(task.Data, new Dictionary<string, object>());
                cmd.ExecuteNonQuery();
            }
            return string.Empty;
        }
    }
}