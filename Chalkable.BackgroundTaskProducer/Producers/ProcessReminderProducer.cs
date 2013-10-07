using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class ProcessReminderProducer : ITaskProducer
    {
        public void Produce()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var schools = sl.SchoolService.GetSchools(false, false);
            foreach (var school in schools)
            {
                sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.ProcessReminder, DateTime.UtcNow, school.Id, string.Empty);
            }   
        }
    }
}