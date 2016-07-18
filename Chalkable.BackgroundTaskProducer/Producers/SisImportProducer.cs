using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class SisImportProducer : BaseProducer
    {
        public SisImportProducer() : base("SisImportProducer")
        {
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var districts = sl.DistrictService.GetDistrictsSyncStatus();
            foreach (var district in districts)
                if (district.SyncFrequency.HasValue && !district.NewId.HasValue)
                {
                    DateTime? lastTry = district.LastSync;
                    if (district.FailedCompleted.HasValue && (!lastTry.HasValue || district.FailedCompleted > lastTry))
                        lastTry = district.FailedCompleted;
                    if (district.CanceledCompleted.HasValue && (!lastTry.HasValue || district.CanceledCompleted > lastTry))
                        lastTry = district.CanceledCompleted;


                    int time = district.SyncFrequency.Value + district.FailCounter * district.FailDelta;
                    if (district.MaxSyncFrequency.HasValue && district.MaxSyncFrequency.Value < time)
                        time = district.MaxSyncFrequency.Value;

                    if (!lastTry.HasValue || (currentTimeUtc - lastTry.Value).TotalSeconds >= time)
                        sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.SisDataImport, currentTimeUtc, district.Id, string.Empty, district.Id.ToString());
                }
        }
    }
}