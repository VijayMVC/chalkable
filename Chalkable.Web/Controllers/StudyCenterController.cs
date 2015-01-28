using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class StudyCenterController : ChalkableController
    {
        [AuthorizationFilter("Teacher, Student", Preference.API_DESCR_SET_PRACTICE_GRADE, true, CallType.Post, new[] { AppPermissionType.Announcement })]
        public ActionResult SetPracticeGrade(int studentId, int standardId, Guid applicationId, string score)
        {
            SchoolLocator.PracticeGradeService.Add(standardId, studentId, applicationId, score);
            return Json(true);
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
    }
}