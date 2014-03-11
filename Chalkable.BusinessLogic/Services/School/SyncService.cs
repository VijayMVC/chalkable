using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISyncService
    {
        IList<SyncVersion> GetVersions();
        void UpdateVersions(Dictionary<string, int> versions);
    }

    public class SyncService : SchoolServiceBase, ISyncService
    {
        public SyncService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<SyncVersion> GetVersions()
        {
            using (var uow = Read())
            {
                var da = new SyncVersionDataAccess(uow);
                return da.GetAll();
            }
        }

        public void UpdateVersions(Dictionary<string, int> versions)
        {
            using (var uow = Update())
            {
                var da = new SyncVersionDataAccess(uow);
                IList<SyncVersion> list = versions.Select(version => new SyncVersion {TableName = version.Key, Version = version.Value}).ToList();
                da.DeleteAll();
                da.Insert(list);
                uow.Commit();
            }
        }
    }
}