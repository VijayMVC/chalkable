using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Controllers.CalendarControllers;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ClassController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Class })]
        public ActionResult List(int? schoolYearId, int? markingPeriodId, int? personId, int? start, int? count)//TODO: remove pagination from there
        {
            IList<ClassDetails> res;
            Trace.Assert(Context.SchoolYearId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            if (Context.RoleId == CoreRoles.TEACHER_ROLE.Id)
                res = SchoolLocator.ClassService.GetTeacherClasses(Context.SchoolYearId.Value, Context.PersonId.Value, markingPeriodId);
            else if (Context.RoleId == CoreRoles.STUDENT_ROLE.Id)
                res = SchoolLocator.ClassService.GetStudentClasses(Context.SchoolYearId.Value, Context.PersonId.Value, markingPeriodId);
            else
                throw new NotImplementedException();
            start = start ?? 0;
            count = count ?? 10;
            var pl = new PaginatedList<ClassDetails>(res, start.Value/count.Value, count.Value, res.Count);
            return Json(pl.Transform(ClassViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin, Teacher, Student")]
        public ActionResult ClassInfo(int classId)
        {
            var classData = SchoolLocator.ClassService.GetClassDetailsById(classId);
            Room room = null;
            if (classData.RoomRef.HasValue)
                room = SchoolLocator.RoomService.GetRoomById(classData.RoomRef.Value);
            ChalkableDepartment department = null;
            if (classData.ChalkableDepartmentRef.HasValue)
                department = MasterLocator.ChalkableDepartmentService.GetChalkableDepartmentById(classData.ChalkableDepartmentRef.Value);
            return Json(ClassInfoViewData.Create(classData, room, department));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult ClassGrading(int classId)
        {
            var classData = SchoolLocator.ClassService.GetClassDetailsById(classId);
            var canCreateItem = SchoolLocator.Context.PersonId == classData.PrimaryTeacherRef;
            var gradingPerMp = ClassLogic.GetGradingSummary(SchoolLocator, classId, GetCurrentSchoolYearId(), null, null, canCreateItem);
            return Json(ClassGradingViewData.Create(classData, gradingPerMp), 8);
        }

        [AuthorizationFilter("System Admin, DistrictAdmin, Teacher, Student")]
        public ActionResult ClassSchedule(int classId, DateTime? date)
        {
            var clazz = SchoolLocator.ClassService.GetClassDetailsById(classId);
            var schedule = AnnouncementCalendarController.BuildDayAnnCalendar(SchoolLocator, date, classId, null, GetCurrentSchoolYearId());
            return Json(ClassScheduleViewData.Create(clazz, schedule), 13);
        }
        
        [AuthorizationFilter("System Admin, DistrictAdmin, Teacher, Student")]
        public ActionResult ClassAttendance(int classId)
        {
            var c = SchoolLocator.ClassService.GetClassDetailsById(classId);
            var attendanceSummary = SchoolLocator.AttendanceService.GetClassAttendanceSummary(classId, null);
            return Json(ClassAttendanceSummaryViewData.Create(c, attendanceSummary));
        }

        [AuthorizationFilter("System Admin, DistrictAdmin, Teacher, Student")]
        public ActionResult Explorer(int classId)
        {
            var c = SchoolLocator.ClassService.GetClassDetailsById(classId);
            var gradingStandards = SchoolLocator.GradingStandardService.GetGradingStandards(classId, null, false);
            return Json(ClassExpolorerViewData.Create(c, gradingStandards));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DetailedCourseTypes(int schoolYearId, int gradeLevelId)
        {
            var courses = SchoolLocator.ClassService.GetAdminCourses(schoolYearId, gradeLevelId);
            var courseTypes = SchoolLocator.CourseTypeService.GetList(true);
            return Json(CourseTypeDetailsViewData.Create(courses, courseTypes));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult AllSchoolsActiveClasses()
        {
            var classes = SchoolLocator.ClassService.GetAllSchoolsActiveClasses();
            var courseTypes = SchoolLocator.CourseTypeService.GetList(false);
            var schools = SchoolLocator.SchoolService.GetSchools();
            var res = AllSchoolsActiveClassesViewData.Create(classes, courseTypes, schools);
            return Json(res);
        }

    }
}