using System;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IServiceLocatorMaster
    {
        IServiceLocatorSchool SchoolServiceLocator(Guid schoolId);
        UserContext Context { get; }
        IUserService UserService { get; }
        ISchoolService SchoolService { get; }
        IBackgroundTaskService BackgroundTaskService { get; }
        IPreferenceService PreferenceService { get; }
    }

    public class ServiceLocatorMaster : ServiceLocator, IServiceLocatorMaster
    {
        private IUserService userService;
        private ISchoolService schoolService;
        private IBackgroundTaskService backgroundTaskService;
        private IPreferenceService preferenceService;

        public ServiceLocatorMaster(UserContext context) : base(context)
        {
            userService = new UserService(this);
            schoolService = new SchoolService(this);
            backgroundTaskService = new BackgroundTaskService(this);
            preferenceService = new PreferenceService(this);
        }

        public IUserService UserService { get { return userService; } }
        public ISchoolService SchoolService { get { return schoolService; } }
        public IBackgroundTaskService BackgroundTaskService { get { return backgroundTaskService; } }
        public IPreferenceService PreferenceService { get { return preferenceService; } }

        public IServiceLocatorSchool SchoolServiceLocator(Guid schoolId)
        {
            var school = SchoolService.GetById(schoolId);
            Context.SwitchSchool(schoolId, school.Name, school.TimeZone, school.ServerUrl);
            var serviceLocator = new ServiceLocatorSchool(this);
            return serviceLocator;
        }
    }
}
