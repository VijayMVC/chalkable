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
            
            
            /*var schoolId = data.SchoolId;
            var school = sl.SchoolService.GetById(schoolId);
            
            var district = sl.DistrictService.GetByIdOrNull(school.DistrictRef);
            
            var connectionInfo = new SisConnectionInfo
                {
                    DbName = district.DbName,
                    SisPassword = district.SisPassword,
                    SisUrl = district.SisUrl,
                    SisUserName = district.SisUserName
                };

            var importService = new ImportService(schoolId, data.SisSchoolId, data.SchoolYearIds, connectionInfo, log);
            importService.ImportPeople(null);
            importService.ImportSchedule(null);*/
            return true;
        }
    }
}