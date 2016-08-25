using System;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.UserTracking;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IServiceLocatorMaster : IServiceLocator
    {
        IServiceLocatorSchool SchoolServiceLocator(Guid districtId, int? schoolLocalId);
        UserContext Context { get; }
        IUserService UserService { get; }
        ISchoolService SchoolService { get; }
        IDistrictService DistrictService { get; }
        IBackgroundTaskService BackgroundTaskService { get; }
        IPreferenceService PreferenceService { get; }
        IChalkableDepartmentService ChalkableDepartmentService { get; }
        IPersonPictureService PersonPictureService { get; }
        IPictureService DepartmentIconService { get; }
        IPictureService ApplicationPictureService { get; }
        IApplicationService ApplicationService { get; }
        ICategoryService CategoryService { get; }
        IApplicationUploadService ApplicationUploadService { get; }
        IEmailService EmailService { get; }
        IDeveloperService DeveloperService { get; }
        IDbService DbService { get; }
        IUserTrackingService UserTrackingService { get; }
        IDbMaintenanceService DbMaintenanceService { get; }
        IAcademicBenchmarkService AcademicBenchmarkService { get; }
        ICustomReportTemplateService CustomReportTemplateService { get; }
    }

    public class ServiceLocatorMaster : ServiceLocator, IServiceLocatorMaster
    {
        private IUserService userService;
        private ISchoolService schoolService;
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
        private IDeveloperService developerService;
        private IUserTrackingService userTrackingService;
        private IDbMaintenanceService dbMaintenanceService;
        private IAcademicBenchmarkService academicBenchmarkService;
        private ICustomReportTemplateService customReportTemplateService;

        public ServiceLocatorMaster(UserContext context) : base(context)
        {
            userService = new UserService(this);
            schoolService = new SchoolService(this);
            backgroundTaskService = new BackgroundTaskService(this);
            preferenceService = new PreferenceService(this);
            chalkableDepartmentService = new ChalkableDepartmentService(this);
            personPictureService = new PersonPictureService(this);
            departmentIconService = new DepartmentIconService(this);
            districtService = new DistrictService(this);
            applicationService = new ApplicationService(this);
            categoryService = new CategoryService(this);
            applicationUploadService = new ApplicationUploadService(this);
            EmailService = new EmailService(this);
            developerService = new DeveloperService(this);
            applicationPictureService = new ApplicationPictureService(this);
            DbService = new DbService(Context?.MasterConnectionString);
            userTrackingService = new MixPanelService(Settings.MixPanelToken);
            dbMaintenanceService = new DbMaintenanceService(this);
            academicBenchmarkService = new AcademicBenchmarkService(this);
            customReportTemplateService = new CustomReportTemplateService(this);
        }

        public IUserService UserService => userService;
        public ISchoolService SchoolService => schoolService;
        public IDistrictService DistrictService => districtService;
        public IBackgroundTaskService BackgroundTaskService => backgroundTaskService;
        public IPreferenceService PreferenceService => preferenceService;
        public IChalkableDepartmentService ChalkableDepartmentService => chalkableDepartmentService;
        public IPersonPictureService PersonPictureService => personPictureService;
        public IPictureService DepartmentIconService => departmentIconService;
        public IApplicationService ApplicationService => applicationService;
        public ICategoryService CategoryService => categoryService;
        public IApplicationUploadService ApplicationUploadService => applicationUploadService;
        public IEmailService EmailService { get; protected set; }
        public IDeveloperService DeveloperService => developerService;
        public IPictureService ApplicationPictureService => applicationPictureService;
        public IDbService DbService { get; protected set; }
        public IUserTrackingService UserTrackingService => userTrackingService;
        public IDbMaintenanceService DbMaintenanceService => dbMaintenanceService;
        public IAcademicBenchmarkService AcademicBenchmarkService => academicBenchmarkService;
        public ICustomReportTemplateService CustomReportTemplateService => customReportTemplateService;


        public virtual IServiceLocatorSchool SchoolServiceLocator(Guid districtId, int? schoolLocalId)
        {
            if (Context.DistrictId != districtId || Context.SchoolLocalId != schoolLocalId)
            {
                var district = DistrictService.GetByIdOrNull(districtId);
                var developer = DeveloperService.GetDeveloperByDictrict(district.Id);
                var developerId = developer != null ? developer.Id : (Guid?)null;
                Context.SwitchSchool(district.Id, district.Name, district.TimeZone, schoolLocalId, district.ServerUrl, developerId);
            }
            var serviceLocator = new ServiceLocatorSchool(this);
            return serviceLocator;
        }


    }
}
