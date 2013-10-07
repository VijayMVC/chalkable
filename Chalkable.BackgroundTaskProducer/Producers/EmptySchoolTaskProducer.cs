using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class EmptySchoolTaskProducer : BaseProducer
    {
        private const string CONFIG_SECTION_NAME = "EmptySchoolTaskProducer";
        public EmptySchoolTaskProducer() : base(CONFIG_SECTION_NAME)
        {
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var existing = sl.BackgroundTaskService.Find(null, BackgroundTaskStateEnum.Created, BackgroundTaskTypeEnum.CreateEmptySchool);
            if (existing == null)
            {
                var have = sl.SchoolService.GetSchools(true, false).Count;
                int need = Settings.Configuration.EmptySchoolsReserved;
                int cnt = Math.Max(0, need - have);
                if (cnt > 0)
                    sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.CreateEmptySchool, currentTimeUtc, null, string.Empty);
            }
                
        }
    }
}