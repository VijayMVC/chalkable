using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Connectors;
//using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.AcademicBenchmarkImport.Mappers;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.AcademicBenchmark;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Master.Model;
//using StandardRelations = Chalkable.AcademicBenchmarkConnector.Models.StandardRelations;

//using Authority = Chalkable.AcademicBenchmarkConnector.Models.Authority;
//using Course = Chalkable.AcademicBenchmarkConnector.Models.Course;
//using Document = Chalkable.AcademicBenchmarkConnector.Models.Document;
//using GradeLevel = Chalkable.AcademicBenchmarkConnector.Models.GradeLevel;
//using Standard = Chalkable.AcademicBenchmarkConnector.Models.Standard;
//using StandardRelations = Chalkable.AcademicBenchmarkConnector.Models.StandardRelations;
//using Subject = Chalkable.AcademicBenchmarkConnector.Models.Subject;

namespace Chalkable.AcademicBenchmarkImport
{
    public class StandardRelationsLoader
    {
        public StandardRelationsLoader(IConnectorLocator abConnectorLocator, IEnumerable<Guid> standardIds)
        {
            ConnectorLocator = abConnectorLocator;
            StandardIdsToProcess = new ConcurrentQueue<Guid>(standardIds);
            Result = new ConcurrentBag<AcademicBenchmarkConnector.Models.StandardRelations>();
            _pool = new List<Thread>();
        }

        protected IConnectorLocator ConnectorLocator { get; }
        protected ConcurrentQueue<Guid> StandardIdsToProcess { get; }
        protected ConcurrentBag<AcademicBenchmarkConnector.Models.StandardRelations> Result { get; set; }

        protected void Worker(object o)
        {
            while (!StandardIdsToProcess.IsEmpty)
            {
                Guid standardId;
                while (StandardIdsToProcess.TryDequeue(out standardId))
                {
                    var id = standardId;
                    var standardRel = Task.Run(() => ConnectorLocator.StandardsConnector.GetStandardRelationsById(id)).Result;

                    if (standardRel != null)
                        Result.Add(standardRel);
                }
            }
        }

        public IList<AcademicBenchmarkConnector.Models.StandardRelations> Load()
        {
            for (var i = 0; i < 10; ++i)
            {
                var currentTh = new Thread(Worker);
                currentTh.Start();

                _pool.Add(currentTh);
            }

            WaitAllThreads(_pool);

            return Result.ToList();
        }

        protected static void WaitAllThreads(IList<Thread> threads)
        {
            foreach (var thread in threads)
                thread.Join();
        }

        private readonly IList<Thread> _pool;
    }
    
    public class ImportResult
    {
        public IList<AcademicBenchmarkConnector.Models.Authority> Authorities { get; set; } 
        public IList<AcademicBenchmarkConnector.Models.Course> Courses { get; set; }
        public IList<AcademicBenchmarkConnector.Models.Document> Documents { get; set; }
        public IList<AcademicBenchmarkConnector.Models.GradeLevel> GradeLevels { get; set; }
        public IList<AcademicBenchmarkConnector.Models.Standard> Standards { get; set; }
        public IList<AcademicBenchmarkConnector.Models.StandardRelations> StandardRelations { get; set; }
        public IList<AcademicBenchmarkConnector.Models.Subject> Subjects { get; set; }
        public IList<AcademicBenchmarkConnector.Models.SubjectDocument> SubjectDocuments { get; set; } 
        public IList<AcademicBenchmarkConnector.Models.Topic> Topics { get; set; } 
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
                        Debug.WriteLine("Ping");
                        var c = uow.GetTextCommand("select 1");
                        c.ExecuteNonQuery();
                        Debug.WriteLine("Pong");
                    }
                    Thread.Sleep(5 * 1000);
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

            ServiceLocator.SyncService.AfterSync();

            dbService.CommitAll();
//StandardRelations
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //var standardRelLoader = new StandardRelationsLoader(ConnectorLocator, importResult.Standards.Select(x => x.Id));
            //var standardRels = standardRelLoader.Load();
            
            //dbService.BeginTransaction();

            //pingThread = new Thread(PingConnection);
            //pingThread.Start(dbService);
            
