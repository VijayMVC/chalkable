using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class GradingController : ChalkableController
    {
        [AuthorizationFilter("Teacher")]
        public ActionResult TeacherSummary(Guid teacherId)
        {
            var schoolYearId = GetCurrentSchoolYearId();
            var gradingStats = SchoolLocator.GradingStatisticService.GetStudentGradePerClass(teacherId, schoolYearId);
            gradingStats = gradingStats.Where(x => x.Avg.HasValue).ToList();
            var classes = SchoolLocator.ClassService.GetClasses(null, null, teacherId);
            return Json(GradingTeacherClassSummaryViewData.Create(gradingStats, classes), 6);
        }
    }
}