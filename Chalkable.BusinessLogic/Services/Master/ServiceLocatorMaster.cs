using System;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IServiceLocatorMaster
    {
        IServiceLocatorSchool SchoolServiceLocator(Guid schoolId, string schoolName, string schoolServerUrl);
        UserContext Context { get; }
        IUserService UserService { get; }
        ISchoolService SchoolService { get; }
    }

    public class ServiceLocatorMaster : ServiceLocator, IServiceLocatorMaster
    {
        private IUserService userService;
        private ISchoolService schoolService;
        public ServiceLocatorMaster(UserContext context) : base(context)
        {
            userService = new UserService(this);
            schoolService = new SchoolService(this);
        }

        public IUserService UserService { get { return userService; } }
        public ISchoolService SchoolService { get { return schoolService; } }

        public IServiceLocatorSchool SchoolServiceLocator(Guid schoolId, string schoolName, string schoolServerUrl)
        {
            Context.SwitchSchool(schoolId, schoolName, schoolServerUrl);
            var serviceLocator = new ServiceLocatorSchool(this);
            return serviceLocator;
        }
    }
}
