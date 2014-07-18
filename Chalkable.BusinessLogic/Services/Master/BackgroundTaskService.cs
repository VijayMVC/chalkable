using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Microsoft.WindowsAzure.Storage.Table;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IBackgroundTaskService
    {
        BackgroundTask ScheduleTask(BackgroundTaskTypeEnum type, DateTime scheduled, Guid? districtRef, string data);
        BackgroundTask GetTaskToProcess(DateTime now);
        PaginatedList<BackgroundTask> GetTasks(int start, int count);
        void Complete(Guid id, bool success);
        PaginatedList<BackgroundTask> Find(Guid? districtId, BackgroundTaskStateEnum? state = null, BackgroundTaskTypeEnum? type = null, bool allDistricts = false, int start = 0, int count = int.MaxValue);
        PaginatedList<BackgroundTaskService.BackgroundTaskLogItem> GetLogItems(Guid backgroundTaskId, int start, int count);
        void Delete(Guid taskId);
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
            public const int FLUSH_SIZE = 1;

            private List<BackgroundTaskLogItem> items = new List<BackgroundTaskLogItem>();
            private Guid backgroundTaskId;
            public BackgroundTaskLog(Guid backgroundTaskId)
            {
                this.backgroundTaskId = backgroundTaskId;
            }

            public void Log(int level, string message)
            {
                var now = DateTime.UtcNow;
                items.Add(new BackgroundTaskLogItem
                    {
                        BackgroundTaskId = backgroundTaskId,
                        RowKey = now.Ticks + "-" + Guid.NewGuid(),
                        Message = message,
                        Level = level,
                        Time = now
                    });
                if (items.Count >= FLUSH_SIZE)
                    Flush();
            }

            public void LogInfo(string message)
            {
                Log(LEVEL_INFO, message);
            }

            public void LogWarning(string message)
            {
                Log(LEVEL_WARN, message);
            }

            public void LogError(string message)
            {
                Log(LEVEL_ERROR, message);
            }

            public void LogException(Exception ex)
            {
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
                var helper = new TableHelper<BackgroundTaskLogItem>();
                helper.Save(Items);
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

        public BackgroundTask ScheduleTask(BackgroundTaskTypeEnum type, DateTime scheduled, Guid? districtRef, string data)
        {
            DateTime now = DateTime.UtcNow;
            var task = new BackgroundTask
                {
                    Created = now,
                    Data = data,
                    Id = Guid.NewGuid(),
                    Scheduled = scheduled,
                    DistrictRef = districtRef,
                    State = BackgroundTaskStateEnum.Created,
                    Type = type
                };
            using (var uow = Update())
            {
                var da = new BackgroundTaskDataAccess(uow);
                da.Insert(task);
                uow.Commit();
            }
            return task;
        }

        public BackgroundTask GetTaskToProcess(DateTime now)
        {
            using (var uow = Read())
            {
                var da = new BackgroundTaskDataAccess(uow);
                return da.GetTaskForProcessing(now);
            }
        }

        public PaginatedList<BackgroundTask> GetTasks(int start, int count)
        {
            using (var uow = Read())
            {
                var da = new BackgroundTaskDataAccess(uow);
                return da.GetTasks(start, count);
            }
        }

        public void Complete(Guid id, bool success)
        {
            using (var uow = Update())
            {
                var da = new BackgroundTaskDataAccess(uow);
                da.Complete(id, success);
                uow.Commit();
            }
        }

        public PaginatedList<BackgroundTask> Find(Guid? districtId, BackgroundTaskStateEnum? state, BackgroundTaskTypeEnum? type, bool allDistricts = false, int start = 0, int count = int.MaxValue)
        {
            using (var uow = Read())
            {
                var da = new BackgroundTaskDataAccess(uow);
                return da.Find(districtId, state, type, allDistricts, start, count);
            }
        }

        public PaginatedList<BackgroundTaskLogItem> GetLogItems(Guid backgroundTaskId, int start, int count)
        {
            var helper = new TableHelper<BackgroundTaskLogItem>();
            var items = helper.GetByPartKey(backgroundTaskId.ToString(), start, count);
            return items;
        }

        public void Delete(Guid taskId)
        {
            using (var uow = Update())
            {
                var da = new BackgroundTaskDataAccess(uow);
                da.Delete(taskId);
                uow.Commit();
            }
        }
    }
}
