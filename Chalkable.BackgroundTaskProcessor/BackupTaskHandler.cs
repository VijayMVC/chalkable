using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common.Backup;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class BackupTaskHandler : ITaskHandler
    {
        private const int TIMEOUT = 10000;
        private const int SCHOOLS_PER_THREAD = 20;

        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var data = task.GetData<DatabaseBackupTaskData>();

            if (data.BackupMaster)
            {
                var c = new SqlConnection(sl.Context.MasterConnectionString);
                var t = new BackupTask
                    {
                        Database = c.Database,
                        Server = c.DataSource,
                        Time = data.Time,
                        Success = false,
                        Completed = false
                    };
                RunBackup(new List<BackupTask> { t });
                if (!t.Success)
                {
                    log.LogError("Db backup error: Master Database");
                    if (t.ErrorMessage != null)
                        log.LogError(t.ErrorMessage);
                    return false;
                }
            }

            var schools = sl.SchoolService.GetSchools(false);
            var allTasks = new List<BackupTask>();
            var threadTasks = new List<BackupTask>();
            var threads = new List<Thread>();
            foreach (var school in schools)
            {
                var t = new BackupTask
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
                    var thread = new Thread(RunBackup);
                    threads.Add(thread);
                    thread.Start(threadTasks);
                    threadTasks = new List<BackupTask>();
                }
            }
            if (threadTasks.Count > 0)
            {
                var thread = new Thread(RunBackup);
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
                    log.LogError(string.Format("Db backup error: {0} - {1}", allTasks[i].Server, allTasks[i].Database));
                    if (allTasks[i].ErrorMessage != null)
                        log.LogError(allTasks[i].ErrorMessage);
                    res = false;
                }
            }
            return res;
        }

        private class BackupTask
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
            IList<BackupTask> tasks = (IList<BackupTask>) o;
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
                    tasks[i].TrackingGuid = BackupHelper.DoExport(tasks[i].Time, tasks[i].Server, tasks[i].Database);
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