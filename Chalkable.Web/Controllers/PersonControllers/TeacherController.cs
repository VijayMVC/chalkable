using System.Diagnostics;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    [RequireHttps, TraceControllerFilter]
    public class TeacherController : PersonController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_TEACHER_SUMMARY, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult Summary(int personId)
        {
            var teacher = SchoolLocator.StaffService.GetStaff(personId);
            var room = SchoolLocator.RoomService.WhereIsPerson(personId, SchoolLocator.Context.NowSchoolYearTime);
            return Json(TeacherSummaryViewData.Create(teacher, room));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Info(int personId)
        {
            var res = GetInfo(personId, TeacherInfoViewData.Create);
            return Json(res);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Schedule(int personId)
        {
            var staff = SchoolLocator.StaffService.GetStaff(personId);
            return Json(PrepareScheduleData(StaffViewData.Create(staff)));
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_TEACHER_TEACHERS, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult GetTeachers(string filter, int? start, int? count, int? classId, bool? byLastName, bool? onlyMyTeachers)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            int? studentId = null;
            if (onlyMyTeachers == true)
                studentId = Context.PersonId;
            var res = SchoolLocator.StaffService.SearchStaff(Context.SchoolYearId.Value, classId, studentId, filter,
                byLastName == false, start ?? 0, count ?? 10);
            return Json(res.Transform(StaffViewData.Create));
        }
    }
}