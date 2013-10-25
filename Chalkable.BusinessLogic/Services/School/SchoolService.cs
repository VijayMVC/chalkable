namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolService
    {

    }

    public class SchoolService : SchoolServiceBase, ISchoolService
    {
        public SchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
    }
}