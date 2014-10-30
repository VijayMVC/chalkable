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
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_TEACHER_SUMMARY, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult Summary(int personId)
        {
            var teacher = SchoolLocator.PersonService.GetPerson(personId);
            var room = SchoolLocator.RoomService.WhereIsPerson(personId, SchoolLocator.Context.NowSchoolYearTime);
            return Json(TeacherSummaryViewData.Create(teacher, room));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Info(int personId)
        {
            var res = GetInfo(personId, TeacherInfoViewData.Create);
            return Json(res);
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_TEACHER_TEACHERS, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult GetTeachers(string filter, int? start, int? count, int? classId, bool? byLastName, bool? onlyMyTeachers)
        {
            var role = CoreRoles.TEACHER_ROLE.Name;
            return Json(PersonLogic.GetPersons(SchoolLocator, start, count, byLastName, filter, role, classId, null, null, onlyMyTeachers));
        }
    }
}