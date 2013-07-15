using Chalkable.BusinessLogic.Logic;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.SisImportFacade;

namespace Chalkable.BackgroundTaskProcessor
{
    public class SisImportDataTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var data = task.GetData<SisImportTaskData>();
            var type = sl.SchoolService.GetById(data.SchoolId).ImportSystemType;
            var schoolId = data.SchoolId;
            var stateMachine = new SchoolStateMachine(schoolId, sl);
            //TODO: school schould be empty
            if (!stateMachine.CanApply(StateActionEnum.SisImportAction))
                throw new InvalidSchoolStatusException(ChlkResources.ERR_CANT_IMPORT_SCHOOL_WITH_CURRENT_STATUS);
            var importService = SisImportProvider.CreateImportService(type, schoolId, data.SisSchoolId, data.SchoolYearIds, log);
            importService.ImportPeople(null);
            importService.ImportSchedule(null);
            importService.ImportAttendances(null);
            stateMachine.Apply(StateActionEnum.SisImportAction);
            return true;
        }
    }
}