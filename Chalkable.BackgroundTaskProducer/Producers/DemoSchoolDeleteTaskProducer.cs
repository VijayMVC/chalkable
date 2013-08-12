using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class DemoSchoolDeleteTaskProducer : ITaskProducer
    {
        public void Produce()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var toDelete = sl.SchoolService.GetDemoSchoolsToDelete();
            foreach (var school in toDelete)
            {
                sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.DeleteSchool, DateTime.UtcNow, school.Id, school.Id.ToString());
            }
        }
    }
}