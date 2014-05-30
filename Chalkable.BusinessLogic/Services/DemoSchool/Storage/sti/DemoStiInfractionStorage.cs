using System;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiInfractionStorage:BaseDemoIntStorage<Infraction>
    {
        public DemoStiInfractionStorage(DemoStorage storage)
            : base(storage, x => x.Id)
        {
        }
    }
}
