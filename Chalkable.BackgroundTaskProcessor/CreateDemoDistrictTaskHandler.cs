using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class CreateDemoDistrictTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var have = sl.DistrictService.GetDistricts(false, true, false).Count;
            int need = Settings.Configuration.DemoSchoolsReserved;
            var cnt = Math.Max(0, need - have);
            log.LogInfo(string.Format("There are {0} demo schools to create", cnt));
            for (int i = 0; i < cnt; i++)
                sl.DistrictService.CreateDemo();
            return true;
        }
    }
}