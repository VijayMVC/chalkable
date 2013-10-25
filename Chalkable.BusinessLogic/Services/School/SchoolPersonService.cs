namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolPersonService
    {
        
    }

    public class SchoolPersonService : SchoolServiceBase, ISchoolPersonService
    {
        public SchoolPersonService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }
    }
}