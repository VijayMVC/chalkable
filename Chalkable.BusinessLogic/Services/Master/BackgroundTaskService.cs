using System;
using Chalkable.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

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
    }
}
