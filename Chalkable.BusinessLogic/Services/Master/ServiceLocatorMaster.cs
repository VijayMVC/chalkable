﻿using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IServiceLocatorMaster : IServiceLocator
    {
        IServiceLocatorSchool SchoolServiceLocator(Guid districtId, Guid? schoolId);
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
        IPictureService FundRequestPictureService { get; }
        IPictureService ApplicationPictureService { get; }
        IApplicationService ApplicationService { get; }
        ICategoryService CategoryService { get; }
        IApplicationUploadService ApplicationUploadService { get; }
        IAccessControlService AccessControlService { get; }
        IEmailService EmailService { get; }
        IFundService FundService { get; }
        IDeveloperService DeveloperService { get; }
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
        private IPictureService fundRequestPictureService;
        private IPictureService applicationPictureService;
        private IApplicationService applicationService;
        private ICategoryService categoryService;
        private IAccessControlService accessControlService;
        private IApplicationUploadService applicationUploadService;
        private IEmailService emailService;
        private IFundService fundService;
        private IDeveloperService developerService;

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
            applicationService = new ApplicationService(this);
            categoryService = new CategoryService(this);
            accessControlService = new AccessControlService(this);
            applicationUploadService = new ApplicationUploadService(this);
            emailService = new EmailService(this);
            fundService = new FundService(this);
            fundRequestPictureService = new FundRequestPictureService(this);
            developerService = new DeveloperService(this);
            applicationPictureService = new ApplicationPictureService(this);
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
        public IApplicationService ApplicationService { get { return applicationService; } }
        public ICategoryService CategoryService { get { return categoryService; } }
        public IApplicationUploadService ApplicationUploadService { get { return applicationUploadService; } }
        public IAccessControlService AccessControlService
        {
            get { return accessControlService; }
            protected set { accessControlService = value; }
        }
        public IEmailService EmailService
        {
            get { return emailService; } 
            protected set { emailService = value; }
        }

        public IFundService FundService { get { return fundService; } }
        public IDeveloperService DeveloperService { get { return developerService; } }
        public IPictureService FundRequestPictureService { get { return fundRequestPictureService; } }
        public IPictureService ApplicationPictureService { get { return applicationPictureService; } }

        public IServiceLocatorSchool SchoolServiceLocator(Guid districtId, Guid? schoolId)
        {
            if (Context.DistrictId != districtId || Context.SchoolId != schoolId)
            {
                var district = DistrictService.GetByIdOrNull(districtId);
                var schoolLocalId = schoolId.HasValue ? SchoolService.GetById(schoolId.Value).LocalId : (int?)null;
                var developer = DeveloperService.GetDeveloperByDictrict(district.Id);
                var developerId = developer != null ? developer.Id : (Guid?)null;
                Context.SwitchSchool(schoolId, district.Id, district.Name, district.TimeZone, schoolLocalId, district.ServerUrl, developerId);
            }
            var serviceLocator = new ServiceLocatorSchool(this);
            return serviceLocator;
        }

        public IServiceLocatorSchool SchoolServiceLocator(Guid schoolId)
        {
            var school = SchoolService.GetById(schoolId);
            return SchoolServiceLocator(school.DistrictRef, schoolId);
        }
        
    }
}
