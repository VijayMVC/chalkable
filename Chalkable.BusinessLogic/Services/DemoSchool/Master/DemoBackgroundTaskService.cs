using System;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoBackgroundTaskService : MasterServiceBase, IBackgroundTaskService
    {
       

        public DemoBackgroundTaskService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public BackgroundTask ScheduleTask(BackgroundTaskTypeEnum type, DateTime scheduled, Guid? districtRef, string data)
        {
            throw new NotImplementedException();
        }

        public BackgroundTask GetTaskToProcess(DateTime now)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<BackgroundTask> GetTasks(int start, int count)
        {
            throw new NotImplementedException();
        }

        public void Complete(Guid id, bool success)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<BackgroundTask> Find(Guid? districtId, BackgroundTaskStateEnum? state = null, BackgroundTaskTypeEnum? type = null,
            bool allDistricts = false, int start = 0, int count = Int32.MaxValue)
        {
            throw new NotImplementedException();
        }

        public BackgroundTask Find(Guid? schoolId, BackgroundTaskStateEnum state, BackgroundTaskTypeEnum type)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<BackgroundTaskService.BackgroundTaskLogItem> GetLogItems(Guid backgroundTaskId, int start, int count)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public void Cancel(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public void DeleteOlder(Guid districtId, DateTime dateTime)
        {
            throw new NotImplementedException();
        }
    }
}
