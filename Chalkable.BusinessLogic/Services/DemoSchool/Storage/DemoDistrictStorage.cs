using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoDistrictStorage: BaseDemoStorage<Guid ,District>
    {
        public DemoDistrictStorage(DemoStorage storage)
            : base(storage)
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

        public PaginatedList<District> GetDistricts(int start, int count)
        {
            throw new NotImplementedException();
        }

        public IList<District> GetDistricts(bool? start, bool? usedDemo)
        {
            throw new NotImplementedException();
        }

        public void Update(District district)
        {
            throw new NotImplementedException();
        }

        public District GetByIdOrNull(Guid id)
        {
            return data.ContainsKey(id) ? data[id] : null;
        }

        public static District CreateDemoDistrict(Guid id)
        {
            return new District
            {
                DbName = "DemoDB",
                DemoPrefix = id.ToString(),
                Id = id,
                Name = "Demo District",
                TimeZone = "UTC"
            };
        }

        public void Setup()
        {
            var districtId = Storage.Context.DistrictId.Value;
            data.Add(districtId, CreateDemoDistrict(districtId));
        }
    }
}
