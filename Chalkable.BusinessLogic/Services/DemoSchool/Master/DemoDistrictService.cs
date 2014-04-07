using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

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
            return Storage.DistrictStorage.Create(name, dbName, sisUrl, sisUserName, sisPassword, timeZone);
        }

        public District Create(string name, string sisUrl, string sisUserName, string sisPassword, string timeZone, Guid? sisDistrictId)
        {
            return Storage.DistrictStorage.Create(name, sisUrl, sisUserName, sisPassword, timeZone, sisDistrictId);
        }

        public PaginatedList<District> GetDistricts(int start = 0, int count = int.MaxValue)
        {
            return Storage.DistrictStorage.GetDistricts(start, count);
        }

        public IList<District> GetDistricts(bool? demo, bool? usedDemo = null)
        {
            return Storage.DistrictStorage.GetDistricts(demo, usedDemo);
        }

        public void Update(District district)
        {
            Storage.DistrictStorage.Update(district);
        }
        
        public District GetByIdOrNull(Guid id)
        {
            return Storage.DistrictStorage.GetByIdOrNull(id);
        }
 
        public District UseDemoDistrict()
        {
           throw new NotImplementedException(); 
        }

        public void DeleteDistrict(Guid id)
        {
            Storage.DistrictStorage.Delete(id);
        }

        public bool IsOnline(Guid id)
        {
            return true;
        }
    }
}