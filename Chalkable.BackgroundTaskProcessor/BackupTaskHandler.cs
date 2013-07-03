using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
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
        private const int TIMEOUT = 10000;
        private const int SCHOOLS_PER_THREAD = 20;
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
                var t = new BackupRestoreTask
                    {
                        Database = c.Database,
                        Server = c.DataSource,
                        Time = data.Time,
                        Success = false,
                        Completed = false
                    };
                if (backup)
                    RunBackup(new List<BackupRestoreTask> {t});
                else
                    RunRestore(new List<BackupRestoreTask> { t });
                if (!t.Success)
                {
                    log.LogError(string.Format("Db {0} error: Master Database", actionName));
                    if (t.ErrorMessage != null)
                        log.LogError(t.ErrorMessage);
                    return false;
                }
            }

            var schools = sl.SchoolService.GetSchools(false);
            var allTasks = new List<BackupRestoreTask>();
            var threadTasks = new List<BackupRestoreTask>();
            var threads = new List<Thread>();
            foreach (var school in schools)
            {
                var t = new BackupRestoreTask
                    {
                        Completed = false,
                        Database = school.Id.ToString(),
                        Server = school.ServerUrl,
                        Success = false,
                        Time = data.Time
                    };
                allTasks.Add(t);
                threadTasks.Add(t);
                if (threadTasks.Count == SCHOOLS_PER_THREAD)
                {
                    var thread = backup ? new Thread(RunBackup) : new Thread(RunRestore);
                    threads.Add(thread);
                    thread.Start(threadTasks);
                    threadTasks = new List<BackupRestoreTask>();
                }
            }
            if (threadTasks.Count > 0)
            {
                var thread = backup ? new Thread(RunBackup) : new Thread(RunRestore);
                threads.Add(thread);
                thread.Start(threadTasks);
            }


            for (int i = 0; i < threads.Count; i++)
                threads[i].Join();

            bool res = true;
            for (int i = 0; i < allTasks.Count; i++)
            {
                if (!allTasks[i].Success)
                {
                    log.LogError(string.Format("Db {0} error: {1} - {2}", actionName, allTasks[i].Server, allTasks[i].Database));
                    if (allTasks[i].ErrorMessage != null)
                        log.LogError(allTasks[i].ErrorMessage);
                    res = false;
                }
            }
            return res;
        }

        private class BackupRestoreTask
        {
            public long Time { get; set; }
            public string Server { get; set; }
            public string Database { get; set; }
            public string ErrorMessage { get; set; }
            public bool Success { get; set; }
            public bool Completed { get; set; }
            public string TrackingGuid { get; set; }
        }

        private static void RunBackup(object o)
        {
            var tasks = (IList<BackupRestoreTask>) o;
            Run(tasks, (task)=>BackupHelper.DoExport(task.Time, task.Server, task.Database));
        }

        private static void RunRestore(object o)
        {
            var tasks = (IList<BackupRestoreTask>)o;
            Run(tasks, delegate(BackupRestoreTask task)
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
                    return BackupHelper.DoImport(task.Time, task.Server, task.Database);
                });
        }

        private static void Run(IList<BackupRestoreTask> tasks, Func<BackupRestoreTask, string> f)
        {
            
            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Success = false;
                tasks[i].Completed = false;
                tasks[i].ErrorMessage = "Task wasn't started";
            }
            for (int i = 0; i < tasks.Count; i++)
            {
                try
                {
                    tasks[i].TrackingGuid = f(tasks[i]);
                }
                catch (Exception ex)
                {
                    tasks[i].Success = false;
                    tasks[i].ErrorMessage = ex.Message + "\n" + ex.StackTrace;
                    return;
                }
            }

            bool all = false;
            while (!all)
            {
                all = true;
                for (int i = 0; i < tasks.Count; i++)
                    if (!tasks[i].Completed)
                    {
                        var statuses = BackupHelper.CheckRequestStatus(tasks[i].TrackingGuid, tasks[i].Server);
                        if (statuses.Any(x => x.Status == "Failed"))
                        {
                            tasks[i].Completed = true;
                            tasks[i].Success = false;
                            tasks[i].ErrorMessage = statuses.First(x => x.Status == "Failed").ErrorMessage;
                            continue;
                        }
                        if (statuses.Any(x => x.Status == "Completed"))
                        {
                            tasks[i].Completed = true;
                            tasks[i].Success = true;
                            continue;
                        }
                        all = false;
                    }
                Thread.Sleep(TIMEOUT);
            }

        }
    }
}