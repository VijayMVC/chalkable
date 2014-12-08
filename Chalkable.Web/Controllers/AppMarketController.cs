﻿using System;
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
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult List(GuidList categoriesIds, IntList gradeLevelsIds, string filter, int? filterMode, int? sortingMode, int? start, int? count)
        {
            var apps = MasterLocator.ApplicationService.GetApplications(categoriesIds, gradeLevelsIds, filter
                          , (AppFilterMode?) filterMode, (AppSortingMode?) sortingMode, start ?? 0, count ?? DEFAULT_PAGE_SIZE);
            return Json(apps.Transform(BaseApplicationViewData.Create));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ListInstalledForAttach(int personId, int classId, int markingPeriodId, int? start, int? count)
        {
            var st = start ?? 0;
            var cnt = count ?? 9;

            var studentCountPerApp = SchoolLocator.AppMarketService.GetNotInstalledStudentCountPerApp(personId, classId, markingPeriodId);
            var installedApp = GetApplications(studentCountPerApp.Select(x => x.Key).Distinct().ToList(), true, null);
            var res = ApplicationForAttachViewData.Create(installedApp, studentCountPerApp);
            var totalCount = res.Count;
            res = res.Skip(st).Take(cnt).ToList();
            return Json(new PaginatedList<ApplicationForAttachViewData>(res, st / cnt, cnt, totalCount));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult SuggestedApps(int classId, StringList standardsCodes, int markingPeriodId, int? start, int? count)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var st = start ?? 0;
            var cnt = count ?? 3;
            var appInstalls = SchoolLocator.AppMarketService.ListInstalledAppInstalls(Context.PersonId.Value);
            var installedAppsIds = appInstalls.GroupBy(x=>x.ApplicationRef).Select(x => x.Key).Distinct().ToList();
            var applications = MasterLocator.ApplicationService.GetSuggestedApplications(standardsCodes.ToList(), installedAppsIds, st, cnt);
            var hasMyAppsDic = applications.ToDictionary(app=> app.Id, app => MasterLocator.ApplicationService.HasMyApps(app));
            return Json(InstalledApplicationViewData.Create(appInstalls, Context.PersonId.Value, applications, hasMyAppsDic));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult SuggestedAppsForAttach(int classId, StringList standardsCodes, int markingPeriodId, int? start, int? count)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            return Json(ApplicationLogic.GetSuggestedAppsForAttach(MasterLocator, SchoolLocator, Context.PersonId.Value, classId, standardsCodes, markingPeriodId, start, count));
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ListInstalled(int personId, string filter, int? start, int? count)
        {
            var st = start ?? 0;
            var cnt = count ?? 9;
            var appInstallations = SchoolLocator.AppMarketService.ListInstalledAppInstalls(personId);
            var installedApp = GetApplications(appInstallations.Select(x => x.ApplicationRef).Distinct().ToList(), true, null);
            var hasMyAppDic = installedApp.ToDictionary(x => x.Id, x => MasterLocator.ApplicationService.HasMyApps(x));
            var res = InstalledApplicationViewData.Create(appInstallations, personId, installedApp, hasMyAppDic);
            var totalCount = res.Count;
            res = res.Skip(st).Take(cnt).ToList();
            var appsList = new PaginatedList<InstalledApplicationViewData>(res, st / cnt, cnt, totalCount);
            return Json(appsList);
        }

        private IList<Application> GetApplications(IList<Guid> ids, bool? forAttach, string filter)
        {
            var res = MasterLocator.ApplicationService.GetApplicationsByIds(ids);
            if(forAttach.HasValue)
                res = res.Where(x => x.CanAttach).ToList();
            if (!string.IsNullOrEmpty(filter))
                res = res.Where(x => x.Name.ToLower().Contains(filter)).ToList();
            return res;
        }

        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        //public ActionResult ListInstalled(int personId, int? classId, string filter, int? start, int? count, bool? forAttach)
        //{
        //    var st = start ?? 0;
        //    var cnt = count ?? 9;

        //    var appInstallations = SchoolLocator.AppMarketService.ListInstalledAppInstalls(personId);
        //    var installedApp = MasterLocator.ApplicationService.GetApplicationsByIds(
        //            appInstallations.Select(x => x.ApplicationRef).Distinct().ToList());
        //    if (forAttach.HasValue && forAttach.Value)
        //        installedApp = installedApp.Where(x => x.CanAttach).ToList();

        //    if (classId.HasValue)
        //    {
        //        var classPersons = SchoolLocator.ClassService.GetClassPersons(null, classId, null, null);
        //    }

        //    var hasMyAppDic = installedApp.ToDictionary(x => x.Id, x => MasterLocator.ApplicationService.HasMyApps(x));

        //    var res = InstalledApplicationViewData.Create(appInstallations, personId, installedApp, hasMyAppDic);
        //    if (!string.IsNullOrEmpty(filter))
        //        res = res.Where(x => x.Name.ToLower().Contains(filter)).ToList();
            
        //    var totalCount = res.Count;
        //    res = res.Skip(st).Take(cnt).ToList();
        //    var appsList = new PaginatedList<InstalledApplicationViewData>(res, st/cnt, cnt, totalCount);
        //    return Json(appsList);
        //}


        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher, Student")]
        public ActionResult Install(Guid applicationId, int? personId, IntList classids, IntList roleIds, GuidList departmentids, IntList gradelevelids)
        {
            var schoolyearId = GetCurrentSchoolYearId();
            if (!SchoolLocator.AppMarketService.CanInstall(applicationId, personId, roleIds, classids, gradelevelids, departmentids))
                throw new ChalkableException(ChlkResources.ERR_APP_NOT_ENOUGH_MONEY_OR_ALREADY_INSTALLED);

            var totalPrice = SchoolLocator.AppMarketService.GetApplicationTotalPrice(applicationId, personId, roleIds, classids, gradelevelids, departmentids).TotalPrice;
            var appinstallAction = SchoolLocator.AppMarketService.Install(applicationId, personId, roleIds, classids, departmentids, gradelevelids, schoolyearId, Context.NowSchoolYearTime);
            try
            {
                if (departmentids == null) departmentids = new GuidList();
                if (gradelevelids == null) gradelevelids = new IntList();
                if (classids == null) classids = new IntList();
                
                //todo: person payment
                // MasterLocator.FundService.AppInstallPersonPayment(appinstallAction.Id, totalPrice, Context.NowSchoolTime, ChlkResources.APP_WAS_BOUGHT);   
                var classes = classids.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToList();
                var gradeLevels = gradelevelids.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToList();
                var departments = departmentids.Select(x => x.ToString()).ToList();
                MasterLocator.UserTrackingService.BoughtApp(Context.Login, applicationId.ToString(), classes, departments, gradeLevels);
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
        public ActionResult Uninstall(IntList applicationInstallIds)
        {
            SchoolLocator.AppMarketService.Uninstall(applicationInstallIds);
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin, Developer, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Read(Guid applicationId)
        {
            var application = MasterLocator.ApplicationService.GetApplicationById(applicationId);
            var categories = MasterLocator.CategoryService.ListCategories();
            var appRatings = MasterLocator.ApplicationService.GetRatings(applicationId);
            
            
            var res = ApplicationDetailsViewData.Create(application, null, categories, appRatings);
            var persons = SchoolLocator.AppMarketService.GetPersonsForApplicationInstallCount(application.Id, Context.PersonId, null, null, null, null);
            res.InstalledForPersonsGroup = ApplicationLogic.PrepareInstalledForPersonGroupData(SchoolLocator, MasterLocator, application);
            res.IsInstalledOnlyForMe = persons.First(x => x.Type == PersonsForAppInstallTypeEnum.Total).Count == 0;
            return Json(res);
        }
        
        [AuthorizationFilter("SysAdmin, Developer, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetApplicationTotalPrice(Guid applicationid, int? personId, IntList classids, IntList roleids, GuidList departments, IntList gradelevelids)
        {
            var app = MasterLocator.ApplicationService.GetApplicationById(applicationid);
            var totalPrice = SchoolLocator.AppMarketService.GetApplicationTotalPrice(applicationid, personId, roleids, classids, gradelevelids, departments);
            return Json(ApplicationTotalPriceViewData.Create(app, totalPrice));
        }

        public ActionResult ExistsReview(Guid applicationId)
        {
            return Json(MasterLocator.ApplicationService.ExistsReview(applicationId));
        }

        public ActionResult WriteReview(Guid applicationId, int rating, string review)
        {
            if(!MasterLocator.ApplicationService.ExistsReview(applicationId))
                MasterLocator.ApplicationService.WriteReview(applicationId, rating, review);
            return Read(applicationId);
        }
    }
}