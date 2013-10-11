using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class ProcessReminderProducer : BaseProducer
    {
        private const string CONFIG_SECTION_NAME = "ProcessReminderProducer";
        public ProcessReminderProducer() : base(CONFIG_SECTION_NAME)
        {
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var schools = sl.SchoolService.GetSchools(false, false);
            foreach (var school in schools)
            {
                sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.ProcessReminder, currentTimeUtc, school.Id, string.Empty);
            }   
        }
    }
}