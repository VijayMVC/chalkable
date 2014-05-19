using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoInfractionStorage:BaseDemoIntStorage<Infraction>
    {
        public DemoInfractionStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
        {
        }

        public IList<Infraction> GetAll(bool onlyActive)
        {
            var infractions = data.Select(x => x.Value);
            if (onlyActive)
                infractions = infractions.Where(x => x.IsActive == true);
            return infractions.ToList();
        }

        public override void Setup()
        {
            var infractions = new List<Infraction>();
            
            var infr1 = new Infraction
            {
                Code = "FI",
                Demerits = 0,
                Description = "Fighting",
                IsActive = true,
                IsSystem = false,
                Name = "Fighting"
            };

            var infr2 = new Infraction
            {
                Code = "DI",
                Demerits = 0,
                Description = "Disrespect",
                IsActive = true,
                IsSystem = false,
                Name = "Disrespect"
            };

            infractions.Add(infr1);
            infractions.Add(infr2);

            Add(infractions);

        }
    }
}
