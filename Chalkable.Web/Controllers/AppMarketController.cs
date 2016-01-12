using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
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

            var studentCountPerApp = SchoolLocator.AppMarketService.GetNotInstalledStudentCountPerApp(personId, classId, markingPeriodId);
            var installedApp = GetApplications(MasterLocator, studentCountPerApp.Select(x => x.Key).Distinct().ToList(), true, null);
            var res = ApplicationForAttachViewData.Create(installedApp, studentCountPerApp);
            var totalCount = res.Count;
            res = res.Skip(st).Take(cnt).ToList();
            return Json(new PaginatedList<ApplicationForAttachViewData>(res, st / cnt, cnt, totalCount));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult ListInstalledForAdminAttach(int personId, IntList groupIds, int? start, int? count)
        {
            var installedApps = GetListInstalledApps(SchoolLocator, MasterLocator, personId, null, start, count, true);
            return Json(installedApps);
        }


        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SuggestedApps(int classId, GuidList abIds, int markingPeriodId, int? start, int? count, bool? myAppsOnly)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var st = start ?? 0;
            var cnt = count ?? int.MaxValue;
            var appInstalls = SchoolLocator.AppMarketService.ListInstalledAppInstalls(Context.PersonId.Value);
            var installedAppsIds = appInstalls.GroupBy(x=>x.ApplicationRef).Select(x => x.Key).Distinct().ToList();
            var applications = MasterLocator.ApplicationService.GetSuggestedApplications(abIds, installedAppsIds, 0, int.MaxValue);
            var hasMyAppsDic = applications.ToDictionary(app=> app.Id, app => MasterLocator.ApplicationService.HasMyApps(app));
            var res = InstalledApplicationViewData.Create(appInstalls, Context.PersonId.Value, applications, hasMyAppsDic);
            if (myAppsOnly.HasValue && myAppsOnly.Value)
                res = res.Where(x => x.HasMyApp).ToList();
            res = res.Skip(st).Take(cnt).ToList();
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SuggestedAppsForAttach(int classId, GuidList abIds, int markingPeriodId, int? start, int? count)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            return Json(ApplicationLogic.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator, Context.PersonId.Value, classId, abIds, markingPeriodId, start, count ?? 3));
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult ListInstalled(int personId, string filter, int? start, int? count)
        {
            var installedApps = GetListInstalledApps(SchoolLocator, MasterLocator, personId, filter, start, count, null);
            return Json(installedApps);
        }

        public static PaginatedList<InstalledApplicationViewData> GetListInstalledApps(IServiceLocatorSchool schoolLocator, IServiceLocatorMaster masterLocator
            , int personId, string filter, int? start, int? count, bool? forAttach)
        {
            var st = start ?? 0;
            var cnt = count ?? 9;
            var appInstallations = schoolLocator.AppMarketService.ListInstalledAppInstalls(personId);
            var installedApp = GetApplications(masterLocator, appInstallations.Select(x => x.ApplicationRef).Distinct().ToList(), forAttach, null);
            var hasMyAppDic = installedApp.ToDictionary(x => x.Id, x => masterLocator.ApplicationService.HasMyApps(x));
            var res = InstalledApplicationViewData.Create(appInstallations, personId, installedApp, hasMyAppDic);
            var totalCount = res.Count;
            res = res.Skip(st).Take(cnt).ToList();
            return new PaginatedList<InstalledApplicationViewData>(res, st / cnt, cnt, totalCount);
        }

        private static IList<Application> GetApplications(IServiceLocatorMaster masterLocator, IList<Guid> ids, bool? forAttach, string filter)
        {
            var res = masterLocator.ApplicationService.GetApplicationsByIds(ids);
            if(forAttach.HasValue && forAttach.Value)
                res = res.Where(x => x.CanAttach).ToList();
            if (!string.IsNullOrEmpty(filter))
                res = res.Where(x => x.Name.ToLower().Contains(filter)).ToList();
            return res;
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Install(Guid applicationId, int? personId, IntList classids)
        {
            if (!SchoolLocator.AppMarketService.CanInstall(applicationId, personId, classids))
                throw new ChalkableException(ChlkResources.ERR_APP_NOT_ENOUGH_MONEY_OR_ALREADY_INSTALLED);

            var appinstallAction = SchoolLocator.AppMarketService.Install(applicationId, personId, classids, Context.NowSchoolYearTime);
            try
            {
                if (classids == null) classids = new IntList();
                
                //todo: person payment
                // MasterLocator.FundService.AppInstallPersonPayment(appinstallAction.Id, totalPrice, Context.NowSchoolTime, ChlkResources.APP_WAS_BOUGHT);   
                var classes = classids.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToList();
                MasterLocator.UserTrackingService.BoughtApp(Context.Login, applicationId.ToString(), classes);
            }
            catch (Exception)
            {
                foreach (var appinstall in appinstallAction.ApplicationInstalls)
                    SchoolLocator.AppMarketService.Uninstall(appinstall.Id);
                throw;
            }
            return Json(true);
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Uninstall(IntList applicationInstallIds)
        {
            SchoolLocator.AppMarketService.Uninstall(applicationInstallIds);
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin, Developer, DistrictAdmin, Teacher, Student")]
        public ActionResult Read(Guid applicationId)
        {
            var application = MasterLocator.ApplicationService.GetApplicationById(applicationId);
            var categories = MasterLocator.CategoryService.ListCategories();
            var appRatings = MasterLocator.ApplicationService.GetRatings(applicationId);

            IList<ApplicationInstallHistory> history = null;
            IList<ApplicationBanHistory> banHistory = null;
            if (Context.Role.Id == CoreRoles.DISTRICT_ADMIN_ROLE.Id)
            {
                history = SchoolLocator.AppMarketService.GetApplicationInstallationHistory(applicationId);
                banHistory = SchoolLocator.ApplicationSchoolService.GetApplicationBanHistory(applicationId);
            }
            var res = ApplicationDetailsViewData.Create(application, null, categories, appRatings, history, banHistory);
            var persons = SchoolLocator.AppMarketService.GetPersonsForApplicationInstallCount(application.Id, Context.PersonId, null);
            res.InstalledForPersonsGroup = ApplicationLogic.PrepareInstalledForPersonGroupData(SchoolLocator, MasterLocator, application);
            res.IsInstalledOnlyForMe = persons.First(x => x.Type == PersonsForAppInstallTypeEnum.Total).Count == 0;
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