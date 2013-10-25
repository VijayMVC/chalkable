using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class AllDistrictsProducer : BaseProducer
    {
        private BackgroundTaskTypeEnum type;
        public AllDistrictsProducer(string configSectionName, BackgroundTaskTypeEnum type)
            : base(configSectionName)
        {
            this.type = type;
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var districts = sl.DistrictService.GetDistricts(false, false, null);
            foreach (var district in districts)
            {
                sl.BackgroundTaskService.ScheduleTask(type, currentTimeUtc, district.Id, string.Empty);
            }
        }
    }
}