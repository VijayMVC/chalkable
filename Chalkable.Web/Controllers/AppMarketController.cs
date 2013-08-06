using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AppMarketController : ChalkableController
    {
        public ActionResult Instal(Guid applicationId, Guid? personId, GuidList classids, IntList roleIds, GuidList departmentids, GuidList gradelevelids)
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
        public ActionResult AppcalitionTotalPrice(Guid applicationid, Guid? personId, GuidList classids, IntList roleids, GuidList departments, GuidList gradelevelids)
        {
            var app = MasterLocator.ApplicationService.GetApplicationById(applicationid);
            var totalPrice = SchoolLocator.AppMarketService.GetApplicationTotalPrice(applicationid, personId, roleids, classids, gradelevelids, departments);
            return Json(ApplicationTotalPriceViewData.Create(app, totalPrice));
        }
    }
}