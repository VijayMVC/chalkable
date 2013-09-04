using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AppMarketController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult List(GuidList categoriesIds, IntList gradeLevelsIds, string filter, int? filterMode, int? sortingMode, int? start, int? count)
        {
            var apps = MasterLocator.ApplicationService.GetApplications(categoriesIds, gradeLevelsIds, filter
                          , (AppFilterMode?) filterMode, (AppSortingMode?) sortingMode, start ?? 0, count ?? DEFAULT_PAGE_SIZE);
            return Json(apps.Transform(BaseApplicationViewData.Create));
        }


        //TODO: add paginated list
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ListInstalled(Guid personId)
        {
            var sp = SchoolLocator.PersonService.GetPerson(personId);
            var appInstallations = SchoolLocator.AppMarketService.ListInstalledAppInstalls(personId);
            var instaledApp = SchoolLocator.AppMarketService.ListInstalled(personId, true);
            var hasMyAppDic = instaledApp.ToDictionary(x => x.Id, x => MasterLocator.ApplicationService.HasMyApps(x));
            var res = InstalledApplicationViewData.Create(appInstallations, sp, instaledApp, hasMyAppDic);
            return Json(res);
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher, Student")]
        public ActionResult Install(Guid applicationId, Guid? personId, GuidList classids, IntList roleIds, GuidList departmentids, GuidList gradelevelids)
        {
            var schoolyearId = GetCurrentSchoolYearId();
            if (!SchoolLocator.AppMarketService.CanInstall(applicationId, personId, roleIds, classids, gradelevelids, departmentids))
                throw new ChalkableException(ChlkResources.ERR_APP_NOT_ENOUGH_MONEY_OR_ALREADY_INSTALLED);

            var totalPrice = SchoolLocator.AppMarketService.GetApplicationTotalPrice(applicationId, personId, roleIds, classids, gradelevelids, departmentids).TotalPrice;
            var appinstallAction = SchoolLocator.AppMarketService.Install(applicationId, personId, roleIds, classids, departmentids, gradelevelids, schoolyearId, Context.NowSchoolTime);
            try
            {
                string description = ChlkResources.APP_WAS_BOUGHT;
                MasterLocator.FundService.AppInstallPersonPayment(appinstallAction.Id, totalPrice, Context.NowSchoolTime, description);   
                //TODO: mix panel
                //var classNames = appinstallAction.ApplicationInstallActionClasses.Select(x => x.Class.Name).ToList();
                ////var departments = appinstallAction.ApplicationInstallActionDepartments.Select(x => x.ChalkableDepartment.Name).ToList();//TODO: fix after DB remapping
                //List<string> departments = new List<string>();
                //var gradeLevels = appinstallAction.ApplicationInstallActionGradeLevels.Select(x => x.GradeLevel.Name).ToList();
                //MixPanelService.BoughtApp(locator.Context.UserName, appinstallAction.Application.Name, classNames, departments, gradeLevels);
            }
            catch (Exception)
            {
                foreach (var appinstall in appinstallAction.ApplicationInstalls)
                    SchoolLocator.AppMarketService.Uninstall(appinstall.Id);
                throw;
            }
            return Json(true);
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher, Student")]
        public ActionResult Uninstall(GuidList applicationInstallIds)
        {
            foreach (var applicationInstallId in applicationInstallIds)
            {
                SchoolLocator.AppMarketService.Uninstall(applicationInstallId);
            }
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin, Developer, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Read(Guid applicationId)
        {
            var application = MasterLocator.ApplicationService.GetApplicationById(applicationId);
            //TODO: application ratings 
            var categories = MasterLocator.CategoryService.ListCategories();
            var res = ApplicationDetailsViewData.Create(application, null, categories);
            var persons = SchoolLocator.AppMarketService.GetPersonsForApplicationInstallCount(application.Id, Context.UserId, null, null, null, null);
            res.InstalledForPersonsGroup = ApplicationLogic.PrepareInstalledForPersonGroupData(SchoolLocator, MasterLocator, application);
            res.IsInstaledOnlyForMe = persons.First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count == 0;
            return Json(res);
        }
        
        [AuthorizationFilter("SysAdmin, Developer, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetAppcalitionTotalPrice(Guid applicationid, Guid? personId, GuidList classids, IntList roleids, GuidList departments, GuidList gradelevelids)
        {
            var app = MasterLocator.ApplicationService.GetApplicationById(applicationid);
            var totalPrice = SchoolLocator.AppMarketService.GetApplicationTotalPrice(applicationid, personId, roleids, classids, gradelevelids, departments);
            return Json(ApplicationTotalPriceViewData.Create(app, totalPrice));
        }
    }
}