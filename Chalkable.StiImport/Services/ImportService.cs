using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using System.Transactions;

namespace Chalkable.StiImport.Services
{
    public partial class ImportService
    {
        private SyncContext context;
        private const string USER_EMAIL_FMT = "user{0}_{1}@chalkable.com";
        private const string DEF_USER_PASS = "Qwerty1@";
        private const string DESCR_WORK = "Work";
        private const string DESCR_CELL = "cell";
        private const string UNKNOWN_ROOM_NUMBER = "Unknown number";
        private IList<int> importedSchoolIds = new List<int>();
        private ConnectorLocator connectorLocator;
        private IList<Person> personsForImportPictures = new List<Person>();

        protected IServiceLocatorMaster ServiceLocatorMaster { get; set; }
        protected IServiceLocatorSchool ServiceLocatorSchool { get; set; }
        protected BackgroundTaskService.BackgroundTaskLog Log { get; private set; }
        protected SisConnectionInfo ConnectionInfo { get; private set; }

        public ImportService(Guid districtId, SisConnectionInfo connectionInfo, BackgroundTaskService.BackgroundTaskLog log)
        {
            ConnectionInfo = connectionInfo;
            Log = log;
            
            var admin = new Data.Master.Model.User { Id = Guid.Empty, Login = "Virtual system admin" };
            var cntx = new UserContext(admin, CoreRoles.SUPER_ADMIN_ROLE, null, null, null);
            ServiceLocatorMaster = new ImportServiceLocatorMaster(cntx);
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(districtId, null);
        }

        public void Import()
        {
            Log.LogInfo("start import");


            var masterDb = (ImportDbService)ServiceLocatorMaster.DbService;
            var schoolDb = (ImportDbService)ServiceLocatorSchool.SchoolDbService;
            Log.LogInfo("begin master transaction");
            masterDb.BeginTransaction();
            Log.LogInfo("begin school transaction");
            schoolDb.BeginTransaction();
            //TODO: clean this 
            Thread t1 = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        using (var uow = masterDb.GetUowForRead())
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
            });

            Thread t2 = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        using (var uow = schoolDb.GetUowForRead())
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
            });
            t1.Start();
            t2.Start();


            importedSchoolIds.Clear();
            personsForImportPictures.Clear();
            connectorLocator = ConnectorLocator.Create(ConnectionInfo.SisUserName, ConnectionInfo.SisPassword, ConnectionInfo.SisUrl);
            Log.LogInfo("download data to sync");
            DownloadSyncData();
            
            bool schoolCommited = false;
            try
            {
                SyncDb();
                Log.LogInfo("commit school db");
                schoolDb.CommitAll();
                schoolCommited = true;
                Log.LogInfo("commit master db");
                masterDb.CommitAll();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                if (!schoolCommited)
                {
                    Log.LogInfo("rollback school db");
                    schoolDb.Rollback();    
                }
                else
                    Log.LogInfo("!!!!!!!!school is commited but master is going to rollback!!!!!!!!!!!!!!!!!!!!!");//TODO.....
                Log.LogInfo("rollback master db");
                masterDb.Rollback();
                throw;
            }
            Log.LogInfo("process pictures");
            ProcessPictures();
            Log.LogInfo("setting link status");
            foreach (var importedSchoolId in importedSchoolIds)
            {
                connectorLocator.LinkConnector.CompleteSync(importedSchoolId);
            }

            try
            {
                t1.Abort();
                t2.Abort();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            Log.LogInfo("import is completed");
        }

        private void SyncDb()
        {
            Log.LogInfo("process inserts");
            ProcessInsert();
            Log.LogInfo("process updates");
            ProcessUpdate();
            Log.LogInfo("process deletes");
            ProcessDelete();
            Log.LogInfo("update versions");
            UpdateVersion();
            UpdateDistrictLastSync();
        }

        private void UpdateDistrictLastSync()
        {
            if (!ServiceLocatorSchool.Context.DistrictId.HasValue)
                throw new Exception("District id should be defined for import");
            var d = ServiceLocatorMaster.DistrictService.GetByIdOrNull(ServiceLocatorSchool.Context.DistrictId.Value);
            d.LastSync = DateTime.UtcNow;
            ServiceLocatorMaster.DistrictService.Update(d);
        }

        private void ProcessPictures()
        {
            if (!ServiceLocatorSchool.Context.DistrictId.HasValue)
                throw new Exception("District id should be defined for import");
            foreach (var person in personsForImportPictures)
            {
                var content = connectorLocator.UsersConnector.GetPhoto(person.PersonID);
                if (content != null)
                    ServiceLocatorMaster.PersonPictureService.UploadPicture(ServiceLocatorSchool.Context.DistrictId.Value, person.PersonID ,content);
                else
                    ServiceLocatorMaster.PersonPictureService.DeletePicture(ServiceLocatorSchool.Context.DistrictId.Value, person.PersonID);
            }
        }

        public void DownloadSyncData()
        {
            context = new SyncContext();
            var currentVersions = ServiceLocatorSchool.SyncService.GetVersions();
            context.SetCurrentVersions(currentVersions);
            //Tables we need all data
            context.TablesToSync[typeof(ScheduledTimeSlot).Name] = null;
            context.TablesToSync[typeof(Gender).Name] = null;
            context.TablesToSync[typeof(SpEdStatus).Name] = null;

            var toSync = context.TablesToSync;
            var results = new List<SyncResultBase>();
            
            foreach (var table in toSync)
            {
                var type = context.Types[table.Key];
                Log.LogInfo("Start downloading " + table.Key);
                var res = (SyncResultBase)connectorLocator.SyncConnector.GetDiff(type, table.Value);
                Log.LogInfo("Table downloaded: " + table.Key + " " + res.RowCount);
                results.Add(res);
            }
            foreach (var syncResultBase in results)
            {
                context.SetResult(syncResultBase);   
            }
        }

        public void UpdateVersion()
        {
            var newVersions = new Dictionary<string, int>();
            foreach (var i in context.TablesToSync)
                if (i.Value.HasValue)
                {
                    newVersions.Add(i.Key, i.Value.Value);
                }
            ServiceLocatorSchool.SyncService.UpdateVersions(newVersions);
        }

        private void ProcessByParts<T>(IList<T> source, Action<IList<T>> processor, int count, string logMsg)
        {
            for (int i = 0; i < source.Count; i += count)
            {
                var l = source.Skip(i).Take(count).ToList();
                if (logMsg != null)
                    Log.LogInfo(string.Format("starting {0} {1} {2}", logMsg, i, l.Count));
                processor(l);
                if (logMsg != null)
                    Log.LogInfo(string.Format("finished {0} {1} {2}", logMsg, i, l.Count));
            }
        }
    }

    public class SisConnectionInfo
    {
        public string DbName { get; set; }
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
    }
}
