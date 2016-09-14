using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BackgroundTaskProcessor.DatabaseDacPacUpdate;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class TaskProcessor
    {
        private Dictionary<BackgroundTaskTypeEnum, ITaskHandler> handlers = new Dictionary<BackgroundTaskTypeEnum, ITaskHandler>();

        public TaskProcessor()
        {
            handlers.Add(BackgroundTaskTypeEnum.SisDataImport, new SisRegularSyncTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.BackupDatabases, new BackupTaskHandler(true));
            handlers.Add(BackgroundTaskTypeEnum.RestoreDatabases, new BackupTaskHandler(false));
            handlers.Add(BackgroundTaskTypeEnum.DatabaseUpdate, new DatabaseUpdateTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.AttendanceNotification, new AttendanceNotificationTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.TeacherAttendanceNotification, new TeacherAttendanceNotificationTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.PictureImport, new PictureImportTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.SisDataResync, new SisResyncTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.SisDataResyncAfterRestore, new SisResyncAfterRestoreTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.DatabaseDacPacUpdate, new DatabaseDacPacUpdateTaskHandler());
            handlers.Add(BackgroundTaskTypeEnum.AcademicBenchmarkImport, new AcademicBenchmarkImportTaskHandler());
        }

        private BackgroundTaskService.BackgroundTaskLog CreateLog(IServiceLocatorMaster sl, BackgroundTask task)
        {
            int logFlushSize = 100;
            if (task.DistrictRef.HasValue)
            {
                var d = sl.DistrictService.GetByIdOrNull(task.DistrictRef.Value);
                logFlushSize = d.LastSync.HasValue ? d.SyncLogFlushSize : 1;
            }
            else
            {
                if (task.Type == BackgroundTaskTypeEnum.DatabaseDacPacUpdate)
                    logFlushSize = 1;
            }
            return new BackgroundTaskService.BackgroundTaskLog(task.Id, logFlushSize);
        }

        public void Process()
        {

            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var task = sl.BackgroundTaskService.GetTaskToProcess(DateTime.UtcNow);
            if (task == null)
                return;

            var requestTimer = Stopwatch.StartNew();            
            var requestStartTime = DateTimeOffset.UtcNow;
            var requestName = "ProcessTask";
            
            using (var log = CreateLog(sl, task))
            {
                if (!handlers.ContainsKey(task.Type))
                {
                    Trace.TraceError($"No task handler for task type {task.Type}");
                }
                try
                {
                    var res = handlers[task.Type].Handle(task, log);
                    if (res) {
                        Telemetry.DispatchRequest(requestName, task.Type.ToString(), requestStartTime, requestTimer.Elapsed, true, Verbosity.Info, task.DistrictRef.ToString(), task.Id.ToString());
                        log.LogInfo($"Task {task.Id} processing succesfully completed");
                    }
                    else {
                        Telemetry.DispatchRequest(requestName, task.Type.ToString(), requestStartTime, requestTimer.Elapsed, false, Verbosity.Error, task.DistrictRef.ToString(), task.Id.ToString());
                        log.LogError($"Task {task.Id} processing failed");
                    }
                    sl.BackgroundTaskService.Complete(task.Id, res);
                }
                catch (Exception ex)
                {                    
                    log.LogException(ex, task.DistrictRef.ToString(), task.Id.ToString());
                    sl.BackgroundTaskService.Complete(task.Id, false);
                }    
            }
        }
    }
}