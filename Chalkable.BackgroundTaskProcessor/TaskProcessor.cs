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
            handlers.Add(BackgroundTaskTypeEnum.SisDataImport, new SisImportDataTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.BackupDatabases, new BackupTaskHandler(true));
            handlers.Add(BackgroundTaskTypeEnum.RestoreDatabases, new BackupTaskHandler(false));
            handlers.Add(BackgroundTaskTypeEnum.DatabaseUpdate, new DatabaseUpdateTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.DeleteDistrict, new DeleteSchoolTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.AttendanceNotification, new AttendanceNotificationTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.TeacherAttendanceNotification, new TeacherAttendanceNotificationTaskHandler());
        }

        public void Process()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var task = sl.BackgroundTaskService.GetTaskToProcess(DateTime.UtcNow);
            if (task == null)
                return;
            int logFlushSize = 100;
            if (task.DistrictRef.HasValue)
            {
                if (task.DistrictRef.Value == Guid.Parse("07abdc8f-2dfc-4bbe-960b-395f6feae8c7"))
                {
                    int a = 0;
                }
                var d = sl.DistrictService.GetByIdOrNull(task.DistrictRef.Value);
                logFlushSize = d.SyncLogFlushSize;
            }
            using (var log = new BackgroundTaskService.BackgroundTaskLog(task.Id, logFlushSize))
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
                    log.LogException(ex);
                    sl.BackgroundTaskService.Complete(task.Id, false);
                }    
            }
        }
    }
}