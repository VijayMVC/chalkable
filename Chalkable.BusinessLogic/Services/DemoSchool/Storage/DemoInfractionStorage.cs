using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoInfractionStorage:BaseDemoStorage<int, Infraction>
    {
        public DemoInfractionStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(IList<Infraction> infractions)
        {
            foreach (var infraction in infractions)
            {
                if (!data.ContainsKey(infraction.Id))
                {
                    data[infraction.Id] = infraction;
                }
            }
        }

        public void Update(IList<Infraction> infractions)
        {
            foreach (var infraction in infractions)
            {
                if (data.ContainsKey(infraction.Id))
                {
                    data[infraction.Id] = infraction;
                }
            }
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
                Id = GetNextFreeId(),
                IsActive = true,
                IsSystem = false,
                Name = "Fighting"
            };

            var infr2 = new Infraction
            {
                Code = "DI",
                Demerits = 0,
                Description = "Disrespect",
                Id = GetNextFreeId(),
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
