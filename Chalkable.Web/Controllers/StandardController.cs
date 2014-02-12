using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class StandardController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetStandards(int? classId, int? subjectId, int? gradeLevelId)
        {
            var standards = SchoolLocator.StandardService.GetStandardes(classId, gradeLevelId, subjectId);
            return Json(AnnouncementStandardViewData.Create(standards));
        }
    }
}