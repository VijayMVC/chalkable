using System;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ClassController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_CLASS_LIST, true, CallType.Post, new[] { AppPermissionType.Class })]
        public ActionResult List(Guid? schoolYearId, int? start, int? count)
        {
            var res = SchoolLocator.ClassService.GetClasses(schoolYearId, start ?? 0, count ?? 10);
            return Json(res.Transform(ClassViewData.Create));
        }
    }
}