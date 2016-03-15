﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
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

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult Summary(int classId)
        {
            var clazz = SchoolLocator.ClassService.GetClassDetailsById(classId);
            Room classRoom = null;

            if (clazz?.RoomRef != null)
                classRoom = SchoolLocator.RoomService.GetRoomById(clazz.RoomRef.Value);

            var classPeriods = SchoolLocator.PeriodService.GetPeriods(clazz?.ClassPeriods.Select(x => x.PeriodRef).ToList());
            var classDayTypes = SchoolLocator.DayTypeService.GetDayTypes(clazz?.ClassPeriods.Select(x => x.DayTypeRef).ToList());

            return Json(ClassSummaryViewData.Create(clazz, classRoom, classPeriods, classDayTypes));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult Grading(int classId)
        {
            var classDetails = SchoolLocator.ClassService.GetClassDetailsById(classId);
            var alphaGrades = classDetails.GradingScaleRef.HasValue
                ? SchoolLocator.AlphaGradeService.GetAlphaGradesByClassId(classId)
                : SchoolLocator.AlphaGradeService.GetAlphaGrades();
            var alphaGradesForStandards = SchoolLocator.AlphaGradeService.GetStandardsAlphaGradesByClassId(classId);
            if (alphaGradesForStandards.Count == 0 && Context.SchoolLocalId.HasValue)
                alphaGradesForStandards = SchoolLocator.AlphaGradeService.GetStandardsAlphaGradesForSchool(Context.SchoolLocalId.Value);
            return Json(ClassAlphaGradesViewData.Create(classDetails, alphaGrades, alphaGradesForStandards));
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

        [AuthorizationFilter("System Admin, DistrictAdmin, Teacher, Student")]
        public ActionResult ClassSchedule(int classId, DateTime? date)
        {
            var clazz = SchoolLocator.ClassService.GetClassDetailsById(classId);
            var schedule = AnnouncementCalendarController.BuildDayAnnCalendar(SchoolLocator, date, classId, null, GetCurrentSchoolYearId());
            return Json(ClassScheduleViewData.Create(clazz, schedule), 13);
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> AttendanceSummary(int classId, int? dateType)
        {
            var datePeriodType = ((ClassLogic.DatePeriodTypeEnum?)dateType) ?? ClassLogic.DatePeriodTypeEnum.Year;
            return await Json(ClassLogic.GetClassAttendanceSummary(classId, datePeriodType, SchoolLocator));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> DisciplineSummary(int classId, int? dateType)
        {
            var datePeriodType = ((ClassLogic.DatePeriodTypeEnum?)dateType) ?? ClassLogic.DatePeriodTypeEnum.Year;
            return await Json(ClassLogic.GetClassDisciplineSummary(classId, datePeriodType, SchoolLocator));
        }

        [AuthorizationFilter("System Admin, DistrictAdmin, Teacher, Student")]
        public async Task<ActionResult> Explorer(int classId)
        {
            var c = SchoolLocator.ClassService.GetClassDetailsById(classId);
            var gradingStandardsTask = SchoolLocator.GradingStandardService.GetGradingStandards(classId, null, false);
            MasterLocator.UserTrackingService.UsedStandardsExplorer(Context.Login, "class explorer");
            return Json(ClassExpolorerViewData.Create(c, await gradingStandardsTask));
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

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult ClassesStats(int schoolYearId, string filter, int? start, int? count, int? teacherId, int? sortType)
        {
            var classes = SchoolLocator.ClassService.GetClassesBySchoolYear(schoolYearId, start, count, filter, teacherId,(ClassSortType?) sortType);
            return Json(classes.Select(ClassStatsViewData.Create));
        }   
    }
}