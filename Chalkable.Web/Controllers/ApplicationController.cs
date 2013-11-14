using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ApplicationController : AnnouncementBaseController //TODO: think about this
    {
        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult List(int? start, int? count)
        {
            var applications = MasterLocator.ApplicationService.GetApplications(start ?? 0, count ?? DEFAULT_PAGE_SIZE, false);
            return Json(applications.Transform(BaseApplicationViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult Create(Guid developerId, string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(new ChalkableException(ChlkResources.ERR_APP_NAME_MISSING));
            if (MasterLocator.ApplicationUploadService.Exists(null, name, null))
                return Json(new ChalkableException(ChlkResources.ERR_APP_DUPLICATE_NAME));
            var appInfo = BaseApplicationInfo.Create(new ShortApplicationInfo {Name = name}, developerId);
            var application = MasterLocator.ApplicationUploadService.Create(appInfo);
           
            //if (application != null) //TODO: mix panel
            //{
            //    MixPanelService.CreatedApp(SchoolLocator.Context.UserName, application.Name);
            //}
            return PrepareAppInfo(application, true, true);
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult Update(ApplicationUpdateInputModel applicationInputModel)
        {
            var shortAppinfo = applicationInputModel.ShortApplicationInfo;
            var appId = applicationInputModel.ApplicationId;
            if (string.IsNullOrEmpty(shortAppinfo.Name))
                return Json(new ChalkableException(ChlkResources.ERR_APP_NAME_MISSING));
            if (string.IsNullOrEmpty(shortAppinfo.Url))
                return Json(new ChalkableException(ChlkResources.ERR_APP_URL_MISSING));
            if (MasterLocator.ApplicationUploadService.Exists(appId, shortAppinfo.Name, null))
                return Json(new ChalkableException(ChlkResources.ERR_APP_DUPLICATE_NAME));
            if (MasterLocator.ApplicationUploadService.Exists(appId, null, shortAppinfo.Url))
                return Json(new ChalkableException(ChlkResources.ERR_APP_DUPLICATE_URL));

            var app = applicationInputModel.ForSubmit
                                ? MasterLocator.ApplicationUploadService.Submit(appId, applicationInputModel)
                                : MasterLocator.ApplicationUploadService.UpdateDraft(appId, applicationInputModel);

            //if (forSubmit) //TODO: mixpanel
            //{

            //    MixPanelService.SubmittedForApprooval(SchoolLocator.Context.UserName, shortAppInfo.Name, shortAppInfo.ShortDescription,
            //                                          subjects, appPrices.Price, appPrices.PricePerSchool, appPrices.PricePerClass);
            //}
            //else
            //{
            //    MixPanelService.UpdatedDraft(SchoolLocator.Context.UserName, shortAppInfo.Name, shortAppInfo.ShortDescription,
            //                                          subjects, appPrices.Price, appPrices.PricePerSchool, appPrices.PricePerClass);
            //}

            return PrepareAppInfo(app, true, true);
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult UploadPicture(int? width, int? height)
        {
            return UploadPicture(MasterLocator.ApplicationPictureService, Guid.NewGuid(), width, height);
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult GetInfo(Guid applicationId)
        {
            var res = MasterLocator.ApplicationService.GetApplicationById(applicationId);
            return PrepareAppInfo(res, true, true);
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult Delete(Guid applicationId)
        {
            var res = MasterLocator.ApplicationUploadService.DeleteApplication(applicationId);
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Approve(Guid applicationId)
        {
            var res = MasterLocator.ApplicationUploadService.ApproveReject(applicationId, true);
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Decline(Guid applicationId)
        {
            var res = MasterLocator.ApplicationUploadService.ApproveReject(applicationId, false);
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult GoLive(Guid applicationId)
        {
            var res = MasterLocator.ApplicationUploadService.GoLive(applicationId);
            //if (res) //TODO: mix panel
            //{
            //    var app = MasterLocator.ApplicationService.GetApplicationById(id, false);
            //    MixPanelService.SelectedLive(ServiceLocator.Context.UserName, app.Name);
            //}
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult Unlist(Guid applicationId)
        {
            var res = MasterLocator.ApplicationUploadService.UnList(applicationId);
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult ChangeApplicationType(Guid applicationId, bool isinternal)
        {
            MasterLocator.ApplicationUploadService.ChangeApplicationType(applicationId, isinternal);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult AddToAnnouncement(int announcementId, Guid applicationId)
        {
            if(!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();

            var res = SchoolLocator.ApplicationSchoolService.AddToAnnouncement(announcementId, applicationId);
            var appInstalls = SchoolLocator.AppMarketService.GetInstallations(applicationId, Context.UserLocalId.Value, false);
            var app = MasterLocator.ApplicationService.GetApplicationById(applicationId);
            return Json(AnnouncementApplicationViewData.Create(res, app, appInstalls, Context.UserLocalId));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult RemoveFromAnnouncement(int announcementApplicationId)
        {
            var ann = SchoolLocator.ApplicationSchoolService.RemoveFromAnnouncement(announcementApplicationId);
            return Json(PrepareFullAnnouncementViewData(ann.Id), 6);
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Attach(int announcementApplicationId)
        {
            SchoolLocator.ApplicationSchoolService.AttachAppToAnnouncement(announcementApplicationId);
            var aa = SchoolLocator.ApplicationSchoolService.GetAnnouncementApplication(announcementApplicationId);
            return Json(PrepareFullAnnouncementViewData(aa.AnnouncementRef));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_GET_APP_ANNOUNCEMENT_APPLICATION, true, CallType.Post, new[] { AppPermissionType.Announcement })]
        public ActionResult GetAnnouncementApplication(int announcementApplicationId)
        {
            var res = SchoolLocator.ApplicationSchoolService.GetAnnouncementApplication(announcementApplicationId);
            var app = MasterLocator.ApplicationService.GetApplicationById(res.ApplicationRef);
            return Json(AnnouncementApplicationViewData.Create(res, app, null, null));
        }


        [AuthorizationFilter]
        public ActionResult GetOauthCode(string applicationUrl)
        {
            //TODO: check if app is installed??
            var authorizationCode = MasterLocator.AccessControlService.GetAuthorizationCode(applicationUrl, MasterLocator.Context.Login);
            authorizationCode = HttpUtility.UrlEncode(authorizationCode);
            return Json(authorizationCode);
        }

        private const string contentType = "text/html";
        private ActionResult PrepareAppInfo(Application application, bool needsliveApp = false, bool needsSecretKey = false)
        {
            return Json(PrepareAppInfo(MasterLocator, application, needsliveApp, needsSecretKey), contentType);
        }
        public static ApplicationViewData PrepareAppInfo(IServiceLocatorMaster locator, Application application, 
            bool needsliveApp = false, bool needsSecretKey = false)
        {
            bool canGetSecretKey = false;
            if (needsSecretKey)
                canGetSecretKey = locator.ApplicationService.CanGetSecretKey(new List<Application> {application});
            var categories = locator.CategoryService.ListCategories();
            var res = ApplicationViewData.Create(application, categories, canGetSecretKey);
            if (needsliveApp && application.OriginalRef.HasValue)
            {
                var liveApp = locator.ApplicationService.GetApplicationById(application.Id);
                res.LiveApplication = ApplicationViewData.Create(liveApp,  categories, canGetSecretKey);
            }
            return res;
        }
    }
}