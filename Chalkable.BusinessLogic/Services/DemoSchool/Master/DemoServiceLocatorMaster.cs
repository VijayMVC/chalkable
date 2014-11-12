using System;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.UserTracking;

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
        private IPictureService courseIconService;
        private IPictureService departmentIconService;
        private IPictureService fundRequestPictureService;
        private IPictureService applicationPictureService;
        private IApplicationService applicationService;
        private ICategoryService categoryService;
        private IApplicationUploadService applicationUploadService;
        private IEmailService emailService;
        private IFundService fundService;
        private IDeveloperService developerService;
        private IDbService dbService;
        private IUserTrackingService userTrackingService;
        private ICommonCoreStandardService commonCoreStandardService;

        public DemoServiceLocatorMaster(UserContext context, DemoStorage storage)
            : base(context)
        {
            userService = new DemoUserService(this, storage);
            schoolService = new DemoSchoolService(this, storage);
            chalkableDepartmentService = new DemoChalkableDepartmentService(this, storage);
            categoryService = new DemoCategoryService(this, storage);
            developerService = new DemoDeveloperService(this, storage);
            districtService = new DemoDistrictService(this, storage);
            emailService = new DemoEmailService(this, storage);
            fundService = new DemoFundService(this, storage);
            backgroundTaskService = new DemoBackgroundTaskService(this);
            applicationService = new DemoApplicationService(this, storage);
            applicationUploadService = new DemoApplicationUploadService(this, storage);

            preferenceService = new PreferenceService(this);
            personPictureService = new PersonPictureService(this);
            courseIconService = new CourseIconService(this);
            departmentIconService = new DepartmentIconService(this);
            AccessControlService = new AccessControlService(this);
            fundRequestPictureService = new FundRequestPictureService(this);
            applicationPictureService = new ApplicationPictureService(this);
            commonCoreStandardService = new CommonCoreStandardService(this);
            dbService = new DbService(Context != null ? Context.MasterConnectionString : null);
            userTrackingService = new NullTrackingService();
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
        public IPictureService CourseIconService { get { return courseIconService; } }
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

        public IFundService FundService { get { return fundService; } }
        public IDeveloperService DeveloperService { get { return developerService; } }
        public IDbService DbService 
        {
            get { return dbService; }
            protected set { dbService = value; }
        }

        public IUserTrackingService UserTrackingService { get { return userTrackingService;} }
        public IPictureService FundRequestPictureService { get { return fundRequestPictureService; } }
        public IPictureService ApplicationPictureService { get { return applicationPictureService; } }
        public ICommonCoreStandardService CommonCoreStandardService { get { return commonCoreStandardService; } }
    }
}
