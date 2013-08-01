using System;
using System.Collections.Generic;
using System.Threading;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor.Parallel
{
    public class AllSchoolRunner<T>
    {
        private const int TIMEOUT = 10000;
        private const int SCHOOLS_PER_THREAD = 20;
        public class Task
        {
            public T Data { get; set; }
            public string Server { get; set; }
            public string Database { get; set; }
            public string ErrorMessage { get; set; }
            public bool Success { get; set; }
            public bool Completed { get; set; }
            public string TrackingGuid { get; set; }
        }

        public enum TaskStatusEnum
        {
            Completed,
            Failed,
            Running
        }

        public bool Run(IList<School> schools, T data, BackgroundTaskService.BackgroundTaskLog log, Func<Task, string> f, Func<Task, TaskStatusEnum> checkStatus)
        {
            var allTasks = new List<Task>();
            var threadTasks = new List<Task>();
            var threads = new List<Thread>();

            var ts = new ParameterizedThreadStart(delegate(object o)
                {
                    var tasks = (IList<Task>)o;
                    Run(tasks, f, checkStatus);
                });

            foreach (var school in schools)
            {
                var t = new Task
                    {
                        Completed = false,
                        Database = school.Id.ToString(),
                        Server = school.ServerUrl,
                        Success = false,
                        Data = data
                    };
                allTasks.Add(t);
                threadTasks.Add(t);
                if (threadTasks.Count == SCHOOLS_PER_THREAD)
                {
                    var thread = new Thread(ts);
                    threads.Add(thread);
                    thread.Start(threadTasks);
                    threadTasks = new List<Task>();
                }
            }
            if (threadTasks.Count > 0)
            {
                var thread = new Thread(ts);
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
                    log.LogError(string.Format("Db error: {0} - {1}", allTasks[i].Server, allTasks[i].Database));
                    if (allTasks[i].ErrorMessage != null)
                        log.LogError(allTasks[i].ErrorMessage);
                    res = false;
                }
            }
            return res;
        }

        public static void Run(IList<Task> tasks, Func<Task, string> f, Func<Task, TaskStatusEnum> checkStatus)
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
                        var stat = checkStatus(tasks[i]);
                        if (stat == TaskStatusEnum.Completed)
                        {
                            tasks[i].Completed = true;
                            tasks[i].Success = true;
                            continue;
                        }
                        if (stat == TaskStatusEnum.Failed)
                        {
                            tasks[i].Completed = true;
                            tasks[i].Success = false;
                            continue;
                        }
                        all = false;
                    }
                Thread.Sleep(TIMEOUT);
            }

        }
    }
}