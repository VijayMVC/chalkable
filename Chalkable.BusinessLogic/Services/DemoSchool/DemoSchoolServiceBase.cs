using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoSchoolServiceBase: SchoolServiceBase
    {
        public DemoSchoolServiceBase(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
    }
}
