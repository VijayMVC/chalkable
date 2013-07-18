using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.ClassesViewData;

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

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ClassInfo(Guid classId)
        {
            var classData = SchoolLocator.ClassService.GetClassById(classId);
            var classGeneralPeriod = SchoolLocator.ClassPeriodService.GetNearestClassPeriod(classId, SchoolLocator.Context.NowSchoolTime);
            Room room = null;
            if (classGeneralPeriod != null)
            {
                var rooms = SchoolLocator.ClassPeriodService.GetAvailableRooms(classGeneralPeriod.PeriodRef);
                room = rooms.FirstOrDefault();
            }
            ChalkableDepartment department = null;
            if (classData.Course.ChalkableDepartmentRef.HasValue)
                department = MasterLocator.ChalkableDepartmentService.GetChalkableDepartmentById(classData.Course.ChalkableDepartmentRef.Value);
            return Json(ClassInfoViewData.Create(classData, room, department));
        }

    }
}