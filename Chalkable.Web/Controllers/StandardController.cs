using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class StandardController : ChalkableController
    {

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult SearchStandards(string filter)
        {
            var stadnards = SchoolLocator.StandardService.GetStandards(filter);
            return Json(stadnards.Select(StandardViewData.Create).ToList());
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetStandards(int? classId, int? subjectId, int? gradeLevelId, int? parentStandardId, bool? allStandards)
        {
            var standards = SchoolLocator.StandardService.GetStandards(classId, gradeLevelId
                , subjectId, parentStandardId, allStandards ?? false);
            return Json(standards.Select(StandardViewData.Create).ToList());
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetStandardsByIds(IntList ids)
        {
            var standards = SchoolLocator.StandardService
                .GetStandards(ids)
                .OrderBy(np => ids.IndexOf(np.Id))
                .ToList();
            return Json(standards.Select(StandardViewData.Create));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetStandardSubject(int? classId)
        {
            var subjects = SchoolLocator.StandardService.GetStandardSubjects(classId);
            return Json(StandardSubjectViewData.Create(subjects));
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult GetCommonCoreStandardCategories()
        {
            var standardCategories = MasterLocator.CommonCoreStandardService.GetCCStandardCategories();
            return Json(CCStandardCategoryViewData.Create(standardCategories));
        }

        [AuthorizationFilter("SysAdmin, Developer")]
        public ActionResult GetCommonCoreStandards(Guid? standardCategoryId, Guid? parentStandardId, bool? allStandards)
        {
            var standards = MasterLocator.CommonCoreStandardService.GetStandards(standardCategoryId, parentStandardId, allStandards ?? false);
            return Json(CommonCoreStandardViewData.Create(standards));
        }

        public ActionResult GetCommonCoreStandardsByIds(GuidList standardsIds)
        {
            var standards = MasterLocator.CommonCoreStandardService.GetStandardsByIds(standardsIds ?? new List<Guid>());
            return Json(CommonCoreStandardViewData.Create(standards));
        }
    }
}