using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class CreateDemoSchoolTaskProducer : ITaskProducer
    {
        public void Produce()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var existing = sl.BackgroundTaskService.Find(null, BackgroundTaskStateEnum.Created, BackgroundTaskTypeEnum.CreateDemoSchool);
            if (existing == null)
            {
                sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.CreateDemoSchool, DateTime.UtcNow, null, string.Empty);
            }
        }
    }
}