using System;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiInfractionStorage:BaseDemoStorage<int, Infraction>
    {
        public DemoStiInfractionStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public override void Setup()
        {
            
        }
    }
}
