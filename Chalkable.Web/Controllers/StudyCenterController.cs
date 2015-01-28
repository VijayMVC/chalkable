using System;
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
using Chalkable.Web.Models;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class StudyCenterController : ChalkableController
    {
        [AuthorizationFilter("Student", Preference.API_DESCR_SET_PRACTICE_GRADE, true, CallType.Post, new[] { AppPermissionType.Announcement })]
        public ActionResult SetPracticeGrade(string ccStandardCode, string score)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var standard = SchoolLocator.StandardService.GetStandardByCode(ccStandardCode);
            var app = MasterLocator.ApplicationService.GetApplicationByUrl(Context.OAuthApplication);
            if (!HasInstalledApp(app.Id, Context.PersonId.Value))
                throw new ChalkableSecurityException("Current studented has no installed app");
            SchoolLocator.PracticeGradeService.Add(standard.Id, Context.PersonId.Value, app.Id, score);
            return Json(true);
        }

        private bool HasInstalledApp(Guid applicationId, int studentId)
        {
            var practiceAppId = Guid.Parse(PreferenceService.Get(Preference.PRACTICE_APPLICATION_ID).Value);
            return practiceAppId == applicationId
                   || SchoolLocator.AppMarketService.GetInstallationForPerson(applicationId, studentId) != null;
        }


        [AuthorizationFilter("SysAdmin, Teacher, Student")]
        public ActionResult PracticeGrades(int studentId, int classId, int? standardId)
        {
            var syId = GetCurrentSchoolYearId();
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syId, Context.NowSchoolYearTime);
            var stadnards = SchoolLocator.StandardService.GetStandards(classId, null, null);
            var practiceGrades = SchoolLocator.PracticeGradeService.GetPracticeGrades(studentId, standardId);
            //TODO : getting standards scores from inow 
            var standardsScores = new List<GradingStandardInfo>();
            //var standardsScores = SchoolLocator.GradingStandardService.GetGradingStandards(classId, gradingPeriod.Id);
            return Json(PracticeGradeGridViewData.Create(practiceGrades, stadnards, standardsScores));
        }


        [AuthorizationFilter("SysAdmin, Teacher, Student")]
        public ActionResult MiniQuizInfo(string ccStandardCode)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            int start = 0, count= 4;
            var appId = Guid.Parse(PreferenceService.Get(Preference.PRACTICE_APPLICATION_ID).Value);

            //var miniQuizApp = new Application(){Pictures = new List<ApplicationPicture>()};
            
            var miniQuizApp = MasterLocator.ApplicationService.GetApplicationById(appId);
            
            var appInstallations = SchoolLocator.AppMarketService.ListInstalledAppInstalls(Context.PersonId.Value);
            var installedAppsIds = appInstallations.Select(x => x.ApplicationRef).Distinct().ToList();
            var suggestedApps = MasterLocator.ApplicationService.GetSuggestedApplications(new List<string>{ccStandardCode}, installedAppsIds, 0, int.MaxValue);
            var hasMyAppDic = suggestedApps.ToDictionary(x => x.Id, x => MasterLocator.ApplicationService.HasMyApps(x));
            return Json(MiniQuizAppInfoViewData.Create(miniQuizApp, suggestedApps, appInstallations, hasMyAppDic, Context.PersonId));
        }
    }
}