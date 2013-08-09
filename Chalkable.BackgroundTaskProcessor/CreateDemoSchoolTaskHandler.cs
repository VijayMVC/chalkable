using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class CreateDemoSchoolTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var have = sl.SchoolService.GetSchools(false, true).Count;
            int need = Settings.Configuration.DemoSchoolsReserved;
            int cnt = Math.Max(0, need - have);
            log.LogInfo(string.Format("There are {0} demo schools to create", cnt));
            for (int i = 0; i < cnt; i++)
                sl.SchoolService.CreateDemo();
            return true;
        }
    }
}