using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
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
                var have = sl.SchoolService.GetSchools(false, true).Count;
                int need = Settings.Configuration.DemoSchoolsReserved;
                int cnt = Math.Max(0, need - have);
                if (cnt > 0)
                    sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.CreateDemoSchool, DateTime.UtcNow, null, string.Empty);
            }
        }
    }
}