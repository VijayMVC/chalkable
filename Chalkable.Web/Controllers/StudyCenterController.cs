using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Authentication;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.BusinessLogic.Security;

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
        public async Task<ActionResult> PracticeGrades(int studentId, int classId, int? standardId)
        {
            var practiceGrades = SchoolLocator.PracticeGradeService.GetPracticeGradesDetails(classId, studentId, standardId);
            var stadnards = SchoolLocator.StandardService.GetStandards(classId, null, null);
            return Json(PracticeGradeGridViewData.Create(await practiceGrades, stadnards));
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

            //filter banned suggestedApps
            if(!BaseSecurity.IsSysAdminOrDeveloper(Context))
                suggestedApps = suggestedApps?.Where(x => allApps.Any(y => x.Id == y.Id)).ToList();

            var hasMyAppDic = suggestedApps.ToDictionary(x => x.Id, x => MasterLocator.ApplicationService.HasMyApps(x));
            var userInfo = OAuthUserIdentityInfo.Create(Context.Login, Context.Role, Context.SchoolYearId, ChalkableAuthentication.GetSessionKey());
            var token = MasterLocator.ApplicationService.GetAccessToken(miniQuizApp.Id, ChalkableAuthentication.GetSessionKey());
            
            return Json(MiniQuizAppInfoViewData.Create(miniQuizApp, suggestedApps.Select(BaseApplicationViewData.Create).ToList(), allApps, 
                hasMyAppDic, Context.PersonId, token));
        }
    }
}