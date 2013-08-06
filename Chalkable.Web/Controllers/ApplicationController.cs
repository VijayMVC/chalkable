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
    public class ApplicationController : ChalkableController
    {
        public ActionResult List(int? start, int? count)
        {
            var applications = MasterLocator.ApplicationService.GetApplications(start ?? 0, count ?? DEFAULT_PAGE_SIZE, false);
            return Json(BaseApplicationViewData.Create(applications));
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
            return PrepareAppInfo(application);
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult Update(Guid applicationId, ShortApplicationInfo shortApplicationInfo, IntList permissionsIds, 
               ApplicationPricesInfo applicationPrices, Guid developerId, ApplicationAccessInfo applicationAccess,
               GuidList categoriesid, GuidList picturesid, IntList gradelevels,  bool forSubmit)
        {
            var permissions = permissionsIds.Select(x => (AppPermissionType) x).ToList();
            var baseAppInfo = BaseApplicationInfo.Create(shortApplicationInfo, developerId, permissions, picturesid, applicationPrices
                                                         , categoriesid, applicationAccess, gradelevels);

            if (string.IsNullOrEmpty(shortApplicationInfo.Name))
                return Json(new ChalkableException(ChlkResources.ERR_APP_NAME_MISSING));
            if (string.IsNullOrEmpty(shortApplicationInfo.Url))
                return Json(new ChalkableException(ChlkResources.ERR_APP_URL_MISSING));
            if (MasterLocator.ApplicationUploadService.Exists(applicationId, shortApplicationInfo.Name, null))
                return Json(new ChalkableException(ChlkResources.ERR_APP_NAME_MISSING));
            if (MasterLocator.ApplicationUploadService.Exists(applicationId, null, shortApplicationInfo.Url))
                return Json(new ChalkableException(ChlkResources.ERR_APP_DUPLICATE_URL));

            var app = forSubmit ? MasterLocator.ApplicationUploadService.Submit(applicationId, baseAppInfo)
                                : MasterLocator.ApplicationUploadService.UpdateDraft(applicationId, baseAppInfo);

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

            return PrepareAppInfo(app);
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult GetInfo(Guid applicationId)
        {
            var res = MasterLocator.ApplicationService.GetApplicationById(applicationId);
            return PrepareAppInfo(res, true);
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

        [AuthorizationFilter]
        public ActionResult GetOauthCode(string applicationUrl)
        {
            //TODO: check if app is installed??
            var authorizationCode = MasterLocator.AccessControlService.GetAuthorizationCode(applicationUrl, MasterLocator.Context.Login);
            authorizationCode = HttpUtility.UrlEncode(authorizationCode);
            return Json(authorizationCode);
        }


        private const string contentType = "text/html";


        private ActionResult PrepareAppInfo(Application application, bool needsliveApp = false)
        {
            return Json(PrepareAppInfo(MasterLocator, application, needsliveApp), contentType);
        }
        public static ApplicationViewData PrepareAppInfo(IServiceLocatorMaster locator, Application application, 
            bool needsliveApp = false, bool needsSecretKey = false)
        {
            var roles = new List<CoreRole>
                {
                    CoreRoles.ADMIN_GRADE_ROLE,
                    CoreRoles.ADMIN_EDIT_ROLE,
                    CoreRoles.ADMIN_VIEW_ROLE,
                    CoreRoles.TEACHER_ROLE,
                    CoreRoles.STUDENT_ROLE,
                };
            bool cangetSecretKey = false;
            if (needsSecretKey)
                cangetSecretKey = locator.ApplicationService.CanGetSecretKey(new List<Application> {application});
            var categories = locator.CategoryService.ListCategories();
            var res = ApplicationViewData.Create(application, roles, categories, cangetSecretKey);
            if (needsliveApp && application.OriginalRef.HasValue)
            {
                var liveApp = locator.ApplicationService.GetApplicationById(application.Id);
                res.LiveApplication = ApplicationViewData.Create(liveApp, roles, categories, cangetSecretKey);
            }
            return res;
        }
    }
}