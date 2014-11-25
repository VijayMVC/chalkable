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
    }
}
