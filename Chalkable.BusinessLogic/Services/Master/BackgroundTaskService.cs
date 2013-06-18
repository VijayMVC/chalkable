﻿using System;
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
        BackgroundTask ScheduleTask(BackgroundTaskTypeEnum type, DateTime scheduled, Guid? schoolRef, string data);
        BackgroundTask GetTaskToProcess(DateTime now);
        PaginatedList<BackgroundTask> GetTasks(int start, int count);
        void Complete(Guid id, bool success);
    }
    
    public class BackgroundTaskService : MasterServiceBase, IBackgroundTaskService
    {
        public class BackgroundTaskLogItem : TableEntity
        {
            
            public string Message { get; set; }
            public int Level { get; set; }
            public DateTime Time { get; set; }
            public Guid Id 
            {
                get { return Guid.Parse(RowKey); }
                set { RowKey = value.ToString(); }
            }

            public Guid BackgroundTaskId
            {
                get { return Guid.Parse(PartitionKey); }
                set { PartitionKey = value.ToString(); }
            }
        }

        public class BackgroundTaskLog
        {
            public const int LEVEL_INFO = 0;
            public const int LEVEL_WARN = 1;
            public const int LEVEL_ERROR = 2;

            private List<BackgroundTaskLogItem> items = new List<BackgroundTaskLogItem>();
            private Guid backgroundTaskId;
            public BackgroundTaskLog(Guid backgroundTaskId)
            {
                this.backgroundTaskId = backgroundTaskId;
            }

            public void Log(int level, string message)
            {
                items.Add(new BackgroundTaskLogItem
                    {
                        BackgroundTaskId = backgroundTaskId,
                        Id = Guid.NewGuid(),
                        Message = message,
                        Level = level,
                        Time = DateTime.Now
                    });
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

            public List<BackgroundTaskLogItem> Items { get { return items; } }
        }
        

        public BackgroundTaskService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public BackgroundTask ScheduleTask(BackgroundTaskTypeEnum type, DateTime scheduled, Guid? schoolRef, string data)
        {
            DateTime now = DateTime.Now;
            var task = new BackgroundTask
                {
                    Created = now,
                    Data = data,
                    Id = Guid.NewGuid(),
                    Scheduled = scheduled,
                    SchoolRef = schoolRef,
                    State = BackgroundTaskStateEnum.Created,
                    Type = type
                };
            using (var uow = Update())
            {
                var da = new BackgroundTaskDataAccess(uow);
                da.Create(task);
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

        public PaginatedList<BackgroundTaskLogItem> GetLogItems(Guid backgroundTaskId, int start, int count)
        {
            var helper = new TableHelper<BackgroundTaskLogItem>();
            var items = helper.GetByPartKey(backgroundTaskId.ToString(), start, count);
            return items;
        }

        public void SaveLog(BackgroundTaskLog log)
        {
            var helper = new TableHelper<BackgroundTaskLogItem>();
            helper.Save(log.Items);
        }
    }
}
