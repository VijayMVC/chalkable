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
        }

        public void Process()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var task = sl.BackgroundTaskService.GetTaskToProcess(DateTime.UtcNow);
            if (task == null)
                return;
            var log = new BackgroundTaskService.BackgroundTaskLog(task.Id);
            if (!handlers.ContainsKey(task.Type))
            {
                Trace.TraceError(string.Format("No task handler for task type {0}", task.Type));
            }
            try
            {
                bool res = handlers[task.Type].Handle(task, log);
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
            finally
            {
                sl.BackgroundTaskService.SaveLog(log);
            }
        }
    }
}