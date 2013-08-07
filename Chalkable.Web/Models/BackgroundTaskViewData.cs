using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class BackgroundTaskViewData
    {
        public Guid Id { get; set; }
        public DateTime? Started { get; set; }
        public DateTime Created { get; set; }
        public DateTime Scheduled { get; set; }
        public DateTime? Completed { get; set; }
        public int TaskState { get; set; }
        public int TaskType { get; set; }
        
        public static BackgroundTaskViewData Create(BackgroundTask task)
        {
            return new BackgroundTaskViewData
            {
                Id = task.Id,
                Created = task.Created,
                Started = task.Started,
                Scheduled = task.Scheduled,
                TaskState = (int)task.State,
                TaskType = (int)task.Type,
                Completed = task.Completed
            };
        }

        public static IList<BackgroundTaskViewData> Create(IList<BackgroundTask> tasks)
        {
            return tasks.Select(Create).ToList();
        } 
    }
}