using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    public class PeriodController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, Teacher", Preference.API_DESCR_GENERAL_PERIOD_LIST, true, CallType.Get, new[] { AppPermissionType.Schedule })]
        public ActionResult List(Guid markingPeriodId, Guid? sectionId)
        {
            var res = SchoolLocator.PeriodService.GetPeriods(markingPeriodId, sectionId);
            return Json(PeriodDetailedViewData.Create(res));
        }
        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit")]
        public ActionResult Create(Guid markingPeriodId, int startTime, int endTime, Guid sectionId)
        {
            var generalPeriod = SchoolLocator.PeriodService.Add(markingPeriodId, startTime, endTime, sectionId, 0);
            var res = PeriodDetailedViewData.Create(generalPeriod);
            return Json(res, 3);
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit")]
        public ActionResult Delete(Guid id)
        {
            SchoolLocator.PeriodService.Delete(id);
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit")]
        public ActionResult Update(Guid id, int startTime, int endTime)
        {
            SchoolLocator.PeriodService.Edit(id, startTime, endTime);
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit")]
        public ActionResult ReGeneratePeriods(GuidList markingPeriodIds, int startTime, int periodlength, int timeBetweenPeriods, int? periodCount)
        {
            var res = SchoolLocator.PeriodService.ReGeneratePeriods(markingPeriodIds, startTime, periodlength, timeBetweenPeriods, periodCount);
            return Json(res.Select(PeriodDetailedViewData.Create));
        }
    }
}