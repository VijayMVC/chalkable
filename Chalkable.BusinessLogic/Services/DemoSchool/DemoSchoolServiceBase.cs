using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoSchoolServiceBase: SchoolServiceBase
    {

        public DemoStorage Storage { get; private set; }
        public DemoSchoolServiceBase(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator)
        {
            Storage = demoStorage;
        }
    }
}
