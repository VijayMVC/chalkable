using System;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IServiceLocatorMaster : IServiceLocator
    {
        IServiceLocatorSchool SchoolServiceLocator(Guid schoolId);
        UserContext Context { get; }
        IUserService UserService { get; }
        ISchoolService SchoolService { get; }
        IDistrictService DistrictService { get; }
        IBackgroundTaskService BackgroundTaskService { get; }
        IPreferenceService PreferenceService { get; }
        IChalkableDepartmentService ChalkableDepartmentService { get; }
        IPictureService PersonPictureService { get; }
        IPictureService CourseIconService { get; }
        IPictureService DepartmentIconService { get; }
    }

    public class ServiceLocatorMaster : ServiceLocator, IServiceLocatorMaster
    {
        private IUserService userService;
        private ISchoolService schoolService;
        private IDistrictService districtService;
        private IBackgroundTaskService backgroundTaskService;
        private IPreferenceService preferenceService;
        private IChalkableDepartmentService chalkableDepartmentService;
        private IPictureService personPictureService;
        private IPictureService courseIconService;
        private IPictureService departmentIconService;
        
        public ServiceLocatorMaster(UserContext context) : base(context)
        {
            userService = new UserService(this);
            schoolService = new SchoolService(this);
            backgroundTaskService = new BackgroundTaskService(this);
            preferenceService = new PreferenceService(this);
            chalkableDepartmentService = new ChalkableDepartmentService(this);
            personPictureService = new PersonPictureService(this);
            courseIconService = new CourseIconService(this);
            departmentIconService = new DepartmentIconService(this);
            districtService = new DistrictService(this);
        }

        public IUserService UserService { get { return userService; } }
        public ISchoolService SchoolService { get { return schoolService; } }
        public IDistrictService DistrictService { get { return districtService; } }
        public IBackgroundTaskService BackgroundTaskService { get { return backgroundTaskService; } }
        public IPreferenceService PreferenceService { get { return preferenceService; } }
        public IChalkableDepartmentService ChalkableDepartmentService { get { return chalkableDepartmentService; } }
        public IPictureService PersonPictureService { get { return personPictureService; } }
        public IPictureService CourseIconService { get { return courseIconService; } }
        public IPictureService DepartmentIconService { get { return departmentIconService; } }

        public IServiceLocatorSchool SchoolServiceLocator(Guid schoolId)
        {
            var school = SchoolService.GetById(schoolId);
            Context.SwitchSchool(schoolId, school.Name, school.TimeZone, school.ServerUrl);
            var serviceLocator = new ServiceLocatorSchool(this);
            return serviceLocator;
        }


        
    }
}
