using System;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    public class AdminController : PersonController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Info(Guid personId)
        {
            var res = GetInfo(personId, TeacherInfoViewData.Create);
            return Json(res);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult UpdateInfo(AdminTeacherInputModel model)
        {
            var teacher = UpdateTeacherOrAdmin(model);
            //MixPanelService.ChangedEmail(SchoolLocator.Context., email);
            return Json(GetInfo(teacher.Id, TeacherInfoViewData.Create));
        }

    }
}