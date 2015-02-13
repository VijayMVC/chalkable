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
        public ActionResult List(int schoolYearId)
        {
            var res = SchoolLocator.PeriodService.GetPeriods(schoolYearId);
            return Json(PeriodViewData.Create(res));
        }
        //[AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit")]
        //public ActionResult Create(int schoolYearId, int startTime, int endTime, int sectionId)
        //{
        //    var generalPeriod = SchoolLocator.PeriodService.Add(,schoolYearId, startTime, endTime, sectionId, 0);
        //    var res = PeriodViewData.Create(generalPeriod);
        //    return Json(res, 3);
        //}

        //[AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit")]
        //public ActionResult Delete(int id)
        //{
        //    SchoolLocator.PeriodService.Delete(id);
        //    return Json(true);
        //}

        //[AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit")]
        //public ActionResult Update(int id, int startTime, int endTime)
        //{
        //    SchoolLocator.PeriodService.Edit(id, startTime, endTime);
        //    return Json(true);
        //}

        //[AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit")]
        //public ActionResult ReGeneratePeriods(GuidList markingPeriodIds, int startTime, int periodlength, int timeBetweenPeriods, int? periodCount)
        //{
        //    var res = SchoolLocator.PeriodService.ReGeneratePeriods(markingPeriodIds, startTime, periodlength, timeBetweenPeriods, periodCount);
        //    return Json(res.Select(PeriodDetailedViewData.Create));
        //}
    }
}