using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class DemoSchoolDeleteTaskProducer : BaseProducer
    {
        private const string CONFIG_SECTION_NAME = "DemoSchoolDeleteTaskProducer";
        public DemoSchoolDeleteTaskProducer() : base(CONFIG_SECTION_NAME)
        {
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var toDelete = sl.SchoolService.GetDemoSchoolsToDelete();
            foreach (var school in toDelete)
            {
                sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.DeleteSchool, currentTimeUtc, school.Id, school.Id.ToString());
            }
        }
    }
}