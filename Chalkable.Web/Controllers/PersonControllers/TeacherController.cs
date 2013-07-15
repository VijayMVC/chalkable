using System;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    [RequireHttps, TraceControllerFilter]
    public class TeacherController : PersonController
    {
        [RequireRequestValue("personId")]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_TEACHER_SUMMARY, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult Summary(Guid personId)
        {
            var teacher = SchoolLocator.PersonService.GetPerson(personId);
            var room = SchoolLocator.RoomService.WhereIsPerson(personId, SchoolLocator.Context.NowSchoolTime);
            return Json(TeacherSummaryViewData.Create(teacher, room));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Info(Guid personId)
        {
            var res = GetInfo(personId, TeacherInfoViewData.Create);
            return Json(res);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetTeachers(string filter, int? start, int? count, Guid? classId, int? sortType)
        {
            var role = CoreRoles.TEACHER_ROLE.Name;
            return Json(PersonLogic.GetPersons(SchoolLocator, start, count, sortType, filter, role, classId));
        }

    }
}