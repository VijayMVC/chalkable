using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoMasterServiceBase: MasterServiceBase
    {
        public DemoStorage Storage { get; private set; }
        public DemoMasterServiceBase(IServiceLocatorMaster serviceLocator, DemoStorage storage):base(serviceLocator)
        {
            Storage = storage;
        }

    }
}
