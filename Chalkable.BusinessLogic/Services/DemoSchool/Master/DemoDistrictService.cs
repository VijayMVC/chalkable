using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{

    public class DemoDistrictStorage : BaseDemoGuidStorage<District>
    {
        public DemoDistrictStorage()
            : base(x => x.Id)
        {
        }

        public District GetByIdOrNull(Guid id)
        {
            return data.ContainsKey(id) ? data[id] : null;
        }

        

        
    }

    public class DemoDistrictService : DemoMasterServiceBase, IDistrictService
    {
        public const int DEMO_EXPIRE_HOURS = 3;

        private DemoDistrictStorage DistrictStorage { get; set; }

        public DemoDistrictService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
            DistrictStorage = new DemoDistrictStorage();
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
                               string timeZone, string stateCode)
        {
            throw new NotImplementedException();
        }

        public District Create(Guid id, string name, string sisUrl, string sisRedirectUrl, string sisUserName, string sisPassword,
            string timeZone, string stateCode, bool isReportCardsEnabled)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<District> GetDistricts(int start = 0, int count = Int32.MaxValue)
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
            return DistrictStorage.GetByIdOrNull(id);
        }
 
        public bool IsOnline(Guid id)
        {
            return true;
        }

        public void UpdateReportCardsEnabled(Guid districtId, bool enabled)
        {
            throw new NotImplementedException();
        }

        public bool IsReportCardsEnabled()
        {
            throw new NotImplementedException();
        }

        public static District CreateDemoDistrict(Guid id)
        {
            return new District
            {
                Id = id,
                IsDemoDistrict = true,
                Name = "Demo District",
                TimeZone = "Central Standard Time"
            };
        }

        public void AddDistrict(UserContext userContext)
        {
            if (!userContext.DistrictId.HasValue)
                throw new Exception("Context doesn't have valid district id");
            var districtId = userContext.DistrictId.Value;
            var district = DemoDistrictService.CreateDemoDistrict(districtId);
            DistrictStorage.Add(district);
        }
    }
}