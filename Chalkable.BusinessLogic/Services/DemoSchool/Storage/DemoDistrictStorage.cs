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

        public override void Setup()
        {
            var districtId = Storage.Context.DistrictId.Value;
            data.Add(districtId, CreateDemoDistrict(districtId));
        }
    }
}
