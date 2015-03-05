using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using User = Chalkable.StiConnector.SyncModel.User;

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

        private Guid districtId;
        protected IServiceLocatorMaster ServiceLocatorMaster { get; set; }
        protected IServiceLocatorSchool ServiceLocatorSchool { get; set; }
        protected BackgroundTaskService.BackgroundTaskLog Log { get; private set; }
        protected SisConnectionInfo ConnectionInfo { get; private set; }

        public ImportService(Guid districtId, SisConnectionInfo connectionInfo, BackgroundTaskService.BackgroundTaskLog log)
        {
            ConnectionInfo = connectionInfo;
            Log = log;
            this.districtId = districtId;
            var admin = new Data.Master.Model.User { Id = Guid.Empty, Login = "Virtual system admin", LoginInfo =  new UserLoginInfo()};
            sysadminCntx = new UserContext(admin, CoreRoles.SUPER_ADMIN_ROLE, null, null, null, null);
            
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
            Log.LogInfo("start import");
            ServiceLocatorMaster = new ServiceLocatorMaster(sysadminCntx);
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(districtId, null);
            if (!ServiceLocatorSchool.Context.DistrictId.HasValue)
                throw new Exception("District id should be defined for import");
            importedSchoolIds.Clear();
            personsForImportPictures.Clear();
            
            var d = ServiceLocatorMaster.DistrictService.GetByIdOrNull(ServiceLocatorSchool.Context.DistrictId.Value);
            try
            {
                connectorLocator = ConnectorLocator.Create(ConnectionInfo.SisUserName, ConnectionInfo.SisPassword, ConnectionInfo.SisUrl);
                Log.LogInfo("download data to sync");
                DownloadSyncData();
                if (d.LastSync.HasValue)
                    DoRegularSync(true);
                else
                    DoInitialSync();
                Log.LogInfo("updating district last sync");
                UpdateDistrictLastSync(d, true);
            }
            catch (Exception)
            {
                UpdateDistrictLastSync(d, false);
                throw;
            }
            
            Log.LogInfo("process pictures");
            ProcessPictures();
            Log.LogInfo("setting link status");
            foreach (var importedSchoolId in importedSchoolIds)
                connectorLocator.LinkConnector.CompleteSync(importedSchoolId);
            
            if (context.GetSyncResult<User>().All.Length > 0)
            {
                Log.LogInfo("creating user login infos");
                ServiceLocatorMaster.UserService.CreateUserLoginInfos();    
            }
            Log.LogInfo("import is completed");
        }

        public void Resync(string tableName)
        {
            Log.LogInfo("start resync");
            connectorLocator = ConnectorLocator.Create(ConnectionInfo.SisUserName, ConnectionInfo.SisPassword, ConnectionInfo.SisUrl);
            ServiceLocatorMaster = new ServiceLocatorMaster(sysadminCntx);
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(districtId, null);
            Log.LogInfo("download data to resync");
            context = new SyncContext();
            var toSync = context.TablesToSync;
            var results = new List<SyncResultBase>();
            foreach (var table in toSync)
            {
                var type = context.Types[table.Key];
                SyncResultBase res;
                var resType = (typeof(SyncResult<>)).MakeGenericType(new[] { type });
                if (table.Key == tableName)
                {
                    Log.LogInfo("Start downloading " + table.Key);
                    res = (SyncResultBase) connectorLocator.SyncConnector.GetDiff(type, null);
                    Log.LogInfo("Table downloaded: " + table.Key + " " + res.RowCount);
                    object o = resType.GetProperty("Rows").GetValue(res);
                    resType.GetProperty("Updated").SetValue(res, o);
                    resType.GetProperty("Inserted").SetValue(res, null);
                    resType.GetProperty("Deleted").SetValue(res, null);
                    resType.GetProperty("Rows").SetValue(res, null);
                }
                else
                {
                    res = (SyncResultBase)Activator.CreateInstance(resType, new object[0]);
                }
                results.Add(res);
            }
            foreach (var syncResultBase in results)
            {
                context.SetResult(syncResultBase);
            }
            if (!ServiceLocatorSchool.Context.DistrictId.HasValue)
                throw new Exception("District id should be defined for import");
            DoRegularSync(false);
        }

        private void DoInitialSync()
        {
            try
            {
                SyncDb(true);
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                throw;
            }
        }

        private void DoRegularSync(bool updateVersions)
        {
            ServiceLocatorMaster = new ImportServiceLocatorMaster(sysadminCntx);
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(districtId, null);

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


            bool schoolCommited = false;
            try
            {
                SyncDb(updateVersions);
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
            ServiceLocatorMaster = new ServiceLocatorMaster(sysadminCntx);
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(districtId, null);
        }

        private void SyncDb(bool updateVersions)
        {
            Log.LogInfo("process inserts");
            ProcessInsert();
            Log.LogInfo("process updates");
            ProcessUpdate();
            Log.LogInfo("process deletes");
            ProcessDelete();
            if (updateVersions)
            {
                Log.LogInfo("update versions");
                UpdateVersion();    
            }
        }

        private void UpdateDistrictLastSync(District d, bool success)
        {
            if (success)
            {
                d.LastSync = DateTime.UtcNow;
                d.FailCounter = 0;    
            }
            else
                d.FailCounter = d.FailCounter + 1;
            ServiceLocatorMaster.DistrictService.Update(d);
        }

        private void ProcessPictures()
        {
            IList<int> ids = new List<int>();
            const int personsPerTask = 5000;
            for (int i = 0; i < personsForImportPictures.Count; i++ )
            {
                ids.Add(personsForImportPictures[i].PersonID);
                if (ids.Count >= personsPerTask || ids.Count > 0 && i + 1 == personsForImportPictures.Count)
                {
                    var data = new PictureImportTaskData(districtId, ids);
                    var domain = String.Format("picture processing for {0} at {1}", districtId, DateTime.UtcNow.Ticks);
                    ServiceLocatorMaster.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.PictureImport, DateTime.UtcNow, districtId, data.ToString(), domain);
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
            if (source.ContainsKey(id))
            {
                was.Add(id);
                var item = source[id];
                var r = reference(item);
                if (r.HasValue && !was.Contains(r.Value))
                    TopologicSort(r.Value, reference, source, was, res);
                res.Add(item);    
            }
        }
    }

    public class SisConnectionInfo
    {
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
    }
}
