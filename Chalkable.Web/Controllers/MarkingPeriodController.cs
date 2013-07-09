using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class MarkingPeriodController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_MARKING_PERIOD_LIST, true, CallType.Get, new[] { AppPermissionType.Schedule })]
        public ActionResult List(Guid schoolYearId, DateTime? tillDate)
        {
            var res = SchoolLocator.MarkingPeriodService.GetMarkingPeriods(schoolYearId);
            if (tillDate.HasValue)
                res = res.Where(x => x.StartDate <= tillDate).ToList();
            return Json(MarkingPeriodViewData.Create(res));
        }



        //[AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit")]
        //public ActionResult ChangeMarkingPeriodsDate(Guid? previosMarkingPeriodId, Guid currentMarkingPeriodId
        //    , Guid? nextMarkingPeriodId, DateTime currentMarkingPeriodStartDate)
        //{

        //    //SchoolLocator.MarkingPeriodService.EditMarkingPeriodDates(leftMarkingPeriodId, rightMarkingPeriodId, date);
        //    return Json(true);
        //}

        [AuthorizationFilter("System Admin, AdminGrade, AdminEdit")]
        public ActionResult ChangeMarkingPeriodsCount(Guid schoolYearId, int count)
        {
            string error;
            var res = SchoolLocator.MarkingPeriodService.ChangeMarkingPeriodsCount(schoolYearId, count, out error);
            return string.IsNullOrEmpty(error) ? Json(res) : Json(new ChalkableException(error));
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit")]
        public ActionResult ChangeWeekDays(GuidList markingPeriodIds, int weekDays)
        {
            var res = SchoolLocator.MarkingPeriodService.ChangeWeekDays(markingPeriodIds, weekDays);
            return Json(res);
        }
    }
}