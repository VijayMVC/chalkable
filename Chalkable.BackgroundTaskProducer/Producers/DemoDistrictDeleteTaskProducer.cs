using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class DemoDistrictDeleteTaskProducer : BaseProducer
    {
        private const string CONFIG_SECTION_NAME = "DemoDistrictDeleteTaskProducer";
        public DemoDistrictDeleteTaskProducer() : base(CONFIG_SECTION_NAME)
        {
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var toDelete = sl.DistrictService.GetDemoDistrictsToDelete();
            foreach (var district in toDelete)
            {
                sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.DeleteDistrict, currentTimeUtc, district.Id, district.Id.ToString());
            }
        }
    }
}