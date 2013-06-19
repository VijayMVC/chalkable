using System;
using Chalkable.BackgroundTaskProducer.Producers;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class EmptySchoolTaskProducer : ITaskProducer
    {
        public void Produce()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var existing = sl.BackgroundTaskService.Find(null, BackgroundTaskStateEnum.Created, BackgroundTaskTypeEnum.CreateEmptySchool);
            if (existing == null)
                sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.CreateEmptySchool, DateTime.UtcNow, null, string.Empty);
        }
    }
}