using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;

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
        private UserContext sysadminCntx;

        protected IServiceLocatorMaster ServiceLocatorMaster { get; set; }
        protected IServiceLocatorSchool ServiceLocatorSchool { get; set; }
        protected BackgroundTaskService.BackgroundTaskLog Log { get; private set; }
        protected SisConnectionInfo ConnectionInfo { get; private set; }

        public ImportService(Guid districtId, SisConnectionInfo connectionInfo, BackgroundTaskService.BackgroundTaskLog log)
        {
            ConnectionInfo = connectionInfo;
            Log = log;
            
            var admin = new Data.Master.Model.User { Id = Guid.Empty, Login = "Virtual system admin", LoginInfo =  new UserLoginInfo()};
            sysadminCntx = new UserContext(admin, CoreRoles.SUPER_ADMIN_ROLE, null, null, null, null);
            ServiceLocatorMaster = new ImportServiceLocatorMaster(sysadminCntx);
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(districtId, null);
        }

        void PingConnection(object o)
        {
            ImportDbService dbService = (ImportDbService)o;
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
            Log.LogInfo("start import");
            connectorLocator = ConnectorLocator.Create(ConnectionInfo.SisUserName, ConnectionInfo.SisPassword, ConnectionInfo.SisUrl);
            importedSchoolIds.Clear();
            personsForImportPictures.Clear();

            DownloadSyncData();

            var masterDb = (ImportDbService)ServiceLocatorMaster.DbService;
            var schoolDb = (ImportDbService)ServiceLocatorSchool.SchoolDbService;
            Log.LogInfo("begin master transaction");
            masterDb.BeginTransaction();
            Log.LogInfo("begin school transaction");
            schoolDb.BeginTransaction();
            var t1 = new Thread(PingConnection);
            var t2 = new Thread(PingConnection);
            t1.Start(masterDb);
            t2.Start(schoolDb);
            
            Log.LogInfo("download data to sync");
            //TODO: can we do this before transaction opens?
            
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
                    Log.LogInfo("!!!!!!!!school is commited but master is going to rollback!!!!!!!!!!!!!!!!!!!!!");
                        //TODO.....
                Log.LogInfo("rollback master db");
                masterDb.Rollback();
                throw;
            }
            finally
            {
                try
                {
                    t1.Abort();
                    t2.Abort();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }    
            }
            schoolDb.Dispose();
            masterDb.Dispose();
            Log.LogInfo("process pictures");
            ProcessPictures();
            Log.LogInfo("setting link status");
            foreach (var importedSchoolId in importedSchoolIds)
                connectorLocator.LinkConnector.CompleteSync(importedSchoolId);
            //recreating locator bc previous one could have expired connection
            ServiceLocatorMaster = new ImportServiceLocatorMaster(sysadminCntx);
            Log.LogInfo("updating district last sync");
            UpdateDistrictLastSync();
            Log.LogInfo("creating user login infos");
            ServiceLocatorMaster.UserService.CreateUserLoginInfos();
            Log.LogInfo("import is completed");
            ((ImportDbService)ServiceLocatorMaster.DbService).Dispose();
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
            IList<int> ids = new List<int>();
            const int personsPerTask = 5000;
            var districtId = ServiceLocatorSchool.Context.DistrictId.Value;
            for (int i = 0; i < personsForImportPictures.Count; i++ )
            {
                ids.Add(personsForImportPictures[i].PersonID);
                if (ids.Count >= personsPerTask || ids.Count > 0 && i + 1 == personsForImportPictures.Count)
                {
                    var data = new PictureImportTaskData(districtId, ids);
                    ServiceLocatorMaster.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.PictureImport, DateTime.UtcNow, null, data.ToString());
                    ids.Clear();
                }
            }
        }

        public void DownloadSyncData()
        {
            context = new SyncContext();
            var currentVersions = ServiceLocatorSchool.SyncService.GetVersions();
            context.SetCurrentVersions(currentVersions);
            //Tables we need all data
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

        private IList<T> TopologicSort<T>(Func<T, int> id, Func<T, int?> reference, Dictionary<int, T> source)
        {
            var was = new HashSet<int>();
            var res = new List<T>();
            foreach (var s in source)
                if (!was.Contains(s.Key))
                    TopologicSort(id(s.Value), reference, source, was, res);
            return res;
        }

        private void TopologicSort<T>(int id, Func<T, int?> reference, Dictionary<int, T> source, HashSet<int> was, IList<T> res)
        {
            was.Add(id);
            var standard = source[id];
            var r = reference(standard);
            if (r.HasValue && !was.Contains(r.Value))
                TopologicSort(r.Value, reference, source, was, res);
            res.Add(standard);
        }
    }

    public class SisConnectionInfo
    {
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
    }
}
