using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    [RequireHttps, TraceControllerFilter]
    public class TeacherController : PersonController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]
        public ActionResult Summary(int personId)
        {
            var teacher = SchoolLocator.StaffService.GetStaff(personId);
            var room = SchoolLocator.RoomService.WhereIsPerson(personId, SchoolLocator.Context.NowSchoolYearTime);
            return Json(TeacherSummaryViewData.Create(teacher, room));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Info(int personId)
        {
            var res = GetInfo(personId, TeacherInfoViewData.Create);
            if (Context.PersonId == personId) //just for teacher user
                res.Email = MasterLocator.UserService.GetUserEmailById(Context.UserId);
            return Json(res);
        }

        protected override bool CanGetInfo(int personId)
        {
            return Context.PersonId == personId;
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Schedule(int personId)
        {
            var staff = SchoolLocator.StaffService.GetStaff(personId);
            return Json(PrepareScheduleData(StaffViewData.Create(staff)));
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]
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

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult TeachersStats(int schoolYearId, string filter, int? start, int? count)
        {
            var teachers = SchoolLocator.StaffService.SearchStaff(schoolYearId, null, null, filter, true, start ?? 0,
                count ?? int.MaxValue);

            var classes = SchoolLocator.ClassService.GetClassesByTeachers(schoolYearId,
                teachers.Select(x => x.Id).ToList(), start, count);

            return Json(TeacherStatsViewData.Create(teachers, classes));
        }
    }
}