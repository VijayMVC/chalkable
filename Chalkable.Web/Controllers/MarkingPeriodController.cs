using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class MarkingPeriodController : ChalkableController
    {
        [AuthorizationFilter("System Admin, AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_MARKING_PERIOD_LIST, true, CallType.Get, new[] { AppPermissionType.Schedule })]
        public ActionResult List(Guid schoolYearId, DateTime? tillDate)
        {
            var res = SchoolLocator.MarkingPeriodService.GetMarkingPeriods(schoolYearId);
            if (tillDate.HasValue)
                res = res.Where(x => x.StartDate <= tillDate).ToList();
            return Json(MarkingPeriodViewData.Create(res));
        }
    }
}