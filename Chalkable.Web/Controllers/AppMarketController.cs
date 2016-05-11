using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Chalkable.API.Helpers;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AppMarketController : ChalkableController
    {
        private const int ATTACH_DEFAULT_PAGE_SIZE = 12;
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult List(GuidList categoriesIds, IntList gradeLevelsIds, string filter, int? filterMode, int? sortingMode, int? start, int? count)
        {
            var apps = MasterLocator.ApplicationService.GetApplications(categoriesIds, gradeLevelsIds, filter
                          , (AppFilterMode?) filterMode, (AppSortingMode?) sortingMode, start ?? 0, count ?? DEFAULT_PAGE_SIZE);
            return Json(apps.Transform(BaseApplicationViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult ListInstalledForAttach(int personId, int classId, int markingPeriodId, int? start, int? count)
        {
            var st = start ?? 0;
            var cnt = count ?? ATTACH_DEFAULT_PAGE_SIZE;
            
            var applications = MasterLocator.ApplicationService.GetApplications(st, cnt, true);
            return Json(applications.Transform(BaseApplicationViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult ListInstalledForAdminAttach(int personId, IntList groupIds, int? start, int? count)
        {
            var st = start ?? 0;
            var cnt = count ?? ATTACH_DEFAULT_PAGE_SIZE;

            var apps = MasterLocator.ApplicationService.GetApplications(st, cnt, true);
            return Json(apps.Transform(BaseApplicationViewData.Create));
        }


        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SuggestedApps(int classId, GuidList abIds, int markingPeriodId, int? start, int? count, bool? myAppsOnly)
        {
            Trace.Assert(Context.PersonId.HasValue);

            var st = start ?? 0;
            var cnt = count ?? int.MaxValue;

            var allApps = MasterLocator.ApplicationService.GetApplications(live: true).Select(x => x.Id).ToList();
            var suggestedApplications = MasterLocator.ApplicationService.GetSuggestedApplications(abIds, allApps, st, cnt);
            
            if (myAppsOnly.HasValue && myAppsOnly.Value)
                suggestedApplications = suggestedApplications.Where(x => MasterLocator.ApplicationService.HasMyApps(x)).ToList();

            return Json(suggestedApplications.Select(BaseApplicationViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SuggestedAppsForAttach(int classId, GuidList abIds, int markingPeriodId, int? start, int? count)
        {
            Trace.Assert(Context.PersonId.HasValue);

            var suggestedAppsForAttach = ApplicationLogic.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator,
                Context.PersonId.Value, classId, abIds, markingPeriodId, start, count ?? 3);

            return Json(suggestedAppsForAttach);
        }
        

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult ListInstalledWithContent(int personId, int classId, int markingPeriodId)
        {
            var res = GetApplicationsWithContent(SchoolLocator, MasterLocator);
            return Json(res);
        }

        public static IList<BaseApplicationViewData> GetApplicationsWithContent(IServiceLocatorSchool schoolLocator, IServiceLocatorMaster masterLocator)
        {
            IList<Application> applications = masterLocator.ApplicationService.GetApplications(live: true);
            applications = applications.Where(x => x.ProvidesRecommendedContent).ToList();

            var res = BaseApplicationViewData.Create(applications);
            foreach (var app in res)
                app.EncodedSecretKey = HashHelper.HexOfCumputedHash(applications.First(x => x.Id == app.Id).SecretKey);

            return res;
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, Student")]
        public ActionResult Read(Guid applicationId)
        {
            var application = MasterLocator.ApplicationService.GetApplicationById(applicationId);
            var categories = MasterLocator.CategoryService.ListCategories();
            var appRatings = MasterLocator.ApplicationService.GetRatings(applicationId);

            IList<ApplicationBanHistory> banHistory = null;
            if (Context.Role.Id == CoreRoles.DISTRICT_ADMIN_ROLE.Id)
                banHistory = SchoolLocator.ApplicationSchoolService.GetApplicationBanHistory(applicationId);
            
            var res = ApplicationDetailsViewData.Create(application, null, categories, appRatings, banHistory);
            return Json(res);
        }
        
        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, Student")]
        public ActionResult GetApplicationTotalPrice(Guid applicationid, int? personId, IntList classids)
        {
            var app = MasterLocator.ApplicationService.GetApplicationById(applicationid);
            var totalPrice = SchoolLocator.AppMarketService.GetApplicationTotalPrice(applicationid, personId, classids);
            return Json(ApplicationTotalPriceViewData.Create(app, totalPrice));
        }

        public ActionResult ExistsReview(Guid applicationId)
        {
            return Json(MasterLocator.ApplicationService.ReviewExists(applicationId));
        }

        public ActionResult WriteReview(Guid applicationId, int rating, string review)
        {
            MasterLocator.ApplicationService.WriteReview(applicationId, rating, review);
            return Read(applicationId);
        }
    }
}