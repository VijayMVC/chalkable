using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class CreateEmptySchoolTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var have = sl.SchoolService.GetSchools(true).Count;
            int need = Settings.Configuration.EmptySchoolsReserved;
            int cnt = Math.Max(0, need - have);
            log.LogInfo(string.Format("There are {0} schools to create", cnt));
            for (int i = 0; i < cnt; i++)
                sl.SchoolService.CreateEmpty();
            return true;
        }
    }

    public class SisImportDataTaskHandler : ITaskHandler
    {
        

        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var data = task.GetData<SisImportTaskData>();
            var type = sl.SchoolService.GetById(data.SchoolId).ImportSystemType;

            /*try
            {
                


                var stateMachine = new SchoolStateMachine(taskInfo.TypedData.SchoolId, serviceLocator);
                if (!stateMachine.CanApply(StateActionEnum.SisImportAction))
                    throw new Exception(ChlkResources.ERR_CANT_IMPORT_SCHOOL_WITH_CURRENT_STATUS);

                var schoolId = taskInfo.TypedData.SchoolId;

                using (var ce = new ChalkableEntities())
                {
                    ce.CommandTimeout = 600;
                    var admins = ce.SchoolPersons.Where(x => x.SchoolInfoRef == schoolId && x.RoleRef == CoreRoles.ADMIN_GRADE_ROLE.Id).Select(x => x.Id).JoinString(",");
                    ce.DeletePersonalData(schoolId, admins);
                }

                using (var ce = new ChalkableEntities())
                {
                    var schoolYears = ce.SchoolYears.Where(x => x.SchoolInfoRef == schoolId).ToList();
                    foreach (var schoolYear in schoolYears)
                    {
                        ce.SchoolYears.DeleteObject(schoolYear);
                    }
                    ce.SaveChanges();
                }


                var importService = SisImportProvider.CreateImportService(type, schoolId, taskInfo.TypedData.SisSchoolId, taskInfo.TypedData.SchoolYearIds, serviceLocator.LoggingService);

                importService.ImportPeople(null);
                importService.ImportSchedule(null);
                importService.ImportAttendances(null);


                stateMachine.Apply(StateActionEnum.SisImportAction);
            }
            catch (Exception ex)
            {
                serviceLocator.LoggingService.LogError(ex.Message);
                if (ex.InnerException != null)
                    serviceLocator.LoggingService.LogError(ex.InnerException.Message);
                serviceLocator.LoggingService.LogError(ex.StackTrace);
                return false;
            }*/
            return true;
        }
    }
}