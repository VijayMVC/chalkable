using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.SisConnector.Services
{
    public abstract class SisExportService
    {
        protected IServiceLocatorMaster ServiceLocatorMaster { get; set; }
        protected IServiceLocatorSchool ServiceLocatorSchool { get; set; }
        protected Guid SchoolId { get; private set; }
        protected int SisSchoolId { get; private set; }
        protected BackgroundTaskService.BackgroundTaskLog Log { get; private set; }

        protected SisExportService(Guid schoolId, int sisSchoolId, BackgroundTaskService.BackgroundTaskLog log)
        {
            ServiceLocatorMaster = ServiceLocatorFactory.CreateMasterSysAdmin();
            var school = ServiceLocatorMaster.SchoolService.GetById(schoolId);
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(school.Id, school.Name, school.TimeZone, school.ServerUrl);
            SchoolId = schoolId;
            SisSchoolId = sisSchoolId;
            Log = log;
        }

        public abstract void ExportAttendances(int sisSchoolId);
    }
}