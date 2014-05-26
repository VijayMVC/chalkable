using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoDistrictStorage: BaseDemoGuidStorage<District>
    {
        public DemoDistrictStorage(DemoStorage storage)
            : base(storage, x => x.Id)
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
                Id = id,
                IsDemoDistrict = true,
                Name = "Demo District",
                TimeZone = "UTC"
            };
        }

        public override void Setup()
        {
            if (!Storage.Context.DistrictId.HasValue)
                throw new Exception("Context doesn't have valid district id");
            var districtId = Storage.Context.DistrictId.Value;
            data.Add(districtId, CreateDemoDistrict(districtId));
        }
    }
}
