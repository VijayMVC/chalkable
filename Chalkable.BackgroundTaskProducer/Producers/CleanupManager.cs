using System;
using System.Linq;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class CleanupManager : BaseProducer
    {
        public CleanupManager(string configSectionName) : base(configSectionName)
        {
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var districts = sl.DistrictService.GetDistricts().ToDictionary(x=>x.Id);

            var processingTasks = sl.BackgroundTaskService.Find(null, BackgroundTaskStateEnum.Processing);
            var now = DateTime.Now;
            foreach (var processingTask in processingTasks)
                if (processingTask.DistrictRef.HasValue && processingTask.Started.HasValue)
                {
                    var d = districts[processingTask.DistrictRef.Value];
                    if ((now - processingTask.Started.Value).TotalSeconds > d.MaxSyncTime && d.LastSync.HasValue)
                        sl.BackgroundTaskService.Cancel(processingTask.Id);
                }
            foreach (var district in districts)
            {
                sl.BackgroundTaskService.DeleteOlder(district.Key, now.AddDays(-district.Value.SyncHistoryDays));
            }
        }
    }
}