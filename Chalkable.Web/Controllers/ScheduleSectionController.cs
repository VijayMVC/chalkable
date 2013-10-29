using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    public class ScheduleSectionController : ChalkableController
    {
        [AuthorizationFilter]
        public ActionResult List(int schoolYearId)
        {
            var res = SchoolLocator.ScheduleSectionService.GetSections(schoolYearId);
            return Json(res.Select(DateTypeViewData.Create));
        }

        //[AuthorizationFilter("System Admin, AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_SCHEDULE_SECTION_LIST_FOR_MARKING_PERIODS, true, CallType.Get, new[] { AppPermissionType.Schedule })]
        //public ActionResult ListForMarkingPeriods(GuidList markingPeriodIds)
        //{
        //    var markingPeriod = SchoolLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodIds[0]);
        //    var canGetSectoions = SchoolLocator.ScheduleSectionService.CanGetSection(markingPeriodIds);
        //    IList<ScheduleSection> sections = new List<ScheduleSection>();
        //    if (canGetSectoions)
        //    {
        //        sections = SchoolLocator.ScheduleSectionService.GetSections(markingPeriodIds);
        //    }
        //    var canchange = canGetSectoions && SchoolLocator.ScheduleSectionService.CanDeleteSections(markingPeriodIds);
        //    return Json(SectionsForMPsViewData.Create(sections, markingPeriod, canGetSectoions, canchange));
        //}

        //[AuthorizationFilter("System Admin, AdminGrade, AdminEdit")]
        //public ActionResult ChangeSections(StringList sections, GuidList markingPeriodIds)
        //{
        //    SchoolLocator.ScheduleSectionService.ReBuildSections(sections, markingPeriodIds);
        //    return Json(true);
        //}
    }
}