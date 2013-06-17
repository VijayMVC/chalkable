using System;
using System.Collections.Generic;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IBackgroundTaskService
    {
        BackgroundTask ScheduleTask(BackgroundTaskTypeEnum type, DateTime scheduled, Guid? schoolRef, string data);
        BackgroundTask GetTaskToProcess();
        IList<BackgroundTask> GetTasks(int start, int count);
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
            }
            return task;
        }

        public BackgroundTask GetTaskToProcess()
        {
            using (var uow = Read())
            {
                var da = new BackgroundTaskDataAccess(uow);
                return da.GetTaskForProcessing();
            }
        }

        public IList<BackgroundTask> GetTasks(int start, int count)
        {
            throw new NotImplementedException();
        }
    }
}