            //try
            //{
            //    var standardDerivatives = new List<StandardDerivative>();
            //    foreach (var standardRel in standardRels)
            //        standardDerivatives.AddRange(MapperHelper.Map(standardRel) ?? new List<StandardDerivative>());

            //    ServiceLocator.StandardDerivativeService.Add(standardDerivatives);
            //}
            //catch (Exception e)
            //{
            //    dbService.Rollback();
            //    throw;
            //}
            //finally
            //{
            //    try
            //    {
            //        pingThread.Abort();
            //    }
            //    catch (Exception)
            //    {
            //        // ignored
            //    }
            //}

            //ServiceLocator.SyncService.UpdateLastSyncDate(DateTime.UtcNow.Date);
            //ServiceLocator.SyncService.AfterSync();
            
            //dbService.CommitAll();
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        protected void SyncData(DateTime lastSyncDate)
        {

        }

        protected ImportResult DownloadAllDataForImport()
        {
            var importRes = new ImportResult();
            var authorities = Task.Run(() => ConnectorLocator.StandardsConnector.GetAuthorities());        
            var documents = Task.Run(() => ConnectorLocator.StandardsConnector.GetDocuments(null));

            var standardGradeLevels = Task.Run(() => ConnectorLocator.StandardsConnector.GetGradeLevels(null, null, null));
            var standardSubjects = Task.Run(() => ConnectorLocator.StandardsConnector.GetSubjects(null, null));
            var standardSubjectsDocs = Task.Run(() => ConnectorLocator.StandardsConnector.GetSubjectDocuments(null, null));
            var standardCourses = Task.Run(() => ConnectorLocator.StandardsConnector.GetCourses(null, null, null, null));

            var topicGradeLevels = Task.Run(() => ConnectorLocator.TopicsConnector.GetGradeLevels());
            var topicSubjects = Task.Run(() => ConnectorLocator.TopicsConnector.GetSubjects());
            var topicSubjectsDocs = Task.Run(() => ConnectorLocator.TopicsConnector.GetSubjectDocuments());
            var topicCourses = Task.Run(() => ConnectorLocator.TopicsConnector.GetCourses(null));

            var standards = Task.Run(() => ConnectorLocator.StandardsConnector.GetAllStandards(0, int.MaxValue));
            var topics = Task.Run(() => ConnectorLocator.TopicsConnector.GetTopics());

            importRes.Authorities = authorities.Result;
            importRes.Documents = documents.Result;

            importRes.Courses = standardCourses.Result.Union(topicCourses.Result).ToList();
            importRes.GradeLevels = standardGradeLevels.Result.Union(topicGradeLevels.Result).ToList();
            importRes.Subjects = standardSubjects.Result.Union(topicSubjects.Result).ToList();
            importRes.SubjectDocuments = standardSubjectsDocs.Result.Union(topicSubjectsDocs.Result).ToList();

            importRes.Standards = standards.Result;
            importRes.Topics = topics.Result;
            
            //importRes.StandardRelations = new List<AcademicBenchmarkConnector.Models.StandardRelations>(); ;

            return importRes;
        }

        protected void InsertAllDataToDb(ImportResult importResult)
        {
            ServiceLocator.AuthorityService.Add(importResult.Authorities?.Select(MapperHelper.Map).ToList());
            ServiceLocator.DocumentService.Add(importResult.Documents?.Select(MapperHelper.Map).ToList());

            ServiceLocator.CourseService.Add(importResult.Courses?.Select(MapperHelper.Map).ToList());
            ServiceLocator.GradeLevelService.Add(importResult.GradeLevels?.Select(MapperHelper.Map).ToList());
            ServiceLocator.SubjectService.Add(importResult.Subjects?.Select(MapperHelper.Map).ToList());
            ServiceLocator.SubjectDocService.Add(importResult.SubjectDocuments?.Select(MapperHelper.Map).ToList());

            ServiceLocator.StandardService.Add(importResult.Standards?.Select(MapperHelper.Map).ToList());
            ServiceLocator.TopicService.Add(importResult.Topics?.Select(MapperHelper.Map).ToList());
        }
    }
}
