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

            SchoolLocator.PracticeGradeService.Add(standard.Id, Context.PersonId.Value, app.Id, score);
            return Json(true);
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
            
            var allApps = MasterLocator.ApplicationService.GetApplications(live:true)
                .Select(BaseApplicationViewData.Create).ToList();
            
            IList<Application> suggestedApps = new List<Application>();

            if (standard.AcademicBenchmarkId.HasValue)
                suggestedApps = MasterLocator.ApplicationService.GetSuggestedApplications(
                        new List<Guid> {standard.AcademicBenchmarkId.Value}, 0, int.MaxValue);

            var hasMyAppDic = suggestedApps.ToDictionary(x => x.Id, x => MasterLocator.ApplicationService.HasMyApps(x));
            var userInfo = OAuthUserIdentityInfo.Create(Context.Login, Context.Role, Context.SchoolYearId, ChalkableAuthentication.GetSessionKey());
            var authorizationCode = MasterLocator.AccessControlService.GetAuthorizationCode(miniQuizApp.Url, userInfo);
            
            return Json(MiniQuizAppInfoViewData.Create(miniQuizApp, suggestedApps.Select(BaseApplicationViewData.Create).ToList(), allApps, 
                hasMyAppDic, Context.PersonId, authorizationCode));
        }
    }
}