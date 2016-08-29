using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Connectors;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.AcademicBenchmarkImport.Mappers;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.AcademicBenchmark;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.AcademicBenchmarkImport
{
    public class ImportResult
    {
        public IList<Authority> Authorities { get; set; } 
        public IList<Course> Courses { get; set; }
        public IList<Document> Documents { get; set; }
        public IList<GradeLevel> GradeLevels { get; set; }
        public IList<Standard> Standards { get; set; }
        public IList<StandardRelations> StandardRelations { get; set; }
        public IList<Subject> Subjects { get; set; }
        public IList<SubjectDocument> SubjectDocuments { get; set; } 
    }

    public class ImportService
    {
        

        protected IConnectorLocator ConnectorLocator { get; set; }
        protected IAcademicBenchmarkServiceLocator ServiceLocator { get; set; }
        protected BackgroundTaskService.BackgroundTaskLog Log { get; }

        private UserContext _sysAdminContext;
        
        public ImportService(BackgroundTaskService.BackgroundTaskLog log)
        {
            var admin = new User {Id = Guid.Empty, Login = "Virtual system admin", LoginInfo = new UserLoginInfo()};
            _sysAdminContext = new UserContext(admin, CoreRoles.SUPER_ADMIN_ROLE, null, null, null, null, null);
            Log = log;
        }

        void PingConnection(object o)
        {
            var dbService = (ImportDbService)o;
            while (true)
            {
                try
                {
                    using (var uow = dbService.GetUowForRead())
                    {
                        var c = uow.GetTextCommand("select 1");
                        c.ExecuteNonQuery();
                    }
                    Thread.Sleep(60 * 1000);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }
        }

        public void Import()
        {
            ConnectorLocator = ConnectorLocator ?? new ConnectorLocator();
            var connectionStr = @"Data Source=uhjc12n4yc.database.windows.net;Initial Catalog=ChalkableAcademicBenchmark;UID=chalkableadmin;Pwd=Hellowebapps1!";
            ServiceLocator = ServiceLocator ?? new ImportAcademicBenchmarkServiceLocator(_sysAdminContext, connectionStr);

            var lastSyncDate = ServiceLocator.SyncService.GetLastSyncDateOrNull();

            if(lastSyncDate.HasValue)
                SyncData(lastSyncDate.Value);
            else
                FullImport();
        }

        protected void FullImport()
        {
            var importResult = DownloadAllDataForImport();

            var dbService = (ImportDbService)ServiceLocator.DbService;

            //Log.LogInfo("No last sync date. Started full import.");

            dbService.BeginTransaction();

            var pingThread = new Thread(PingConnection);
            pingThread.Start(dbService);

            try
            {
                ServiceLocator.SyncService.BeforeSync();
                InsertAllDataToDb(importResult);
            }
            catch (Exception e)
            {
                dbService.Rollback();
                throw;
            }
            finally
            {
                try
                {
                    pingThread.Abort();
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            ServiceLocator.SyncService.UpdateLastSyncDate(DateTime.UtcNow.Date);
            ServiceLocator.SyncService.AfterSync();
            dbService.CommitAll();
        }

        protected void SyncData(DateTime lastSyncDate)
        {

        }

        protected ImportResult DownloadAllDataForImport()
        {
            var importRes = new ImportResult();
            var authorities = Task.Run(() => ConnectorLocator.StandardsConnector.GetAuthorities());
            var courses = Task.Run(() => ConnectorLocator.StandardsConnector.GetCourses(null, null, null, null));
            var documents = Task.Run(() => ConnectorLocator.StandardsConnector.GetDocuments(null));
            var gradeLevels = Task.Run(() => ConnectorLocator.StandardsConnector.GetGradeLevels(null, null, null));
            var subjects = Task.Run(() => ConnectorLocator.StandardsConnector.GetSubjects(null, null));
            var subjectsDocs = Task.Run(() => ConnectorLocator.StandardsConnector.GetSubjectDocuments(null, null));
            var standards = Task.Run(() => ConnectorLocator.StandardsConnector.GetAllStandards(0, 1000));

            importRes.Authorities = authorities.Result;
            importRes.Courses = courses.Result;
            importRes.Documents = documents.Result;
            importRes.GradeLevels = gradeLevels.Result;
            importRes.Subjects = subjects.Result;
            importRes.SubjectDocuments = subjectsDocs.Result;
            importRes.Standards = standards.Result;

            return importRes;
        }

        protected void InsertAllDataToDb(ImportResult importResult)
        {
            ServiceLocator.AuthorityService.Add(importResult.Authorities?.Select(MapperHelper.Map).ToList());
            ServiceLocator.CourseService.Add(importResult.Courses?.Select(MapperHelper.Map).ToList());
            ServiceLocator.DocumentService.Add(importResult.Documents?.Select(MapperHelper.Map).ToList());
            ServiceLocator.GradeLevelService.Add(importResult.GradeLevels?.Select(MapperHelper.Map).ToList());
            ServiceLocator.SubjectService.Add(importResult.Subjects?.Select(MapperHelper.Map).ToList());
            ServiceLocator.SubjectDocService.Add(importResult.SubjectDocuments?.Select(MapperHelper.Map).ToList());
            ServiceLocator.StandardService.Add(importResult.Standards?.Select(MapperHelper.Map).ToList());
        }
    }
}
