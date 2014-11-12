using System;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class StandardController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetStandards(int? classId, int? subjectId, int? gradeLevelId, int? parentStandardId, bool? allStandards)
        {
            var standards = SchoolLocator.StandardService.GetStandards(classId, gradeLevelId
                , subjectId, parentStandardId, allStandards ?? false);
            return Json(AnnouncementStandardViewData.Create(standards));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetStandardSubject()
        {
            var subjects = SchoolLocator.StandardService.GetStandardSubjects();
            return Json(StandardSubjectViewData.Create(subjects));
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult GetCommonCoreStandardCategory(Guid? parentCategoryId, bool? allCategories)
        {
            var standardCategories = MasterLocator.CommonCoreStandardService.GetCCStandardCategories(parentCategoryId, allCategories ?? false);
            return Json(CCStandardCategoryViewData.Create(standardCategories));
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult GetCommonCoreStandard(Guid? standardCategoryId)
        {
            var standards = MasterLocator.CommonCoreStandardService.GetStandards(standardCategoryId);
            return Json(CommonCoreStandardViewData.Create(standards));
        }
    }
}