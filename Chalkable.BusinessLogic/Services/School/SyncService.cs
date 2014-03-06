using System;
using System.Collections.Generic;
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
                foreach (var version in versions)
                {
                    da.UpdateVersion(version.Key, version.Value);    
                }
                uow.Commit();
            }
        }
    }
}