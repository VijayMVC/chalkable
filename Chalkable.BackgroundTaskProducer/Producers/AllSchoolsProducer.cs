using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class AllSchoolsProducer : BaseProducer
    {
        private BackgroundTaskTypeEnum type;
        public AllSchoolsProducer(string configSectionName, BackgroundTaskTypeEnum type)
            : base(configSectionName)
        {
            this.type = type;
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var schools = sl.SchoolService.GetSchools(false, false);
            foreach (var school in schools)
            {
                sl.BackgroundTaskService.ScheduleTask(type, currentTimeUtc, school.Id, string.Empty);
            }
        }
    }
}