using System;
using System.Collections.Generic;
using System.Text;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IApplicationUploadService
    {
        Application Create(BaseApplicationInfo appInfo);
        Application UpdateDraft(Guid applicationId, BaseApplicationInfo appInfo);
        Application Submit(Guid applicationId, BaseApplicationInfo appInfo);
        bool ApproveReject(Guid applicationId, bool isApprove);
        bool GoLive(Guid applicationId);
        bool UnList(Guid applicationId);
        bool Exists(Guid? currentApplicationId, string name, string url);
        bool DeleteApplication(Guid id);
        void ChangeApplicationType(Guid applicationId, bool isInternal);
        void SetApplicationInternalData(Guid applicationId, int? internalScore, string internalDescription);
    }

    public class ApplicationUploadService : MasterServiceBase, IApplicationUploadService
    {
        public ApplicationUploadService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        private Application EditApplication(Application application, BaseApplicationInfo applicationInfo, bool addToOauth = false,
                                            ApplicationStateEnum state = ApplicationStateEnum.Draft)
        {
            if (application.DeveloperRef == Guid.Empty)
                application.DeveloperRef = applicationInfo.DeveloperId;
            
            if (!ApplicationSecurity.CanUploadApplication(Context) || !ApplicationSecurity.CanEditApplication(Context, application))
                throw new ChalkableSecurityException(ChlkResources.ERR_APP_INVALID_RIGHTS);

            if (applicationInfo.ApplicationPrices.Price < 0 || applicationInfo.ApplicationPrices.PricePerClass < 0
               || applicationInfo.ApplicationPrices.PricePerSchool < 0)
                throw new ChalkableException(ChlkResources.ERR_APP_INVALID_PRICE);
            var newAppName = applicationInfo.ShortApplicationInfo.Name;
            var newAppUrl = applicationInfo.ShortApplicationInfo.Url ?? "";
            if (state != ApplicationStateEnum.Live)
            {
                if (Exists(application.Id, newAppName, null))
                    throw new ChalkableException(ChlkResources.ERR_APP_DUPLICATE_NAME);
                if (addToOauth && Exists(application.Id, null, newAppUrl))
                    throw new ChalkableException(ChlkResources.ERR_APP_DUPLICATE_URL);
            }

            application.Name = newAppName;
            application.Url = newAppUrl;
            application.ShortDescription = applicationInfo.ShortApplicationInfo.ShortDescription;
            application.Description = applicationInfo.ShortApplicationInfo.Description;
            application.SmallPictureRef = applicationInfo.ShortApplicationInfo.SmallPictureId;
            application.BigPictureRef = applicationInfo.ShortApplicationInfo.BigPictureId;
            application.ExternalAttachPictureRef = applicationInfo.ShortApplicationInfo.ExternalAttachPictureId;
            application.Description = applicationInfo.ShortApplicationInfo.Description;
            application.VideoUrl = applicationInfo.ShortApplicationInfo.VideoDemoUrl;
            application.IsAdvanced = applicationInfo.ShortApplicationInfo.AdvancedApp;
            
            application.Price = applicationInfo.ApplicationPrices.Price;
            application.PricePerClass = applicationInfo.ApplicationPrices.PricePerClass;
            application.PricePerSchool = applicationInfo.ApplicationPrices.PricePerSchool;

            application.HasStudentMyApps = applicationInfo.ApplicationAccessInfo.HasStudentMyApps;
            application.HasTeacherMyApps = applicationInfo.ApplicationAccessInfo.HasTeacherMyApps;
            application.HasParentMyApps = applicationInfo.ApplicationAccessInfo.HasParentMyApps;
            application.HasAdminMyApps = applicationInfo.ApplicationAccessInfo.HasAdminMyApps;
            application.CanAttach = applicationInfo.ApplicationAccessInfo.CanAttach;
            application.ShowInGradeView = applicationInfo.ApplicationAccessInfo.ShowInGradeView;
            application.HasAdminExternalAttach = applicationInfo.ApplicationAccessInfo.HasAdminExternalAttach;
            application.HasTeacherExternalAttach = applicationInfo.ApplicationAccessInfo.HasTeacherExternalAttach;
            application.HasStudentExternalAttach = applicationInfo.ApplicationAccessInfo.HasStudentExternalAttach;
            application.HasSysAdminSettings = applicationInfo.ApplicationAccessInfo.HasSysAdminSettings;
            application.HasDistrictAdminSettings = applicationInfo.ApplicationAccessInfo.HasDistrictAdminSettings;
            application.HasStudentProfile = applicationInfo.ApplicationAccessInfo.HasStudentProfile;
            application.ProvidesRecommendedContent = applicationInfo.ApplicationAccessInfo.ProvidesRecommendedContent;

            application.State = state;

            using (var uow = Update())
            {
                var da = new ApplicationDataAccess(uow);
                
                if (application.Id == Guid.Empty)
                {
                    application.Id = Guid.NewGuid();
                    application.CreateDateTime = DateTime.UtcNow;
                    da.Insert(application);
                }
                else da.Update(application);

                application.Categories = da.UpdateCategories(application.Id, applicationInfo.Categories);
                application.Pictures = da.UpdatePictures(application.Id, applicationInfo.PicturesId);
                application.GradeLevels = da.UpdateGradeLevels(application.Id, applicationInfo.GradeLevels);
                application.Permissions = da.UpdatePermissions(application.Id, applicationInfo.PermissionIds);
                application.ApplicationStandards = da.UpdateApplicationStandards(application.Id, applicationInfo.StandardsIds);
                uow.Commit();
            }
            application.Developer = ServiceLocator.DeveloperService.GetById(application.DeveloperRef);
            return application;
        }
        private string GenerateSecretKey()
        {
            var builder = new StringBuilder();
            builder.Append(Guid.NewGuid().ToString().Replace("-", ""));
            builder.Append(Guid.NewGuid().ToString().Replace("-", ""));
            builder.Append(Guid.NewGuid().ToString().Replace("-", ""));
            builder.Append(Guid.NewGuid().ToString().Replace("-", ""));
            return builder.ToString();
        }

        public Application Create(BaseApplicationInfo appInfo)
        {
            var application = new Application { SecretKey = GenerateSecretKey(), Id = Guid.Empty};
            application = EditApplication(application, appInfo);
            return application;
        }

        public Application UpdateDraft(Guid applicationId, BaseApplicationInfo appInfo)
        {
            Application application;
            using (var uow = Read())
            {
                var da = new ApplicationDataAccess(uow);
                application = da.GetApplicationById(applicationId);
            }
            if (application.IsLive)
                throw  new ChalkableSecurityException("Only draft applications can be updated");
            if (string.IsNullOrEmpty(appInfo.ShortApplicationInfo.Name))
                throw new ChalkableException(ChlkResources.ERR_APP_NAME_MISSING);
            if (string.IsNullOrEmpty(appInfo.ShortApplicationInfo.Url))
                throw new ChalkableException(ChlkResources.ERR_APP_URL_MISSING);
            application = EditApplication(application, appInfo, true);
            return application;
        }

        public Application Submit(Guid applicationId, BaseApplicationInfo appInfo)
        {
            Application application;
            using (var uow = Read())
            {
                var da = new ApplicationDataAccess(uow);
                application = da.GetApplicationById(applicationId);
            }
            var shortAppInfo = appInfo.ShortApplicationInfo;
            if (application.IsLive)
                throw new ChalkableSecurityException("Only draft applications can't be submitted");
            if (string.IsNullOrEmpty(shortAppInfo.Name))
                throw new ChalkableException(ChlkResources.ERR_APP_NAME_MISSING);
            if (string.IsNullOrEmpty(shortAppInfo.Url))
                throw new ChalkableException(ChlkResources.ERR_APP_URL_MISSING);
            if (string.IsNullOrEmpty(shortAppInfo.ShortDescription))
                throw new ChalkableException(ChlkResources.ERR_APP_SHORT_DESCRIPTION_MISSING);
            if (string.IsNullOrEmpty(shortAppInfo.Description))
                throw new ChalkableException(ChlkResources.ERR_APP_LONG_DESCRIPTION_MISSING);


            if (appInfo.ApplicationAccessInfo.CanAttach)
            {
                if (!(shortAppInfo.BigPictureId.HasValue && shortAppInfo.SmallPictureId.HasValue))
                    throw new ChalkableException(ChlkResources.ERR_APP_ICONS_MISSING);    
            }
            else
            {
                if (!(shortAppInfo.SmallPictureId.HasValue))
                    throw new ChalkableException(ChlkResources.ERR_APP_SMALL_ICON_MISSING);        
            }

            if ((appInfo.ApplicationAccessInfo.HasStudentExternalAttach
                || appInfo.ApplicationAccessInfo.HasTeacherExternalAttach
                || appInfo.ApplicationAccessInfo.HasAdminExternalAttach)
                && !appInfo.ShortApplicationInfo.ExternalAttachPictureId.HasValue)
            {
                throw new ChalkableException(ChlkResources.ERR_APP_ATTACH_ICON_MISSING);
            }
            var developer = application.Developer;
            if (string.IsNullOrEmpty(developer.Name) || string.IsNullOrEmpty(developer.WebSite))
                throw new ChalkableException(ChlkResources.ERR_APP_DEV_INFO_MISSING);

            application = EditApplication(application, appInfo, true, ApplicationStateEnum.SubmitForApprove);
            ServiceLocator.EmailService.SendApplicationEmailToSysadmin(application); 
            return application;    
        }
        
        public bool ApproveReject(Guid applicationId, bool isApprove)
        {
            if (!BaseSecurity.IsSysAdmin(Context) && !BaseSecurity.IsAppTester(Context))
                throw new ChalkableSecurityException();
            Application application;
            using (var uow = Read())
            {
                var da = new ApplicationDataAccess(uow);
                application = da.GetApplicationById(applicationId);
            }
            if (application.State == ApplicationStateEnum.SubmitForApprove)
            {
                User person = application.Developer.User;
                person.ConfirmationKey = Guid.NewGuid().ToString().Replace("-", "");
                application.State = isApprove ? ApplicationStateEnum.Approved : ApplicationStateEnum.Rejected;
                using (var uow = Update())
                {
                    new UserDataAccess(uow).Update(person);
                    var da = new ApplicationDataAccess(uow);
                    da.Update(application);
                    uow.Commit();
                }
                ServiceLocator.EmailService.SendApplicationEmailToDeveloper(application);
                return true;
            }
            return false;
        }

        public bool GoLive(Guid applicationId)
        {
            Application application;
            using (var uow = Update())
            {
                var da = new ApplicationDataAccess(uow);
                application = da.GetApplicationById(applicationId);
                if (!ApplicationSecurity.CanEditApplication(Context, application))
                    throw new ChalkableSecurityException(ChlkResources.ERR_APP_INVALID_RIGHTS);

                if (application.State == ApplicationStateEnum.Approved)
                {
                    var appInfo = BaseApplicationInfo.Create(application);
                    Application orignApplication;
                    if (application.OriginalRef.HasValue)
                        orignApplication = da.GetApplicationById(application.OriginalRef.Value);
                    else
                    {
                        orignApplication = new Application { SecretKey = application.SecretKey, Id = Guid.Empty };
                        application.CreateDateTime = DateTime.UtcNow;
                    }                       
                    orignApplication = EditApplication(orignApplication, appInfo, true, ApplicationStateEnum.Live);
                    application.State = ApplicationStateEnum.Draft;
                    if (!application.OriginalRef.HasValue)
                        application.OriginalRef = orignApplication.Id;
                    da.Update(application);
                    uow.Commit();
                    return true;
                }
            }
            return false;
        }

        public bool UnList(Guid applicationId)
        {
            Application application;
            using (var uow = Read())
            {
                var da = new ApplicationDataAccess(uow);
                application = da.GetApplicationById(applicationId);
            }
            if (!ApplicationSecurity.CanEditApplication(Context, application))
                throw new ChalkableSecurityException(ChlkResources.ERR_APP_INVALID_RIGHTS);
            if (application.State == ApplicationStateEnum.Live)
            {
                DeleteApplication(application.Id);
                return true;
            }
            return false;
        }

        public bool Exists(Guid? currentApplicationId, string name, string url)
        {
            using (var uow = Read())
            {
                var da = new ApplicationDataAccess(uow);
                return da.AppExists(currentApplicationId, name, url);
            }
        }

        public bool DeleteApplication(Guid id)
        {
            Application application;
            IList<Application> draftApps;
            using (var uow = Update())
            {
                var da = new ApplicationDataAccess(uow);
                application = da.GetApplicationById(id);
                if (!ApplicationSecurity.CanEditApplication(Context, application))
                    throw new ChalkableSecurityException(ChlkResources.ERR_APP_INVALID_RIGHTS);

                if (application.OriginalRef.HasValue)
                {
                    var orginalAppId = application.OriginalRef.Value;
                    application.OriginalRef = null;
                    da.Update(application);
                    da.Delete(orginalAppId);
                }
                else
                {
                    draftApps = da.GetAll(new AndQueryCondition { { nameof(Application.OriginalRef), id } });
                    foreach (var draftApp in draftApps)
                    {
                        draftApp.OriginalRef = null;
                        da.Update(draftApp);
                    }
                }
                da.Delete(id);
                uow.Commit();
            }
            return true;
        }

        public void ChangeApplicationType(Guid applicationId, bool isInternal)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new ApplicationDataAccess(uow);
                var application = da.GetApplicationById(applicationId);
               
                if(!application.IsLive)
                    throw new ChalkableException("Only live application can be internal");
                application.IsInternal = isInternal;
                da.Update(application);
                uow.Commit();
            }
        }

        public void SetApplicationInternalData(Guid applicationId, int? internalScore, string internalDescription)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            
            if(internalScore.HasValue && (internalScore.Value < 0 || internalScore.Value > 100))
                throw new ChalkableException("Internal score out of range. Internal score should be in range [0,100]");
            
            using (var uow = Update())
            {
                var da = new ApplicationDataAccess(uow);
                var app = da.GetApplicationById(applicationId);
                if(!app.IsLive)
                    throw new ChalkableException("Only live application can have internal data");
                app.InternalScore = internalScore;
                app.InternalDescription = internalDescription;
                da.Update(app);
                uow.Commit();
            }
        }
    }
}