using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class StandardController : ChalkableController
    {

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SearchStandards(string filter, int? classId, bool? activeOnly, bool? deepest)
        {
            var stadnards = SchoolLocator.StandardService.GetStandards(filter, classId, activeOnly ?? false, deepest);
            return Json(stadnards.Select(StandardViewData.Create).ToList());
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult GetStandards(int? classId, int? subjectId, int? gradeLevelId, int? parentStandardId, bool? allStandards, bool? activeOnly)
        {
            var standards = SchoolLocator.StandardService.GetStandards(classId, gradeLevelId
                , subjectId, parentStandardId, allStandards ?? false, activeOnly ?? false);
            return Json(standards.Select(StandardViewData.Create).ToList());
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult GetStandardParentsSubTree(int standardId, int? classId)
        {
            var res = SchoolLocator.StandardService.GetStandardParentsSubTree(standardId, classId);
            return Json(StandardsTableViewData.Create(res), 6);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult GetStandardsByIds(IntList ids)
        {
            var standards = SchoolLocator.StandardService
                .GetStandards(ids)
                .OrderBy(np => ids.IndexOf(np.Id))
                .ToList();
            return Json(standards.Select(StandardViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult GetStandardSubject(int? classId)
        {
            var subjects = SchoolLocator.StandardService.GetStandardSubjects(classId);
            return Json(StandardSubjectViewData.Create(subjects));
        }      
    }
}