using System;
using System.Linq;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class CleanupManager : BaseProducer
    {
        private const int NO_DISTRICT_TASK_HISTORY = -30;
        private const int NO_DISTRICT_TASK_MAX_TIME = 12 * 3600;

        public CleanupManager(string configSectionName) : base(configSectionName)
        {
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var districts = sl.DistrictService.GetDistricts().ToDictionary(x=>x.Id);

            var processingTasks = sl.BackgroundTaskService.Find(null, BackgroundTaskStateEnum.Processing, null, true);
            var now = DateTime.UtcNow;
            foreach (var processingTask in processingTasks)
                if (processingTask.Started.HasValue)
                {
                    if (processingTask.DistrictRef.HasValue)
                    {
                        var d = districts[processingTask.DistrictRef.Value];
                        if ((now - processingTask.Started.Value).TotalSeconds > d.MaxSyncTime && d.LastSync.HasValue)
                            sl.BackgroundTaskService.Cancel(processingTask.Id);
                    }
                    else
                    {
                        if ((now - processingTask.Started.Value).TotalSeconds > NO_DISTRICT_TASK_MAX_TIME)
                            sl.BackgroundTaskService.Cancel(processingTask.Id);
                    }
                }
                
            foreach (var district in districts)
            {
                sl.BackgroundTaskService.DeleteOlder(district.Key, now.AddDays(-district.Value.SyncHistoryDays));
            }
            sl.BackgroundTaskService.DeleteOlder(null, now.AddDays(NO_DISTRICT_TASK_HISTORY));
        }
    }
}