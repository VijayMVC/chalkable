using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;
using Chalkable.Web.Controllers.AnnouncementControllers;
using Chalkable.Web.Logic;
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

                MasterLocator.UserTrackingService.SubmittedForApprooval(Context.Login, app.Name, app.ShortDescription, subjects);
            }
            else
            {
                MasterLocator.UserTrackingService.UpdatedDraft(Context.Login, app.Name, app.ShortDescription, subjects);
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


        [AuthorizationFilter("SysAdmin, DistrictAdmin")]
        public ActionResult SubmitApplicationBan(Guid applicationId, GuidList schoolIds)
        {
            MasterLocator.ApplicationService.SubmitApplicationBan(applicationId, schoolIds);
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
        public ActionResult AddToAnnouncement(int announcementId, int announcementType, Guid applicationId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            var res = SchoolLocator.ApplicationSchoolService.AddToAnnouncement(announcementId, (AnnouncementTypeEnum) announcementType,applicationId);
            var app = MasterLocator.ApplicationService.GetApplicationById(applicationId);
            
            var assessmentApp = MasterLocator.ApplicationService.GetAssessmentApplication();
            if (assessmentApp != null && applicationId == assessmentApp.Id)
                MasterLocator.UserTrackingService.AttachedAssessment(Context.Login, announcementId);

            return Json(AnnouncementApplicationViewData.Create(res, app, Context.PersonId, (AnnouncementTypeEnum)announcementType));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult RemoveFromAnnouncement(int announcementApplicationId, int announcementType)
        {
            var ann = SchoolLocator.ApplicationSchoolService.RemoveFromAnnouncement(announcementApplicationId, (AnnouncementTypeEnum)announcementType);
            return Json(PrepareFullAnnouncementViewData(ann.Id, (AnnouncementTypeEnum)announcementType), 6);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Attach(int announcementApplicationId, int announcementType)
        {
            SchoolLocator.ApplicationSchoolService.AttachAppToAnnouncement(announcementApplicationId, (AnnouncementTypeEnum)announcementType);
            var aa = SchoolLocator.ApplicationSchoolService.GetAnnouncementApplication(announcementApplicationId);
            return Json(PrepareFullAnnouncementViewData(aa.AnnouncementRef, (AnnouncementTypeEnum)announcementType));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher", true, new[] { AppPermissionType.Announcement })]
        public ActionResult UpdateAnnouncementApplicationMeta(int announcementApplicationId, string text, string imageUrl, string description)
        {
            var res = SchoolLocator.ApplicationSchoolService.GetAnnouncementApplication(announcementApplicationId);
            var announcementType = SchoolLocator.AnnouncementFetchService.GetAnnouncementType(res.AnnouncementRef);
            SchoolLocator.ApplicationSchoolService.UpdateAnnouncementApplicationMeta(announcementApplicationId, announcementType, text, imageUrl, description);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult GetAnnouncementApplication(int announcementApplicationId)
        {
            var res = SchoolLocator.ApplicationSchoolService.GetAnnouncementApplication(announcementApplicationId);
            var announcementType = SchoolLocator.AnnouncementFetchService.GetAnnouncementType(res.AnnouncementRef);
            var app = MasterLocator.ApplicationService.GetApplicationById(res.ApplicationRef);
            return Json(AnnouncementApplicationViewData.Create(res, app, null, announcementType));
        }
        
        [AuthorizationFilter]
        public ActionResult GetAccessToken(string applicationUrl, Guid? applicationId)
        {
            var app = !string.IsNullOrWhiteSpace(applicationUrl)
                    ? MasterLocator.ApplicationService.GetApplicationByUrl(applicationUrl) :
                applicationId.HasValue
                    ? MasterLocator.ApplicationService.GetApplicationById(applicationId.Value) : null;

            if (app == null)
                throw new ChalkableException("Application not found");

            var token = MasterLocator.ApplicationService.GetAccessToken(app.Id, ChalkableAuthentication.GetSessionKey());
            
            return Json(new
            {
                Token = token,
                ApplicationInfo = BaseApplicationViewData.Create(app)
            });
        }

        private const string CONTENT_TYPE = "text/html";

        public static ApplicationViewData PrepareAppInfo(IServiceLocatorMaster locator, Application application, 
            bool needsliveApp = false, bool needsSecretKey = false)
        {
            bool canGetSecretKey = false;
            if (needsSecretKey)
                canGetSecretKey = locator.ApplicationService.CanGetSecretKey(new List<Application> {application});
            
            var categories = locator.CategoryService.ListCategories();


            //TODO: IMPLEMENT STANDARTS LATER
            var res = ApplicationViewData.Create(application, categories, null, canGetSecretKey);
            if (needsliveApp && application.OriginalRef.HasValue)
            {
                var liveApp = locator.ApplicationService.GetApplicationById(application.Id);

                //TODO: IMPLEMENT STANDARTS LATER
                res.LiveApplication = ApplicationViewData.Create(liveApp, categories, null, canGetSecretKey);
            }
            return res;
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult AnnouncementApplicationRecipients(int? studentId)
        {
            var app = MasterLocator.ApplicationService.GetApplicationByUrl(SchoolLocator.Context.OAuthApplication);
            var announcementApplicationRecipient = SchoolLocator.ApplicationSchoolService.GetAnnouncementApplicationRecipients(studentId, app.Id);
            return Json(announcementApplicationRecipient);
        }

        private const int ATTACH_DEFAULT_PAGE_SIZE = 12;
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult MyApps(GuidList categoriesIds, IntList gradeLevelsIds, string filter, int? start, int? count)
        {
            var myAppsOnly = !BaseSecurity.IsDistrictAdmin(Context) ? true : (bool?) null;
            var apps = MasterLocator.ApplicationService.GetApplications(categoriesIds, gradeLevelsIds, filter, start ?? 0, count ?? DEFAULT_PAGE_SIZE, withBanned: true, myApps: myAppsOnly);

            if (!BaseSecurity.IsDistrictAdmin(Context))
                return Json(apps.Transform(BaseApplicationViewData.Create));

            Trace.Assert(Context.DistrictId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            var school = MasterLocator.SchoolService.GetById(Context.DistrictId.Value, Context.SchoolLocalId.Value);
            var appBanInfos = MasterLocator.ApplicationService
                .GetApplicationBanInfos(Context.DistrictId.Value, school.Id, apps.Select(x => x.Id).ToList());

            var viewData = apps.Transform(x => BaseApplicationViewData.Create(x, appBanInfos.FirstOrDefault(y => y.ApplicationId == x.Id)));

            return Json(viewData);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult ApplicationBannedSchools(Guid applicationId)
        {
            Trace.Assert(Context.DistrictId.HasValue);
            
            var appSchoolOptions = MasterLocator.ApplicationService.GetApplicationSchoolBans(Context.DistrictId.Value, applicationId);

            return Json(appSchoolOptions.Select(ApplicationSchoolOptionViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult ListForAttach(int? start, int? count)
        {
            var st = start ?? 0;
            var cnt = count ?? ATTACH_DEFAULT_PAGE_SIZE;

            var applications = MasterLocator.ApplicationService.GetApplications(st, cnt, true, canAttach: true);
            return Json(applications.Transform(BaseApplicationViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SuggestedApps(GuidList abIds, int? start, int? count, bool? myAppsOnly)
        {
            Trace.Assert(Context.PersonId.HasValue);

            var st = start ?? 0;
            var cnt = count ?? int.MaxValue;

            var suggestedApplications = MasterLocator.ApplicationService.GetSuggestedApplications(abIds, st, cnt);

            if (myAppsOnly.HasValue && myAppsOnly.Value)
                suggestedApplications = suggestedApplications.Where(x => MasterLocator.ApplicationService.HasMyApps(x)).ToList();

            return Json(suggestedApplications.Select(BaseApplicationViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult ExternalAttachApps(int? start, int? count)
        {
            var apps = MasterLocator.ApplicationService.GetApplications(live: true).ToList();
            apps = apps.Where(app => MasterLocator.ApplicationService.HasExternalAttachMode(app)).ToList();
            return Json(BaseApplicationViewData.Create(apps));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult UpdateStudentAnnouncementApplicationMeta(int announcementApplicationId, int studentId, string text)
        {
            SchoolLocator.ApplicationSchoolService.UpdateStudentAnnouncementApplicationMeta(announcementApplicationId, studentId, text);
            return Json(true);
        }
    }
}