using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using Chalkable.StiImport.Services.SyncModelAdapters;
using Newtonsoft.Json;
using School = Chalkable.StiConnector.SyncModel.School;
using User = Chalkable.Data.Master.Model.User;

namespace Chalkable.StiImport.Services
{
    public class ImportService
    {
        private SyncContext context;
        
        
        private ConnectorLocator connectorLocator;
        private UserContext sysadminCntx;

        private Guid districtId;
        private Guid taskId;
        protected IServiceLocatorMaster ServiceLocatorMaster { get; set; }
        protected IServiceLocatorSchool ServiceLocatorSchool { get; set; }
        protected BackgroundTaskService.BackgroundTaskLog Log { get; }
        protected SisConnectionInfo ConnectionInfo { get; }
        private IList<Guid> insertedUsers;
        private const int MAX_LOOGED_ENTITIES = 2000;

        public ImportService(Guid districtId, SisConnectionInfo connectionInfo, BackgroundTaskService.BackgroundTaskLog log)
        {
            ConnectionInfo = connectionInfo;
            Log = log;
            this.districtId = districtId;
            var admin = new User { Id = Guid.Empty, Login = "Virtual system admin", LoginInfo =  new UserLoginInfo()};
            sysadminCntx = new UserContext(admin, CoreRoles.SUPER_ADMIN_ROLE, null, null, null, null, null);
            
        }

        public ImportService(Guid districtId, SisConnectionInfo connectionInfo, BackgroundTaskService.BackgroundTaskLog log, Guid taskId) {
            // TODO: obviously we don't want duplicate code here            
            this.taskId = taskId;
            ConnectionInfo = connectionInfo;
            Log = log;
            this.districtId = districtId;
            var admin = new User { Id = Guid.Empty, Login = "Virtual system admin", LoginInfo = new UserLoginInfo() };
            sysadminCntx = new UserContext(admin, CoreRoles.SUPER_ADMIN_ROLE, null, null, null, null, null);

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
                d = ServiceLocatorMaster.DistrictService.GetByIdOrNull(ServiceLocatorSchool.Context.DistrictId.Value);
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
            var importedSchoolIds = context.GetSyncResult<School>().All.Select(x => x.SchoolID);
            foreach (var importedSchoolId in importedSchoolIds)
                connectorLocator.LinkConnector.CompleteSync(importedSchoolId);
            CreateUserLoginInfos();
            Log.LogInfo("import is completed");
        }

        public void ResyncAfterRestore()
        {
            Log.LogInfo("start resync after sti DB restore");
            ServiceLocatorMaster = new ServiceLocatorMaster(sysadminCntx);
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(districtId, null);
            if (!ServiceLocatorSchool.Context.DistrictId.HasValue)
                throw new Exception("District id should be defined for import");
            
            var d = ServiceLocatorMaster.DistrictService.GetByIdOrNull(ServiceLocatorSchool.Context.DistrictId.Value);
            connectorLocator = ConnectorLocator.Create(ConnectionInfo.SisUserName, ConnectionInfo.SisPassword, ConnectionInfo.SisUrl);
            Log.LogInfo("remove district last sync label");
            d.LastSync = null;
            ServiceLocatorMaster.DistrictService.Update(d);
            Log.LogInfo("performing before restore preparation");
            ServiceLocatorMaster.DbMaintenanceService.BeforeSisRestore(d.Id);
            ServiceLocatorSchool.DbMaintenanceService.BeforeSisRestore();
            Log.LogInfo("download data to restore");
            DownloadSyncData();
            Log.LogInfo("remove user records");
            var all = ServiceLocatorMaster.UserService.GetAll(districtId);
            var allIds = new HashSet<int>(all.Select(x => x.SisUserId.Value));
            var newUsers = new List<StiConnector.SyncModel.User>();
            var rows = context.GetSyncResult<StiConnector.SyncModel.User>().Rows;
            foreach (var r in rows)
                if (!allIds.Contains(r.UserID))
                    newUsers.Add(r);
            context.GetSyncResult<StiConnector.SyncModel.User>().Inserted = null;
            context.GetSyncResult<StiConnector.SyncModel.User>().Rows = newUsers.ToArray();
            context.GetSyncResult<StiConnector.SyncModel.User>().Updated = null;
            context.GetSyncResult<StiConnector.SyncModel.User>().Deleted = null;
            Log.LogInfo("do initial sync");
            DoInitialSync();
            d = ServiceLocatorMaster.DistrictService.GetByIdOrNull(ServiceLocatorSchool.Context.DistrictId.Value);
            Log.LogInfo("performing after restore preparation");
            var logs = ServiceLocatorMaster.DbMaintenanceService.AfterSisRestore(d.Id);
            ServiceLocatorSchool.DbMaintenanceService.AfterSisRestore();
            foreach (var restoreLogItem in logs)
            {
                Log.LogWarning(restoreLogItem.Msg);
            }
            Log.LogInfo("updating district last sync");
            UpdateDistrictLastSync(d, true);
            CreateUserLoginInfos();
            Log.LogInfo("resync after sti DB restore is completed");   
        }

