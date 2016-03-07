using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Master.PictureServices;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.UserTracking;
using IDbMaintenanceService = Chalkable.BusinessLogic.Services.Master.IDbMaintenanceService;
using ISchoolService = Chalkable.BusinessLogic.Services.Master.ISchoolService;

namespace Chalkable.StandardImport.Services
{
    public class StandardImportServiceLocatorMaster : IServiceLocatorMaster
    {
        public StandardImportServiceLocatorMaster(string connectionString)
        {
            DbService = new DbService(connectionString);
            CommonCoreStandardService = new CommonCoreStandardService(this);
        }


        public IStorageBlobService StorageBlobService { get; private set; }
        public ICrocodocService CrocodocService { get; private set; }
        public IServiceLocatorSchool SchoolServiceLocator(Guid districtId, int? schoolLocalId)
        {
            throw new NotImplementedException();
        }

        public UserContext Context { get; private set; }
        public IUserService UserService { get; private set; }
        public ISchoolService SchoolService { get; private set; }
        public IDistrictService DistrictService { get; private set; }
        public IBackgroundTaskService BackgroundTaskService { get; private set; }
        public IPreferenceService PreferenceService { get; private set; }
        public IChalkableDepartmentService ChalkableDepartmentService { get; private set; }
        public IPersonPictureService PersonPictureService { get; private set; }
        public IPictureService DepartmentIconService { get; private set; }
        public IPictureService FundRequestPictureService { get; private set; }
        public IPictureService ApplicationPictureService { get; private set; }
        public IApplicationService ApplicationService { get; private set; }
        public ICategoryService CategoryService { get; private set; }
        public IApplicationUploadService ApplicationUploadService { get; private set; }
        public IAccessControlService AccessControlService { get; private set; }
        public IEmailService EmailService { get; private set; }
        public IFundService FundService { get; private set; }
        public IDeveloperService DeveloperService { get; private set; }
        public IDbService DbService { get; private set; }
        public IUserTrackingService UserTrackingService { get; private set; }
        public ICommonCoreStandardService CommonCoreStandardService { get; private set; }
        public IDbMaintenanceService DbMaintenanceService { get; private set; }
        public IAcademicBenchmarkService AcademicBenchmarkService { get; private set; }
    }
    
}
