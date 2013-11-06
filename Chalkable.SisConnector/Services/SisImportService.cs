using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.SisConnector.PublicModel;
using SchoolInfo = Chalkable.SisConnector.PublicModel.SchoolInfo;

namespace Chalkable.SisConnector.Services
{
    public abstract class SisImportService
    {
        protected IServiceLocatorMaster ServiceLocatorMaster { get; set; }
        protected IServiceLocatorSchool ServiceLocatorSchool { get; set; }
        protected Guid SchoolId { get; private set; }
        protected int SisSchoolId { get; private set; }
        protected BackgroundTaskService.BackgroundTaskLog Log { get; private set; }
        protected IList<int> SisSchoolYearIds { get; private set; }
        protected SisConnectionInfo ConnectionInfo { get; private set; }

        protected SisImportService(Guid schoolId, int sisSchoolId, IList<int> schoolYearIds, SisConnectionInfo connectionInfo, BackgroundTaskService.BackgroundTaskLog log)
        {
            ServiceLocatorMaster = ServiceLocatorFactory.CreateMasterSysAdmin();
            ConnectionInfo = connectionInfo;
            if (schoolId != Guid.Empty)
            {
                var school = ServiceLocatorMaster.SchoolService.GetById(schoolId);
                ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(school.Id);    
            }
            SchoolId = schoolId;
            SisSchoolId = sisSchoolId;
            Log = log;
            SisSchoolYearIds = schoolYearIds;
        }

        public abstract IList<SchoolInfo> GetSchools();

        public abstract void ImportAttendances(DateTime? lastUpdate);
        public abstract void ImportSchedule(DateTime? lastUpdate);
        public abstract void ImportPeople(DateTime? lastUpdate);
    }
}
