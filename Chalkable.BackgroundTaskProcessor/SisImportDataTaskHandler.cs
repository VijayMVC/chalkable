using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.StiImport.Services;

namespace Chalkable.BackgroundTaskProcessor
{
    public abstract class SisImportDataTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            if (!task.DistrictRef.HasValue)
                throw new ChalkableException("No district id for district task");
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            if (!sl.DistrictService.IsOnline(task.DistrictRef.Value))
            {
                log.LogError(string.Format("district {0} is not online yet", task.DistrictRef.Value));
                return false;
            }

            var districtId = task.DistrictRef.Value;
            
            var district = sl.DistrictService.GetByIdOrNull(districtId);
            HandleInternal(task, district, log);
            return true;
        }
        public abstract void HandleInternal(BackgroundTask task, District district, BackgroundTaskService.BackgroundTaskLog log);
    }

    public class SisRegularSyncTaskHandler : SisImportDataTaskHandler
    {
        public override void HandleInternal(BackgroundTask task, District district, BackgroundTaskService.BackgroundTaskLog log)
        {
            var info = new SisConnectionInfo
            {
                SisPassword = district.SisPassword,
                SisUrl = district.SisUrl,
                SisUserName = district.SisUserName
            };
            var importService = new ImportService(district.Id, info, log, task.Id);
            importService.Import();
        }
    }
}