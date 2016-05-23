using System;
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
using Chalkable.Web.Models;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.Settings;

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

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult Panorama(int classId)
        {
            var c = SchoolLocator.ClassService.GetById(classId);
            var settings = SchoolLocator.PanoramaSettingsService.Get<ClassProfilePanoramaSettings>();

            //TODO remove this default data later
            
            var standardizedTests = new List<StandardizedTestDetails>();
            standardizedTests.Add(new StandardizedTestDetails
            {
                Id = 1,
                Name = "Test1",
                DisplayName = "Test1",
                Components = new List<StandardizedTestComponent>
                {
                    new StandardizedTestComponent {Id =1, Name = "Math", StandardizedTestRef = 1},
                    new StandardizedTestComponent {Id =2, Name = "English", StandardizedTestRef = 1},
                },
                ScoreTypes = new List<StandardizedTestScoreType>
                {
                    new StandardizedTestScoreType {Id = 1, StandardizedTestRef = 1, Name = "Numeric"},
                    new StandardizedTestScoreType {Id = 2, StandardizedTestRef = 1, Name = "Pass"},
                    new StandardizedTestScoreType {Id = 3, StandardizedTestRef = 1, Name = "Raw"}
                }
            });

            standardizedTests.Add(new StandardizedTestDetails
            {
                Id = 1,
                Name = "Test2",
                DisplayName = "Test2",
                Components = new List<StandardizedTestComponent>
                {
                    new StandardizedTestComponent {Id =3, Name = "Math2", StandardizedTestRef = 2},
                    new StandardizedTestComponent {Id =4, Name = "English2", StandardizedTestRef = 2},
                },
                ScoreTypes = new List<StandardizedTestScoreType>
                {
                    new StandardizedTestScoreType {Id = 4, StandardizedTestRef = 2, Name = "Numeric2"},
                    new StandardizedTestScoreType {Id = 5, StandardizedTestRef = 2, Name = "Pass2"},
                    new StandardizedTestScoreType {Id = 6, StandardizedTestRef = 2, Name = "Raw2"}
                }
            });
            
            return Json(ClassPanoramaViewData.Create(c, settings, standardizedTests));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult SavePanoramaSettings(ClassProfilePanoramaSettings settings)
        {
            SchoolLocator.PanoramaSettingsService.Save(settings);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult RestorePanoramaSettings()
        {
            var settings = SchoolLocator.PanoramaSettingsService.Restore<ClassProfilePanoramaSettings>();
            return Json(ClassProfilePanoramaSettingsViewData.Create(settings));
        }

    }
}