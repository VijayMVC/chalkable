﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model.ApplicationInstall;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class StudyCenterController : ChalkableController
    {
        [AuthorizationFilter("Student", true, new []{ AppPermissionType.Practice })]
        public ActionResult SetPracticeGrade(Guid id, string score)
        {
            if (string.IsNullOrWhiteSpace(score))
                throw new ChalkableException("Parameter score is required");

            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            var standard = SchoolLocator.StandardService.GetStandardByABId(id);
            if (standard == null)
                throw new ChalkableException("Standard not found");

            var app = MasterLocator.ApplicationService.GetApplicationByUrl(Context.OAuthApplication);
            if (!IsPracticeOrInstalledApp(app.Id, Context.PersonId.Value))
                throw new ChalkableSecurityException("Application is not installed for current student");

            SchoolLocator.PracticeGradeService.Add(standard.Id, Context.PersonId.Value, app.Id, score);
            return Json(true);
        }

        private bool IsPracticeOrInstalledApp(Guid applicationId, int studentId)
        {
            var practiceAppId = MasterLocator.ApplicationService.GetMiniQuizAppicationId();
            return practiceAppId == applicationId
                   || SchoolLocator.AppMarketService.GetInstallationForPerson(applicationId, studentId) != null;
        }


        [AuthorizationFilter("Student")]
        public ActionResult PracticeGrades(int studentId, int classId, int? standardId)
        {
            var stadnards = SchoolLocator.StandardService.GetStandards(classId, null, null);
            var practiceGrades = SchoolLocator.PracticeGradeService.GetPracticeGradesDetails(classId, studentId, standardId);
            return Json(PracticeGradeGridViewData.Create(practiceGrades, stadnards));
        }
        
        [AuthorizationFilter("Student")]
        public ActionResult MiniQuizInfo(int standardId)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var standard = SchoolLocator.StandardService.GetStandardById(standardId);
            var miniQuizApp = MasterLocator.ApplicationService.GetMiniQuizAppication();
            //var appInstallations = SchoolLocator.AppMarketService.ListInstalledAppInstalls(Context.PersonId.Value);
            //var installedAppsIds = appInstallations.Select(x => x.ApplicationRef).Distinct().ToList();
            //TODO: Fix logic. All apps will be avaliable for user except banned one
            var appInstallations = MasterLocator.ApplicationService.GetApplications().Select(x => new ApplicationInstall
            {
                Active = true,
                AppInstallActionRef = 0,
                ApplicationRef = x.Id,
                AppUninstallActionRef = null,
                Id = 0,
                InstallDate = DateTime.UtcNow,
                PersonRef = Context.PersonId.Value,
                SchoolYearRef = Context.SchoolYearId.Value,
                OwnerRef = Context.PersonId.Value
            }).ToList();
            var allApps = MasterLocator.ApplicationService.GetApplications().Select(x => x.Id).ToList();
            var suggestedApps = standard.AcademicBenchmarkId.HasValue 
                    ? MasterLocator.ApplicationService.GetSuggestedApplications(new List<Guid> { standard.AcademicBenchmarkId.Value }, allApps, 0, int.MaxValue)
                    : new Application[]{};
            var hasMyAppDic = suggestedApps.ToDictionary(x => x.Id, x => MasterLocator.ApplicationService.HasMyApps(x));

            var userInfo = OAuthUserIdentityInfo.Create(Context.Login, Context.Role, Context.SchoolYearId, ChalkableAuthentication.GetSessionKey());
            var authorizationCode = MasterLocator.AccessControlService.GetAuthorizationCode(miniQuizApp.Url, userInfo);

            return Json(MiniQuizAppInfoViewData.Create(miniQuizApp, suggestedApps, appInstallations, hasMyAppDic, Context.PersonId, authorizationCode));
        }
    }
}