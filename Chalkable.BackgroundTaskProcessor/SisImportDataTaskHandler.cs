using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.Services;

namespace Chalkable.BackgroundTaskProcessor
{
    public class SisImportDataTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();

            var districtId = Guid.Parse(task.Data);
            
            var district = sl.DistrictService.GetByIdOrNull(districtId);
            
            var info = new SisConnectionInfo
                {
                    DbName = district.DbName,
                    SisPassword = district.SisPassword,
                    SisUrl = district.SisUrl,
                    SisUserName = district.SisUserName
                };
            var importService = new ImportService(districtId, info, log);
            importService.ImportPeople(null);
            importService.ImportSchedule(null);
            return true;
        }
    }
}