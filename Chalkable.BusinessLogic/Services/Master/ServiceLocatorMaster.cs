﻿using System;
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
        IPictureService FundRequestPictureService { get; }
        IPictureService ApplicationPictureService { get; }
        IApplicationService ApplicationService { get; }
        ICategoryService CategoryService { get; }
        IApplicationUploadService ApplicationUploadService { get; }
        IAccessControlService AccessControlService { get; }
        IEmailService EmailService { get; }
        IFundService FundService { get; }
        IDeveloperService DeveloperService { get; }
        IDbService DbService { get; }
        IUserTrackingService UserTrackingService { get; }
        ICommonCoreStandardService CommonCoreStandardService { get; }
        IDbMaintenanceService DbMaintenanceService { get; }
        IAcademicBenchmarkService AcademicBenchmarkService { get; }
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
        private IPictureService fundRequestPictureService;
        private IPictureService applicationPictureService;
        private IApplicationService applicationService;
        private ICategoryService categoryService;
        private IApplicationUploadService applicationUploadService;
        private IFundService fundService;
        private IDeveloperService developerService;
        private IUserTrackingService userTrackingService;
        private ICommonCoreStandardService commonCoreStandardService;
        private IDbMaintenanceService dbMaintenanceService;
        private IAcademicBenchmarkService academicBenchmarkService;

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
            AccessControlService = new AccessControlService(this);
            applicationUploadService = new ApplicationUploadService(this);
            EmailService = new EmailService(this);
            fundService = new FundService(this);
            fundRequestPictureService = new FundRequestPictureService(this);
            developerService = new DeveloperService(this);
            applicationPictureService = new ApplicationPictureService(this);
            commonCoreStandardService = new CommonCoreStandardService(this);
            DbService = new DbService(Context != null ? Context.MasterConnectionString : null);
            userTrackingService = new MixPanelService(Settings.MixPanelToken);
            dbMaintenanceService = new DbMaintenanceService(this);
            academicBenchmarkService = new AcademicBenchmarkService(this);
        }

        public IUserService UserService { get { return userService; } }
        public ISchoolService SchoolService { get { return schoolService; } }
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
        public IEmailService EmailService { get; protected set; }
        public IFundService FundService { get { return fundService; } }
        public IDeveloperService DeveloperService { get { return developerService; } }
        public IPictureService FundRequestPictureService { get { return fundRequestPictureService; } }
        public IPictureService ApplicationPictureService { get { return applicationPictureService; } }
        public IDbService DbService { get; protected set; }
        public IUserTrackingService UserTrackingService { get { return userTrackingService; } }
        public ICommonCoreStandardService CommonCoreStandardService { get { return commonCoreStandardService; } }
        public IDbMaintenanceService DbMaintenanceService {get { return dbMaintenanceService; }}
        public IAcademicBenchmarkService AcademicBenchmarkService { get { return academicBenchmarkService; } }


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
