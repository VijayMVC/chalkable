using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using School = Chalkable.StiConnector.SyncModel.School;

namespace Chalkable.StiImport.Services
{
    public partial class ImportService
    {
        private SyncContext context;
        private const string USER_EMAIL_FMT = "user{0}_{1}@chalkable.com";
        private const string DEF_USER_PASS = "Qwerty1@";
        private const string DESCR_WORK = "Work";
        private const string DESCR_CELL = "cell";
        private const string IMG = "image";
        private const string UNKNOWN_ROOM_NUMBER = "Unknown number";

        protected IServiceLocatorMaster ServiceLocatorMaster { get; set; }
        protected IServiceLocatorSchool ServiceLocatorSchool { get; set; }
        protected BackgroundTaskService.BackgroundTaskLog Log { get; private set; }
        protected SisConnectionInfo ConnectionInfo { get; private set; }

        public ImportService(Guid districtId, SisConnectionInfo connectionInfo, BackgroundTaskService.BackgroundTaskLog log)
        {
            ConnectionInfo = connectionInfo;
            Log = log;
            ServiceLocatorMaster = ServiceLocatorFactory.CreateMasterSysAdmin();
            ServiceLocatorSchool = ServiceLocatorMaster.SchoolServiceLocator(districtId, null);    
        }

        public void Import()
        {
            DownloadSyncData();
            ProcessInsert();
            ProcessUpdate();
            ProcessDelete();
        }
        
        public void DownloadSyncData()
        {
            context = new SyncContext();
            context.SetCurrentVersions(ServiceLocatorSchool.SyncService.GetVersions());
            var toSync = context.TablesToSync;
            var results = new List<SyncResultBase>();
            var cl = ConnectorLocator.Create(ConnectionInfo.SisUserName, ConnectionInfo.SisPassword, ConnectionInfo.SisUrl);
            foreach (var table in toSync)
            {
                var type = context.Types[table.Key];
                var res = cl.SyncConnector.GetDiff(type, table.Value);
                results.Add((SyncResultBase)res);
            }
            foreach (var syncResultBase in results)
            {
                context.SetResult(syncResultBase);   
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
