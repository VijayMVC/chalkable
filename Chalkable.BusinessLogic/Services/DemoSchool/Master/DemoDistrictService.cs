using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
  
    public class DemoDistrictService : DemoMasterServiceBase, IDistrictService
    {
        public const int DEMO_EXPIRE_HOURS = 3;

        public DemoDistrictService(IServiceLocatorMaster serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
        {
        }

        public District Create(string name, string dbName, string sisUrl, string sisUserName, string sisPassword, string timeZone)
        {
            throw new NotImplementedException();
        }

        public District Create(string name, string sisUrl, string sisUserName, string sisPassword, string timeZone, Guid? sisDistrictId)
        {
            throw new NotImplementedException();
        }

        public District Create(Guid id, string name, string sisUrl, string sisUserName, string sisPassword, string timeZone)
        {
            throw new NotImplementedException();
        }

        public District Create(Guid id, string name, string sisUrl, string sisRedirectUrl, string sisUserName, string sisPassword,
                               string timeZone)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<District> GetDistricts(int start = 0, int count = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<DistrictSyncStatus> GetDistrictsSyncStatus(int start = 0, int count = Int32.MaxValue)
        {
            throw new NotImplementedException();
        }

        public IList<District> GetDistricts(bool? demo, bool? usedDemo = null)
        {
            throw new NotImplementedException();
        }

        public void Update(District district)
        {
            throw new NotImplementedException();
        }
        
        public District GetByIdOrNull(Guid id)
        {
            return Storage.DistrictStorage.GetByIdOrNull(id);
        }
 
        public void DeleteDistrict(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool IsOnline(Guid id)
        {
            return true;
        }
    }
}