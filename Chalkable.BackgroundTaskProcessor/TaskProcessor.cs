using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class TaskProcessor
    {
        private Dictionary<BackgroundTaskTypeEnum, ITaskHandler> handlers = new Dictionary<BackgroundTaskTypeEnum, ITaskHandler>();

        public TaskProcessor()
        {
            handlers.Add(BackgroundTaskTypeEnum.CreateEmptySchool, new CreateEmptySchoolTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.SisDataImport, new SisImportDataTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.BackupDatabases, new BackupTaskHandler(true));
            handlers.Add(BackgroundTaskTypeEnum.RestoreDatabases, new BackupTaskHandler(false));
            handlers.Add(BackgroundTaskTypeEnum.DatabaseUpdate, new DatabaseUpdateTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.CreateDemoSchool, new CreateDemoSchoolTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.DeleteSchool, new DeleteSchoolTaskHandler());
        }

        public void Process()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var task = sl.BackgroundTaskService.GetTaskToProcess(DateTime.UtcNow);
            if (task == null)
                return;
            using (var log = new BackgroundTaskService.BackgroundTaskLog(task.Id))
            {
                if (!handlers.ContainsKey(task.Type))
                {
                    Trace.TraceError(string.Format("No task handler for task type {0}", task.Type));
                }
                try
                {
                    var res = handlers[task.Type].Handle(task, log);
                    if (res)
                        log.LogInfo(string.Format("Task {0} processing succesfully completed", task.Id));
                    else
                        log.LogError(string.Format("Task {0} processing failed", task.Id));
                    sl.BackgroundTaskService.Complete(task.Id, res);
                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        Trace.TraceError(ex.Message);
                        Trace.TraceError(ex.StackTrace);
                        log.LogError(ex.Message);
                        log.LogError(ex.StackTrace);
                        ex = ex.InnerException;
                    }
                    sl.BackgroundTaskService.Complete(task.Id, false);
                }    
            }
        }
    }
}