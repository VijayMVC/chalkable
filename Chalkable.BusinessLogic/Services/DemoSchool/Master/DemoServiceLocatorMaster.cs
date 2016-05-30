using System;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.UserTracking;
using IDbMaintenanceService = Chalkable.BusinessLogic.Services.Master.IDbMaintenanceService;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{

    public class DemoServiceLocatorMaster : ServiceLocator, IServiceLocatorMaster
    {
        private IUserService userService;
        private Services.Master.ISchoolService schoolService;
        private IDistrictService districtService;
        private IBackgroundTaskService backgroundTaskService;
        private IPreferenceService preferenceService;
        private IChalkableDepartmentService chalkableDepartmentService;
        private IPersonPictureService personPictureService;
        private IPictureService departmentIconService;
        private IPictureService applicationPictureService;
        private IApplicationService applicationService;
        private ICategoryService categoryService;
        private IApplicationUploadService applicationUploadService;
        private IEmailService emailService;
        private IDeveloperService developerService;
        private IDbService dbService;
        private IUserTrackingService userTrackingService;

        public DemoServiceLocatorMaster(UserContext context)
            : base(context)
        {
            userService = new DemoUserService(this);
            schoolService = new DemoSchoolService(this);
            chalkableDepartmentService = new DemoChalkableDepartmentService(this);
            categoryService = new DemoCategoryService(this);
            developerService = new DemoDeveloperService(this);
            districtService = new DemoDistrictService(this);
            emailService = new DemoEmailService(this);
            backgroundTaskService = new DemoBackgroundTaskService(this);
            applicationService = new DemoApplicationService(this);
            applicationUploadService = new DemoApplicationUploadService(this);

            preferenceService = new PreferenceService(this);
            personPictureService = new PersonPictureService(this);
            departmentIconService = new DepartmentIconService(this);
            AccessControlService = new AccessControlService(this);
            applicationPictureService = new ApplicationPictureService(this);
            dbService = new DbService(Context != null ? Context.MasterConnectionString : null);
            userTrackingService = new NullTrackingService();
        }


        public void Update(UserContext context)
        {
            Context = context;
        }

        public IServiceLocatorSchool SchoolServiceLocator(Guid districtId, int? schoolLocalId)
        {
            throw new NotImplementedException();
        }

        public IUserService UserService { get { return userService; } }
        public Services.Master.ISchoolService SchoolService { get { return schoolService; } }
        public IDistrictService DistrictService { get { return districtService; } }
        public IBackgroundTaskService BackgroundTaskService { get { return backgroundTaskService; } }
        public IPreferenceService PreferenceService { get { return preferenceService; } }
        public IChalkableDepartmentService ChalkableDepartmentService { get { return chalkableDepartmentService; } }
        public IPersonPictureService PersonPictureService { get { return personPictureService; } }
        public IPictureService DepartmentIconService { get { return departmentIconService; } }
        public IApplicationService ApplicationService { get { return applicationService; } }
        public ICategoryService CategoryService { get { return categoryService; } }
        public IApplicationUploadService ApplicationUploadService { get { return applicationUploadService; } }
        public IAccessControlService AccessControlService { get; protected set; }

        public IEmailService EmailService
        {
            get { return emailService; } 
            protected set { emailService = value; }
        }
        
        public IDeveloperService DeveloperService { get { return developerService; } }
        public IDbService DbService 
        {
            get { return dbService; }
            protected set { dbService = value; }
        }

        public IUserTrackingService UserTrackingService { get { return userTrackingService;} }
        public IPictureService ApplicationPictureService { get { return applicationPictureService; } }
        public IDbMaintenanceService DbMaintenanceService { get{throw new NotImplementedException();} }
        public IAcademicBenchmarkService AcademicBenchmarkService { get{throw new NotImplementedException();} }
    }
}
