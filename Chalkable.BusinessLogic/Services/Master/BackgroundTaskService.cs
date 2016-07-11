using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Microsoft.WindowsAzure.Storage.Table;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IBackgroundTaskService
    {
        BackgroundTask ScheduleTask(BackgroundTaskTypeEnum type, DateTime scheduled, Guid? districtRef, string data, string domain);
        BackgroundTask GetTaskToProcess(DateTime now);
        PaginatedList<BackgroundTask> GetTasks(int start, int count);
        void Complete(Guid id, bool success);
        PaginatedList<BackgroundTask> Find(Guid? districtId, BackgroundTaskStateEnum? state = null, BackgroundTaskTypeEnum? type = null, bool allDistricts = false, int start = 0, int count = int.MaxValue);
        PaginatedList<BackgroundTaskService.BackgroundTaskLogItem> GetLogItems(Guid backgroundTaskId, int start, int count);
        void Delete(Guid taskId);
        void Cancel(Guid taskId);
        void DeleteOlder(Guid? districtId, DateTime dateTime);
        void RerunTasks(IList<Guid> taskIds);
        BackgroundTask GetById(Guid id);
    }
    
    public class BackgroundTaskService : MasterServiceBase, IBackgroundTaskService
    {
        public class BackgroundTaskLogItem : TableEntity
        {
            
            public string Message { get; set; }
            public int Level { get; set; }
            public DateTime Time { get; set; }

            public Guid BackgroundTaskId
            {
                get { return Guid.Parse(PartitionKey); }
                set { PartitionKey = value.ToString(); }
            }
        }

        public class BackgroundTaskLog : IDisposable
        {
            public const int LEVEL_INFO = 0;
            public const int LEVEL_WARN = 1;
            public const int LEVEL_ERROR = 2;
            private int flushSize;

            private List<BackgroundTaskLogItem> items = new List<BackgroundTaskLogItem>();
            private Guid backgroundTaskId;
            public BackgroundTaskLog(Guid backgroundTaskId, int flushSize)
            {
                this.backgroundTaskId = backgroundTaskId;
                this.flushSize = flushSize;
            }

            private const int MAX_MSG_LEN = (1 << 16) - 1;
            public void Log(int level, string message)
            {
                LogWithoutFlush(level, message);
                if (items.Count >= flushSize)
                    Flush();
            }

            public void Log(int level, IEnumerable<string> messages)
            {
                foreach (var message in messages)
                {
                    LogWithoutFlush(level, message);
                }
                if (items.Count >= flushSize)
                    Flush();
            }

            private void LogWithoutFlush(int level, string message)
            {
                if (message == null)
                    message = "[null]";
                if (message.Length > MAX_MSG_LEN)
                {
                    message = message.Substring(0, MAX_MSG_LEN);
                    Trace.TraceWarning("Log message was truncated");
                }
                var now = DateTime.UtcNow;
                items.Add(new BackgroundTaskLogItem
                {
                    BackgroundTaskId = backgroundTaskId,
                    RowKey = now.Ticks + "-" + Guid.NewGuid(),
                    Message = message,
                    Level = level,
                    Time = now
                });
            }

            public void LogInfo(string message)
            {                
                Log(LEVEL_INFO, message);
            }

            public void LogInfo(IEnumerable<string> messages)
            {
                Log(LEVEL_INFO, messages);
            }

            public void LogWarning(string message)
            {
                Log(LEVEL_WARN, message);
            }

            public void LogError(string message)
            {                
                Log(LEVEL_ERROR, message);
            }

            public void LogException(Exception ex, string districtId = null, string taskId = null)
            {
                Chalkable.Common.Telemetry.TrackException(ex, districtId, taskId);
                while (ex != null)
                {
                    LogError(ex.Message);
                    LogError(ex.StackTrace);
                    ex = ex.InnerException;
                }
            }

            public List<BackgroundTaskLogItem> Items { get { return items; } }

            public void Flush()
            {
                try
                {
                    var helper = new TableHelper<BackgroundTaskLogItem>();
                    helper.Save(Items);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Exception durring the logging process, All messages in buffer are lost");
                    Trace.TraceError(ex.Message);
                    Trace.TraceError(ex.StackTrace);
                }
                items.Clear();
            }

            public void Dispose()
            {
                Flush();
            }
        }
        

        public BackgroundTaskService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public BackgroundTask ScheduleTask(BackgroundTaskTypeEnum type, DateTime scheduled, Guid? districtRef, string data, string domain)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            var task = new BackgroundTask
                {
                    Created = DateTime.UtcNow,
                    Data = data,
                    Id = Guid.NewGuid(),
                    Scheduled = scheduled,
                    DistrictRef = districtRef,
                    State = BackgroundTaskStateEnum.Created,
                    Type = type,
                    Domain = domain
                };
            DoUpdate(u => new BackgroundTaskDataAccess(u).Insert(task));
            return task;
        }

        public BackgroundTask GetTaskToProcess(DateTime now)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            return DoRead(u => new BackgroundTaskDataAccess(u).GetTaskForProcessing(now));
        }

        public PaginatedList<BackgroundTask> GetTasks(int start, int count)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            return DoRead(u => new BackgroundTaskDataAccess(u).GetTasks(start, count));
        }

        public void Complete(Guid id, bool success)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new BackgroundTaskDataAccess(u).Complete(id, success));
        }

        public PaginatedList<BackgroundTask> Find(Guid? districtId, BackgroundTaskStateEnum? state, BackgroundTaskTypeEnum? type, bool allDistricts = false, int start = 0, int count = int.MaxValue)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            return DoRead(u => new BackgroundTaskDataAccess(u).Find(districtId, state, type, allDistricts, start, count));
        }

        public PaginatedList<BackgroundTaskLogItem> GetLogItems(Guid backgroundTaskId, int start, int count)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            var helper = new TableHelper<BackgroundTaskLogItem>();
            var items = helper.GetByPartKey(backgroundTaskId.ToString(), start, count);
            return items;
        }

        public void Delete(Guid taskId)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new BackgroundTaskDataAccess(u).Delete(taskId));
        }

        public void Cancel(Guid taskId)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new BackgroundTaskDataAccess(u).Cancel(taskId));
        }

        public void DeleteOlder(Guid? districtId, DateTime dateTime)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new BackgroundTaskDataAccess(u).DeleteOlder(districtId, dateTime));
        }

        public void RerunTasks(IList<Guid> taskIds)
        {
            if(taskIds.Count == 0) return;
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new BackgroundTaskDataAccess(u).RerunTasks(taskIds));
        }

        public BackgroundTask GetById(Guid id)
        {
            return DoRead(u => new BackgroundTaskDataAccess(u).GetById(id));
        }
    }
}
