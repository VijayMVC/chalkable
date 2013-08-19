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
    public class DisciplineTypeController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_DISCIPLINE_TYPE_LIST, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        public ActionResult List(int? start, int? count)
        {
            var res = SchoolLocator.DisciplineTypeService.GetDisciplineTypes(start ?? 0, count ?? 10);
            return Json(res.Transform(DisciplineTypeViewData.Create), 3);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit")]
        public ActionResult Create(string name, int score)
        {
            var res = SchoolLocator.DisciplineTypeService.Add(name, score);
            return Json(DisciplineTypeViewData.Create(res), 3);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit")]
        public ActionResult Update(Guid disciplineTypeId, string name, int score)
        {
            var res = SchoolLocator.DisciplineTypeService.Edit(disciplineTypeId, name, score);
            return Json(DisciplineTypeViewData.Create(res), 3);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit")]
        public ActionResult Delete(Guid disciplineTypeId)
        {
            SchoolLocator.DisciplineTypeService.Delete(disciplineTypeId);
            return Json(true);
        }
    }
}