﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model.ApplicationInstall;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ApplicationController : AnnouncementBaseController //TODO: think about this
    {
        [AuthorizationFilter("SysAdmin, Developer, AppTester")]
        public ActionResult List(Guid? developerId, int? state, int? start, int? count)
        {           
            var applications = MasterLocator.ApplicationService.GetApplicationsWithLive(developerId, (ApplicationStateEnum?)state, null, start ?? 0, count ?? DEFAULT_PAGE_SIZE);  
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
           
            if (application != null)
            {
                MasterLocator.UserTrackingService.CreatedApp(Context.Login, application.Name);
            }
            return Json(PrepareAppInfo(MasterLocator, application, true, true));
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


            var subjects = string.Join(",", app.Categories.Select(x => x.CategoryRef.ToString()));

            if (applicationInputModel.ForSubmit) //TODO: mixpanel
            {

                MasterLocator.UserTrackingService.SubmittedForApprooval(Context.Login, app.Name, app.ShortDescription,
                                                      subjects, applicationInputModel.ApplicationPrices.Price, 
                                                      applicationInputModel.ApplicationPrices.PricePerSchool,
                                                      applicationInputModel.ApplicationPrices.PricePerClass);
            }
            else
            {
                MasterLocator.UserTrackingService.UpdatedDraft(Context.Login, app.Name, app.ShortDescription,
                                                      subjects, applicationInputModel.ApplicationPrices.Price, 
                                                      applicationInputModel.ApplicationPrices.PricePerSchool, 
                                                      applicationInputModel.ApplicationPrices.PricePerClass);
            }

            return Json(PrepareAppInfo(MasterLocator, app, true, true), CONTENT_TYPE);
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
            return Json(PrepareAppInfo(MasterLocator, res, true, true));
        }

        [AuthorizationFilter("Developer")]
        public ActionResult GetAppAnalytics(Guid applicationId)
        {
            return FakeJson("~/fakeData/appAnalytics.json");
        }

        [AuthorizationFilter("Developer")]
        public ActionResult GetAppReviews(Guid applicationId)
        {
            var appRatings = MasterLocator.ApplicationService.GetRatings(applicationId);
            return Json(ApplicationRatingViewData.Create(appRatings));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult BanApp(Guid applicationId)
        {
            Trace.Assert(Context.DistrictId.HasValue);
            MasterLocator.ApplicationService.SetApplicationDistrictOptions(applicationId, Context.DistrictId.Value, true);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult UnbanApp(Guid applicationId)
        {
            Trace.Assert(Context.DistrictId.HasValue);
            MasterLocator.ApplicationService.SetApplicationDistrictOptions(applicationId, Context.DistrictId.Value, false);
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult Delete(Guid applicationId)
        {
            var res = MasterLocator.ApplicationUploadService.DeleteApplication(applicationId);
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin, AppTester")]
        public ActionResult Approve(Guid applicationId)
        {
            var res = MasterLocator.ApplicationUploadService.ApproveReject(applicationId, true);
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin, AppTester")]
        public ActionResult Decline(Guid applicationId)
        {
            var res = MasterLocator.ApplicationUploadService.ApproveReject(applicationId, false);
            return Json(res);
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult GoLive(Guid applicationId)
        {
            var res = MasterLocator.ApplicationUploadService.GoLive(applicationId);
            if (res)
            {
                MasterLocator.UserTrackingService.SelectedLive(Context.Login, applicationId.ToString());
            }
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
        
        [AuthorizationFilter("SysAdmin")]
        public ActionResult SetApplicationInternalData(Guid applicationId, int? internalScore, string internalDescription)
        {
            MasterLocator.ApplicationUploadService.SetApplicationInternalData(applicationId, internalScore, internalDescription);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult AddToAnnouncement(int announcementId, Guid applicationId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            var res = SchoolLocator.ApplicationSchoolService.AddToAnnouncement(announcementId, applicationId);
            var appInstalls = SchoolLocator.AppMarketService.GetInstallations(applicationId, Context.PersonId.Value, false);
            var app = MasterLocator.ApplicationService.GetApplicationById(applicationId);
            return Json(AnnouncementApplicationViewData.Create(res, app, appInstalls, Context.PersonId));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult RemoveFromAnnouncement(int announcementApplicationId)
        {
            var ann = SchoolLocator.ApplicationSchoolService.RemoveFromAnnouncement(announcementApplicationId);
            return Json(PrepareFullAnnouncementViewData(ann.Id), 6);
        }


        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Attach(int announcementApplicationId)
        {
            SchoolLocator.ApplicationSchoolService.AttachAppToAnnouncement(announcementApplicationId);
            var aa = SchoolLocator.ApplicationSchoolService.GetAnnouncementApplication(announcementApplicationId);
            return Json(PrepareFullAnnouncementViewData(aa.AnnouncementRef));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
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
           
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var authorizationCode = MasterLocator.AccessControlService.GetAuthorizationCode(applicationUrl, SchoolLocator.Context.Login, SchoolLocator.Context.SchoolYearId);
            authorizationCode = HttpUtility.UrlEncode(authorizationCode);
            var app = MasterLocator.ApplicationService.GetApplicationByUrl(applicationUrl);
            var appInstall = SchoolLocator.AppMarketService.GetInstallationForPerson(app.Id, Context.PersonId.Value);
            var hasMyApps = MasterLocator.ApplicationService.HasMyApps(app);
            var applicationInstalls = new List<ApplicationInstall>();
            if(appInstall != null)
                applicationInstalls.Add(appInstall);
            var appView = InstalledApplicationViewData.Create(applicationInstalls, Context.PersonId, app, hasMyApps);
            return Json( new {
                                AuthorizationCode = authorizationCode,
                                ApplicationInfo = appView
                             });
        }

        private const string CONTENT_TYPE = "text/html";

        public static ApplicationViewData PrepareAppInfo(IServiceLocatorMaster locator, Application application, 
            bool needsliveApp = false, bool needsSecretKey = false)
        {
            bool canGetSecretKey = false;
            if (needsSecretKey)
                canGetSecretKey = locator.ApplicationService.CanGetSecretKey(new List<Application> {application});
            var standards = locator.CommonCoreStandardService.GetStandards();
            var categories = locator.CategoryService.ListCategories();
            var res = ApplicationViewData.Create(application, categories, standards, canGetSecretKey);
            if (needsliveApp && application.OriginalRef.HasValue)
            {
                var liveApp = locator.ApplicationService.GetApplicationById(application.Id);
                res.LiveApplication = ApplicationViewData.Create(liveApp, categories, standards, canGetSecretKey);
            }
            return res;
        }
    }
}