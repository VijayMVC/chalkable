using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Connectors;
using Chalkable.AcademicBenchmarkImport.Mappers;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.AcademicBenchmark;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Master.Model;
using Chalkable.AcademicBenchmarkImport.Helper;
using Chalkable.AcademicBenchmarkImport.Model;

namespace Chalkable.AcademicBenchmarkImport
{
    internal enum OperationType
    {
        Insert,
        Delete,
        Update
    }

    public class ImportService
    {
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
            //var connectionStr = @"Data Source=uhjc12n4yc.database.windows.net;Initial Catalog=ChalkableAcademicBenchmark_Testing;UID=chalkableadmin;Pwd=Hellowebapps1!";
            ServiceLocator = ServiceLocator ?? new ImportAcademicBenchmarkServiceLocator(_sysAdminContext, Settings.AcademicBenchmarkDbConnectionString);

            var lastSyncDate = ServiceLocator.SyncService.GetLastSyncDateOrNull();

            if(lastSyncDate.HasValue)
                SyncData(lastSyncDate.Value);
            else
                FullImport();
        }

        protected void FullImport()
        {
            Log.LogInfo("Last sync date is empty. Started full import.");
            var importResult = DownloadAllDataForImport();

            var dbService = (ImportDbService)ServiceLocator.DbService;

            dbService.BeginTransaction();
            Log.LogInfo("Started database transaction");
            var pingThread = new Thread(PingConnection);
            pingThread.Start(dbService);

            try
            {
                ServiceLocator.SyncService.BeforeSync();
                ProcessImportResult(importResult);
            }
            catch (Exception e)
            {
                Log.LogInfo($"Error: {e.Message}");
                Log.LogInfo("Transaction is going to rollback");
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
            Log.LogInfo("Transaction was commited");
/////////////////////////////////////Proccessing Standardar Relations in another transaction////////////////////////////////////////////////////
            var standardIds = importResult.Standards.Select(x => x.Id).ToList();
            Log.LogInfo("Loading Standard Relations . . .");
            var standardRelLoader = new StandardRelationsLoader(ConnectorLocator, standardIds);
            var standardRels = standardRelLoader.Load();

            dbService.BeginTransaction();
            Log.LogInfo("Started database transaction");

            pingThread = new Thread(PingConnection);
            pingThread.Start(dbService);

            try
            {
                var standardDerivatives = new List<StandardDerivative>();
                foreach (var standardRel in standardRels)
                    standardDerivatives.AddRange(MapperHelper.Map(standardRel) ?? new List<StandardDerivative>());

                ServiceLocator.StandardDerivativeService.Add(standardDerivatives);
            }
            catch (Exception e)
            {
                Log.LogInfo($"Error: {e.Message}");
                Log.LogInfo("Transaction is going to rollback");
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
            
            dbService.CommitAll();
            Log.LogInfo("Transaction was commited.");
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        protected void SyncData(DateTime lastSyncDate)
        {
            var syncResult = DownloadAllDataForSync(lastSyncDate);

            var dbService = (ImportDbService)ServiceLocator.DbService;

            dbService.BeginTransaction();

            var pingThread = new Thread(PingConnection);
            pingThread.Start(dbService);

            try
            {
                ServiceLocator.SyncService.BeforeSync();
                ProcessSyncResult(syncResult);
            }
            catch (Exception e)
            {
                Log.LogInfo($"Error: {e.Message}");
                Log.LogInfo("Transaction is going to rollback");
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
            ServiceLocator.SyncService.UpdateLastSyncDate(DateTime.UtcNow.Date);

            dbService.CommitAll();
            Log.LogInfo("Transaction was commited.");
        }

        protected void ProcessImportResult(ImportResult importResult)
        {
            ProcessBaseResult(importResult);

            ServiceLocator.StandardService.Add(importResult.Standards?.Select(MapperHelper.Map).ToList());
            Log.LogInfo("Processed Standard table");
            ServiceLocator.TopicService.Add(importResult.Topics?.Select(MapperHelper.Map).ToList());
            Log.LogInfo("Processed Topic table");
        }

        protected void ProcessSyncResult(SyncResult syncResult)
        {
            ProcessBaseResult(syncResult);
            var standardSyncData = SyncData<Standard, Guid>.Create(syncResult.StandardSyncItems, syncResult.Standards);
            ServiceLocator.StandardService.Add(standardSyncData.Insert?.Select(MapperHelper.Map).ToList());
            ServiceLocator.StandardService.Edit(standardSyncData.Update?.Select(MapperHelper.Map).ToList());
            ServiceLocator.StandardService.Delete(standardSyncData.Delete);
            Log.LogInfo("Processed Standard table");

            var topicSyncData = SyncData<Topic, Guid>.Create(syncResult.TopicSyncItems, syncResult.Topics);
            ServiceLocator.TopicService.Add(topicSyncData.Insert?.Select(MapperHelper.Map).ToList());
            ServiceLocator.TopicService.Edit(topicSyncData.Update?.Select(MapperHelper.Map).ToList());
            ServiceLocator.TopicService.Delete(topicSyncData.Delete);
            Log.LogInfo("Processed Topic table");
        }

        protected void ProcessBaseResult(ResultBase resultBase)
        {
            ServiceLocator.AuthorityService.Add(resultBase.Authorities?.Select(MapperHelper.Map).ToList());
            ServiceLocator.DocumentService.Add(resultBase.Documents?.Select(MapperHelper.Map).ToList());

            ServiceLocator.CourseService.Add(resultBase.Courses?.Select(MapperHelper.Map).ToList());
            ServiceLocator.GradeLevelService.Add(resultBase.GradeLevels?.Select(MapperHelper.Map).ToList());
            ServiceLocator.SubjectService.Add(resultBase.Subjects?.Select(MapperHelper.Map).ToList());
            ServiceLocator.SubjectDocService.Add(resultBase.SubjectDocuments?.Select(MapperHelper.Map).ToList());

            Log.LogInfo("Processed Authority, Document, Course, GradeLevel, Subject, SubjectDoc tables");
        }

        protected ImportResult DownloadAllDataForImport()
        {
            var baseResult = DownloadBaseData();
            var importRes = new ImportResult(baseResult);

            var standards = Task.Run(() => ConnectorLocator.StandardsConnector.GetAllStandards(0, int.MaxValue));
            Log.LogInfo("Downloaded Standard table");
            var topics = Task.Run(() => ConnectorLocator.TopicsConnector.GetTopics());
            Log.LogInfo("Downloaded Topic table");
            
            importRes.Standards = standards.Result;
            importRes.Topics = topics.Result;

            return importRes;
        }

        protected SyncResult DownloadAllDataForSync(DateTime lastSyncDate)
        {
            var resultBase = DownloadBaseData();
            
            var standardSyncItems = Task.Run(() => ConnectorLocator.SyncConnector.GetStandardsSyncData(lastSyncDate, 0, int.MaxValue));
            var topicSyncItems = Task.Run(() => ConnectorLocator.SyncConnector.GetTopicsSyncData(lastSyncDate, 0, int.MaxValue));
            var syncRes = new SyncResult(resultBase)
            {
                StandardSyncItems = standardSyncItems.Result,
                TopicSyncItems = topicSyncItems.Result
            };

            var standardIds = syncRes.StandardSyncItems.Select(x => x.Id);
            var standardLoader = new LoaderBase<Guid, AcademicBenchmarkConnector.Models.Standard>(standardIds);
            syncRes.Standards = standardLoader.Load(id => Task.Run(() => ConnectorLocator.StandardsConnector.GetStandardById(id)).Result);

            var topicIds = syncRes.TopicSyncItems.Select(x => x.Id);
            var topicLoader = new LoaderBase<Guid, AcademicBenchmarkConnector.Models.Topic>(topicIds);
            syncRes.Topics = topicLoader.Load(id => Task.Run(() => ConnectorLocator.TopicsConnector.GetTopic(id)).Result);
            
            Log.LogInfo("Downloaded Standard, Topic sync data");
            return syncRes;
        }

        protected ResultBase DownloadBaseData()
        {
            var result = new ResultBase();
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

            result.Authorities = authorities.Result;
            result.Documents = documents.Result;

            result.Courses = standardCourses.Result.Union(topicCourses.Result).ToList();
            result.GradeLevels = standardGradeLevels.Result.Union(topicGradeLevels.Result).ToList();
            result.Subjects = standardSubjects.Result.Union(topicSubjects.Result).ToList();
            result.SubjectDocuments = standardSubjectsDocs.Result.Union(topicSubjectsDocs.Result).ToList();

            Log.LogInfo("Downloaded Authority, Document, Course, GradeLevel, Subject, SubjectDoc tables");

            return result;
        }

        protected IConnectorLocator ConnectorLocator { get; set; }
        protected IAcademicBenchmarkServiceLocator ServiceLocator { get; set; }
        protected BackgroundTaskService.BackgroundTaskLog Log { get; }

        private readonly UserContext _sysAdminContext;
    }
}
