using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;
using Chalkable.StiImport.Services;

namespace Chalkable.BackgroundTaskProcessor
{
    public class SisResyncTaskHandler : SisImportDataTaskHandler
    {
        public override void HandleInternal(BackgroundTask task, District district, BackgroundTaskService.BackgroundTaskLog log)
        {
            var info = new SisConnectionInfo
            {
                SisPassword = district.SisPassword,
                SisUrl = district.SisUrl,
                SisUserName = district.SisUserName
            };
            var importService = new ImportService(district.Id, info, log);
            importService.Resync(task.Data);
        }
    }

    public class SisResyncAfterRestoreTaskHandler : SisImportDataTaskHandler
    {
        public override void HandleInternal(BackgroundTask task, District district, BackgroundTaskService.BackgroundTaskLog log)
        {
            var info = new SisConnectionInfo
            {
                SisPassword = district.SisPassword,
                SisUrl = district.SisUrl,
                SisUserName = district.SisUserName
            };
            var importService = new ImportService(district.Id, info, log);
            importService.ResyncAfterRestore();
        }
    }
}