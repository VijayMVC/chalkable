using System;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiInfractionStorage:BaseDemoIntStorage<Infraction>
    {
        public DemoStiInfractionStorage()
            : base(x => x.Id)
        {
        }
    }
}
