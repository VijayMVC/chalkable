using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DisciplineTypeController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_DISCIPLINE_TYPE_LIST, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        public ActionResult List(int? start, int? count)
        {
            var res = SchoolLocator.InfractionService.GetDisciplineTypes();
            return Json(res, 3);
        }
    }
}