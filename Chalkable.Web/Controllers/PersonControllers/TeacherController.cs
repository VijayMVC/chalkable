using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
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
            var res = GetInfo(personId, null, TeacherInfoViewData.Create);
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
        public ActionResult TeachersStats(int schoolYearId, string filter, int? start, int? count, int? sortType)
        {
            var teachers = SchoolLocator.StaffService.GetTeachersStats(schoolYearId, filter, start, count, (TeacherSortType?) sortType);
            return Json(teachers.Select(TeacherStatsViewData.Create));
        }
    }
}