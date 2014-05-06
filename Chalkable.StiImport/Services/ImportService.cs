﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
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
            importedSchoolIds.Clear();
            personsForImportPictures.Clear();
            connectorLocator = ConnectorLocator.Create(ConnectionInfo.SisUserName, ConnectionInfo.SisPassword, ConnectionInfo.SisUrl);
            DownloadSyncData();
            ProcessInsert();
            ProcessUpdate();
            ProcessPictures();
            ProcessDelete();
            foreach (var importedSchoolId in importedSchoolIds)
            {
                connectorLocator.LinkConnector.CompleteSync(importedSchoolId);
            }
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
                {
                    ServiceLocatorMaster.PersonPictureService.DeletePicture(ServiceLocatorSchool.Context.DistrictId.Value, person.PersonID);
                }
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
                Debug.WriteLine("Start downloading " + table.Key);
                var res = connectorLocator.SyncConnector.GetDiff(type, table.Value);
                Debug.WriteLine("Table downloaded: " + table.Key);
                results.Add((SyncResultBase)res);
            }
            foreach (var syncResultBase in results)
            {
                context.SetResult(syncResultBase);   
            }
            var newVersions = new Dictionary<string, int>();
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
        public string DbName { get; set; }
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
    }
}
