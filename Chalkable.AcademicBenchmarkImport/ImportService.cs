using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Connectors;
using Chalkable.AcademicBenchmarkImport.Mappers;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.AcademicBenchmark;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.AcademicBenchmarkImport
{
    public class ImportService
    {
        protected IConnectorLocator ConnectorLocator { get; set; }
        protected IAcademicBenchmarkServiceLocator ServiceLocator { get; set; }
        private UserContext _sysAdminContext;
        
        public ImportService(BackgroundTaskService.BackgroundTaskLog log)
        {
            var admin = new User {Id = Guid.Empty, Login = "Virtual system admin", LoginInfo = new UserLoginInfo()};
            _sysAdminContext = new UserContext(admin, CoreRoles.SUPER_ADMIN_ROLE, null, null, null, null, null);
        }

        public void Import()
        {
            ConnectorLocator = ConnectorLocator ?? new ConnectorLocator();
            var connectionStr = @"Data Source=uhjc12n4yc.database.windows.net;Initial Catalog=ChalkableAcademicBenchmark;UID=chalkableadmin;Pwd=Hellowebapps1!";
            ServiceLocator = ServiceLocator ?? new AcademicBenchmarkServiceLocator(_sysAdminContext, connectionStr);

            var lastSyncDate = ServiceLocator.SyncService.GetLastSyncDateOrNull();

            if(lastSyncDate.HasValue)
                SyncData(lastSyncDate.Value);
            else
                FullImport();
        }

        protected void FullImport()
        {
            ImportSimpleTables();
            ServiceLocator.SyncService.UpdateLastSyncDate(DateTime.UtcNow.Date);
        }

        protected void SyncData(DateTime lastSyncDate)
        {
            //ImportSimpleTables();
            //ServiceLocator.SyncService.UpdateLastSyncDate(DateTime.UtcNow.Date);
        }

        protected void ImportSimpleTables()
        {
            var authorities = Task.Run(() => ConnectorLocator.StandardsConnector.GetAuthorities()).Result;
            ServiceLocator.AuthorityService.Add(authorities.Select(MapperHelper.Map).ToList());

            var courses = Task.Run(() => ConnectorLocator.StandardsConnector.GetCourses(null, null, null, null)).Result;
            ServiceLocator.CourseService.Add(courses.Select(MapperHelper.Map).ToList());

            var documents = Task.Run(() => ConnectorLocator.StandardsConnector.GetDocuments(null)).Result;
            ServiceLocator.DocumentService.Add(documents.Select(MapperHelper.Map).ToList());

            var gradeLevels = Task.Run(() => ConnectorLocator.StandardsConnector.GetGradeLevels(null, null, null)).Result;
            ServiceLocator.GradeLevelService.Add(gradeLevels.Select(MapperHelper.Map).ToList());

            var subjects = Task.Run(() => ConnectorLocator.StandardsConnector.GetSubjects(null, null)).Result;
            ServiceLocator.SubjectService.Add(subjects.Select(MapperHelper.Map).ToList());

            var subjectsDocs = Task.Run(() => ConnectorLocator.StandardsConnector.GetSubjectDocuments(null, null)).Result;
            ServiceLocator.SubjectDocService.Add(subjectsDocs.Select(MapperHelper.Map).ToList());
        }
    }
}