        private void CreateUserLoginInfos()
        {
            if (insertedUsers != null && insertedUsers.Count > 0)
            {
                Log.LogInfo("creating user login infos");
                ServiceLocatorMaster.UserService.CreateUserLoginInfos(insertedUsers);
            }
        }

        public void Resync(string tableName)
        {
            Log.LogInfo("start resync");
            connectorLocator = ConnectorLocator.Create(ConnectionInfo.SisUserName, ConnectionInfo.SisPassword, ConnectionInfo.SisUrl);
            ServiceLocatorMaster = new ServiceLocatorMaster(sysadminCntx);
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(districtId, null);
            Log.LogInfo("download data to resync");
            context = new SyncContext();

            var tablesToResync = new List<string>
            {
                tableName,
                nameof(Gender),    // We need this tables for mappers
                nameof(SpEdStatus) // inside StudentAdapter/Selector
            };

            var toSync = context.TablesToSync;
            var results = new List<SyncResultBase<SyncModel>>();
            foreach (var table in toSync)
            {
                var type = context.Types[table.Key];
                SyncResultBase<SyncModel> res;
                var resType = (typeof(SyncResult<>)).MakeGenericType(type);
                if (tablesToResync.Contains(table.Key))
                {
                    Log.LogInfo("Start downloading " + table.Key);
                    res = (SyncResultBase<SyncModel>) connectorLocator.SyncConnector.GetDiff(type, null);
                    Log.LogInfo("Table downloaded: " + table.Key + " " + res.RowCount);
                }
                else
                    res = (SyncResultBase<SyncModel>)Activator.CreateInstance(resType, new object[0]);
                

                if (table.Key == tableName) //resync table
                {
                    object o = resType.GetProperty("Rows").GetValue(res);
                    resType.GetProperty("Updated").SetValue(res, o);
                    resType.GetProperty("Inserted").SetValue(res, null);
                    resType.GetProperty("Deleted").SetValue(res, null);
                    resType.GetProperty("Rows").SetValue(res, null);
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
                Log.LogException(ex, districtId.ToString(), taskId.ToString());
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
                Log.LogException(ex, districtId.ToString(), taskId.ToString());
                if (!schoolCommited)
                {
                    Log.LogError("rollback school db");
                    schoolDb.Rollback();
                }
                else
                    Log.LogError("!!!!!!!!school is commited but master is going to rollback!!!!!!!!!!!!!!!!!!!!!");
                //TODO.....
                Log.LogError("rollback master db");
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
            var adapterLocator = new AdapterLocator(ServiceLocatorMaster, ServiceLocatorSchool
                , context.GetSyncResult<Gender>().All
                , context.GetSyncResult<SpEdStatus>().All);

            List <SyncModelWrapper> models = new List<SyncModelWrapper>();
            foreach (var type in context.Types)
            {
                var fkProps = new List<PropertyInfo>();
                var flag = BindingFlags.Instance | BindingFlags.Public;
                var props = type.Value.GetProperties(flag);
                foreach (var propertyInfo in props)
                    if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetCustomAttribute<NullableForeignKey>() != null)
                    {
                        fkProps.Add(propertyInfo);
                    }

                SyncResultBase<SyncModel> sr = context.GetSyncResult(type.Value);
                foreach (var syncModel in sr.All)
                {
                    if (syncModel.SYS_CHANGE_VERSION == syncModel.SYS_CHANGE_CREATION_VERSION || fkProps.Count == 0)
                        models.Add(SyncModelWrapper.Create(syncModel.SYS_CHANGE_CREATION_VERSION ?? syncModel.SYS_CHANGE_VERSION,PersistOperationType.Insert, syncModel));
                    else
                    {
                        var iModel = syncModel.Clone();
                        foreach (var propertyInfo in fkProps)
                            propertyInfo.SetValue(iModel, null);
                        models.Add(SyncModelWrapper.Create(syncModel.SYS_CHANGE_CREATION_VERSION, PersistOperationType.Insert, iModel));
                        models.Add(SyncModelWrapper.Create(syncModel.SYS_CHANGE_VERSION, PersistOperationType.Update, syncModel));
                    }
                }
                
                if (sr.Updated != null)
                    models.AddRange(sr.Updated.Select(x => SyncModelWrapper.Create(x.SYS_CHANGE_VERSION, PersistOperationType.Update, x)));
                if (sr.Deleted != null)
                    foreach (var syncModel in sr.Deleted)
                    {
                        models.Add(SyncModelWrapper.Create(syncModel.SYS_CHANGE_VERSION, PersistOperationType.Delete, syncModel));
                        if (fkProps.Count != 0)
                            models.Add(SyncModelWrapper.Create(0, PersistOperationType.PrepareToDelete, syncModel));
                    }
            }
            models.Sort();
            IList<SyncModelWrapper> batch = new List<SyncModelWrapper>();
            for (int i = 0; i < models.Count; i++)
            {
                if (i > 0 && (models[i].Model.GetType().Name != models[i - 1].GetType().Name || models[i].OperationType != models[i - 1].OperationType))
                {
                    Persist(adapterLocator.GetAdapter(batch[0].Model.GetType()), batch[0].OperationType,
                        batch.Select(x => x.Model).ToList());
                    batch.Clear();
                }
                batch.Add(models[i]);
            }
            Persist(adapterLocator.GetAdapter(batch[0].Model.GetType()), batch[0].OperationType,
                        batch.Select(x => x.Model).ToList());

            insertedUsers = adapterLocator.InsertedUserIds;

            if (updateVersions)
            {
                Log.LogInfo("update versions");
                UpdateVersion();    
            }
        }

        private void Persist(ISyncModelAdapter adapter, PersistOperationType type, IList<SyncModel> entities)
        {
            if (entities.Count == 0)
                return;
            if (adapter == null)
            {
                Log.LogInfo($"No adapter for {entities[0].GetType().Name}");
                return;
            }
            var requestStartTime = DateTimeOffset.UtcNow;
            var requestTimer = Stopwatch.StartNew();
            try
            {
                adapter.Persist(type, entities);
            }
            catch (Exception)
            {
                Log.LogError($"Error durring persisting {entities[0].GetType().Name}");
                int cnt = 0;
                foreach (var entity in entities)
                {
                    var s = JsonConvert.SerializeObject(entity);
                    Log.LogError(s);
                    cnt++;
                    if (cnt >= MAX_LOOGED_ENTITIES)
                        break;
                }
                if (cnt < entities.Count)
                    Log.LogError($"only first {MAX_LOOGED_ENTITIES}  if {entities.Count} entities were logged");
                throw;
            }
            
            string typeName = type.ToString();
            var adapterName = adapter.GetType().Name;
            Log.LogInfo($"persist {typeName} by {adapterName}" );
            Telemetry.DispatchRequest(typeName, adapterName, requestStartTime, requestTimer.Elapsed, true, Verbosity.Info, districtId.ToString(), taskId.ToString());
        }

        private void UpdateDistrictLastSync(Data.Master.Model.District d, bool success)
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
            if (Settings.PictureProcessorCount > 0)
            {
                var personsForImportPictures = context.GetSyncResult<Person>().All.Where(person => person.PhotoModifiedDate.HasValue).ToList();
                if (context.GetSyncResult<Person>().Updated != null)
                    personsForImportPictures.AddRange(context.GetSyncResult<Person>().Updated.Where(person => person.PhotoModifiedDate.HasValue));
                IList<int> ids = new List<int>();
                const int personsPerTask = 5000;
                var random = new Random((int)(DateTime.Now.Ticks % 1000000000));
                for (int i = 0; i < personsForImportPictures.Count; i++)
                {
                    ids.Add(personsForImportPictures[i].PersonID);
                    if (ids.Count >= personsPerTask || ids.Count > 0 && i + 1 == personsForImportPictures.Count)
                    {
                        var data = new PictureImportTaskData(districtId, ids);
                        var domain = $"picture processing {random.Next(Settings.PictureProcessorCount)}";

                        ServiceLocatorMaster.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.PictureImport, DateTime.UtcNow, districtId, data.ToString(), domain);
                        ids.Clear();
                    }
                }    
            }
            else
                Log.LogWarning("picture Processing is disabled");
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
            var results = new List<SyncResultBase<SyncModel>>();
            foreach (var table in toSync)
            {
                var type = context.Types[table.Key];
                Log.LogInfo("Start downloading " + table.Key);
                
                Stopwatch requestTimer = Stopwatch.StartNew();
                var startTime = DateTimeOffset.UtcNow;
                var res = (SyncResultBase<SyncModel>)connectorLocator.SyncConnector.GetDiff(type, table.Value);
                Log.LogInfo("Table downloaded: " + table.Key + " " + res.RowCount);
                var details = new Dictionary<string, string>
                {
                    {"DistrictId", districtId.ToString()},
                    {"TaskId", taskId.ToString()},
                    {"RowCount", res.RowCount.ToString()},
                    {"Version", table.Value.ToString()}
                };

                Telemetry.DispatchRequest("Table download", table.Key, startTime, requestTimer.Elapsed, true, Verbosity.Info, details);
                Log.LogInfo("Table downloaded: " + table.Key);
                results.Add(res);
            }
            foreach (var syncResultBase in results)
            {
                context.SetResult(syncResultBase);   
            }
        }

        public void UpdateVersion()
        {
            var newVersions = new Dictionary<string, long>();
            foreach (var i in context.TablesToSync)
                if (i.Value.HasValue)
                {
                    newVersions.Add(i.Key, i.Value.Value);
                }
            ServiceLocatorSchool.SyncService.UpdateVersions(newVersions);
        }
    }

    public class SisConnectionInfo
    {
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
    }
}
