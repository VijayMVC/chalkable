using System;
using System.Collections.Generic;
using System.Linq;
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
                var masterSqls = data.Sqls.Where(x => x.RunOnMaster).ToList();
                var districtSqls = data.Sqls.Where(x => !x.RunOnMaster).ToList();

                using (var uow = new UnitOfWork(Settings.MasterConnectionString, true))
                {
                    foreach (var updateSql in masterSqls)
                    {
                        var cmd = uow.GetTextCommandWithParams(updateSql.Sql, new Dictionary<string, object>());
                        cmd.CommandTimeout = Settings.DbUpdateTimeout;
                        cmd.ExecuteNonQuery();
                    }
                    uow.Commit();
                }


                foreach (var dbServer in Settings.ChalkableSchoolDbServers)
                {
                    using (var uow = new UnitOfWork(Settings.GetSchoolTemplateConnectionString(dbServer), true))
                    {
                        foreach (var updateSql in districtSqls)
                        {
                            var cmd = uow.GetTextCommandWithParams(updateSql.Sql, new Dictionary<string, object>());
                            cmd.CommandTimeout = Settings.DbUpdateTimeout;
                            cmd.ExecuteNonQuery();
                        }
                        uow.Commit();
                    }
                }
                var runer = new AllSchoolRunner<IEnumerable<UpdateSql>>();
                if (!runer.Run(districts, districtSqls, log, ExecSql, task1 => AllSchoolRunner<IEnumerable<UpdateSql>>.TaskStatusEnum.Completed))
                {
                    res = false;
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

        private string ExecSql(AllSchoolRunner<IEnumerable<UpdateSql>>.Task task)
        {
            string connectionString = Settings.GetSchoolConnectionString(task.Server, task.Database);
            using (var uow = new UnitOfWork(connectionString, true))
            {
                foreach (var updateSql in task.Data)
                {
                    var cmd = uow.GetTextCommandWithParams(updateSql.Sql, new Dictionary<string, object>());
                    cmd.CommandTimeout = Settings.DbUpdateTimeout;
                    cmd.ExecuteNonQuery();
                }
                uow.Commit();
            }
            return string.Empty;
        }
    }
}