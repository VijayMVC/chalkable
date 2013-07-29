using Chalkable.BusinessLogic.Logic;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.SisConnector.PublicModel;
using Chalkable.SisImportFacade;

namespace Chalkable.BackgroundTaskProcessor
{
    public class SisImportDataTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var data = task.GetData<SisImportTaskData>();
            var schoolId = data.SchoolId;
            var school = sl.SchoolService.GetById(schoolId);
            var type = school.ImportSystemType;
            if (!school.DistrictRef.HasValue)
                throw new ChalkableException("School is not assigned to district");

            var district = sl.DistrictService.GetById(school.DistrictRef.Value);
            
            var stateMachine = new SchoolStateMachine(schoolId, sl);
            //TODO: school schould be empty
            if (!stateMachine.CanApply(StateActionEnum.SisImportAction))
                throw new InvalidSchoolStatusException(ChlkResources.ERR_CANT_IMPORT_SCHOOL_WITH_CURRENT_STATUS);


            var connectionInfo = new SisConnectionInfo
                {
                    DbName = district.DbName,
                    SisPassword = district.SisPassword,
                    SisUrl = district.SisUrl,
                    SisUserName = district.SisUserName
                };

            var importService = SisImportProvider.CreateImportService(type, schoolId, data.SisSchoolId, data.SchoolYearIds, connectionInfo, log);
            importService.ImportPeople(null);
            importService.ImportSchedule(null);
            importService.ImportAttendances(null);
            stateMachine.Apply(StateActionEnum.SisImportAction);
            return true;
        }
    }
}