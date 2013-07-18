﻿using System;
using System.Text;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
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
    }

    public class ApplicationUploadService : MasterServiceBase, IApplicationUploadService
    {
        public ApplicationUploadService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        private Application EditApplication(Application application, BaseApplicationInfo applicationInfo, bool addToOauth = false,
                                            ApplicationStateEnum state = ApplicationStateEnum.Draft)
        {
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
            application.Description = applicationInfo.ShortApplicationInfo.Description;
            application.CreateDateTime = DateTime.UtcNow;
            application.VideoUrl = applicationInfo.ShortApplicationInfo.VideoDemoUrl;
            application.DeveloperRef = applicationInfo.DeveloperId;

            application.Price = applicationInfo.ApplicationPrices.Price;
            application.PricePerClass = applicationInfo.ApplicationPrices.PricePerClass;
            application.PricePerSchool = applicationInfo.ApplicationPrices.PricePerSchool;
            application.HasStudentMyApps = applicationInfo.ApplicationAccessInfo.HasStudentMyApps;
            application.HasTeacherMyApps = applicationInfo.ApplicationAccessInfo.HasTeacherMyApps;
            application.HasParentMyApps = applicationInfo.ApplicationAccessInfo.HasParentMyApps;
            application.HasAdminMyApps = applicationInfo.ApplicationAccessInfo.HasAdminMyApps;
            application.CanAttach = applicationInfo.ApplicationAccessInfo.CanAttach;
            application.ShowInGradeView = applicationInfo.ApplicationAccessInfo.ShowInGradeView;
            application.State = state;

            using (var uow = Update())
            {
                var da = new ApplicationDataAccess(uow);
                
                if (application.Id == Guid.Empty)
                {
                    application.Id = Guid.NewGuid();
                    da.Insert(application);
                }
                else
                    da.Update(application);

                da.UpdateCategories(application.Id, applicationInfo.Categories);
                da.UpdatePictures(application.Id, applicationInfo.PicturesId);
                da.UpdateGradeLevels(application.Id, applicationInfo.GradeLevels);
                da.UpdatePermissions(application.Id, applicationInfo.ShortApplicationInfo.PermissionIds);
                if (addToOauth)
                {
                    if (!string.IsNullOrEmpty(application.Url))
                    {
                        var clientId = application.Url;
                        var appName = application.Name;
                        if (ServiceLocator.AccessControlService.GetApplication(clientId) == null)
                            ServiceLocator.AccessControlService.RegisterApplication(clientId, application.SecretKey, application.Url, appName);
                        else
                        {
                            ServiceLocator.AccessControlService.RemoveApplication(clientId);
                            ServiceLocator.AccessControlService.RegisterApplication(clientId, application.SecretKey, application.Url, appName);
                            //todo: looks like applicationRegistrationService.UpdateApplicationRedirectUri() doesn't work :(
                        }
                    }
                }
                uow.Commit();
            }
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
            if (string.IsNullOrEmpty(shortAppInfo.Name))
                throw new ChalkableException(ChlkResources.ERR_APP_NAME_MISSING);
            if (string.IsNullOrEmpty(shortAppInfo.Url))
                throw new ChalkableException(ChlkResources.ERR_APP_URL_MISSING);
            if (string.IsNullOrEmpty(shortAppInfo.ShortDescription))
                throw new ChalkableException(ChlkResources.ERR_APP_SHORT_DESCRIPTION_MISSING);
            if (string.IsNullOrEmpty(shortAppInfo.Description))
                throw new ChalkableException(ChlkResources.ERR_APP_LONG_DESCRIPTION_MISSING);
            if (!(shortAppInfo.BigPictureId.HasValue && shortAppInfo.SmallPictureId.HasValue))
                throw new ChalkableException(ChlkResources.ERR_APP_ICONS_MISSING);

            var developer = application.Developer;
            if (string.IsNullOrEmpty(developer.Name) || string.IsNullOrEmpty(developer.WebSite))
                throw new ChalkableException(ChlkResources.ERR_APP_DEV_INFO_MISSING);

            application = EditApplication(application, appInfo, true, ApplicationStateEnum.SubmitForApprove);
            //ServiceLocator.EmailService.SendApplicationEmailToSysadmin(application); TODO: need mail service
            return application;    
        }

        public bool ApproveReject(Guid applicationId, bool isApprove)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
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
                //ServiceLocator.EmailService.SendApplicationEmailToDeveloper(application); //TODO: need mail service. think about this
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
                    var orignApplication = application.OriginalRef.HasValue
                                               ? da.GetApplicationById(application.OriginalRef.Value)
                                               : new Application { SecretKey = application.SecretKey, Id = Guid.Empty};

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
            throw new System.NotImplementedException();
        }

        public bool Exists(Guid? currentApplicationId, string name, string url)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteApplication(Guid id)
        {
            throw new System.NotImplementedException();
        }

        public void ChangeApplicationType(Guid applicationId, bool isInternal)
        {
            throw new System.NotImplementedException();
        }
    }
}