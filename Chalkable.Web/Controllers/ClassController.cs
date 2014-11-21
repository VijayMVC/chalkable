using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Controllers.CalendarControllers;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.CalendarsViewData;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ClassController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_CLASS_LIST, true, CallType.Post, new[] { AppPermissionType.Class })]
        public ActionResult List(int? schoolYearId, int? markingPeriodId, int? personId, int? start, int? count)
        {
            var res = SchoolLocator.ClassService.GetClasses(schoolYearId, markingPeriodId, personId, start ?? 0, count ?? DEFAULT_PAGE_SIZE);
            return Json(res.Transform(ClassViewData.Create));
        }

        [AuthorizationFilter("SysAdmin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
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

        [Obsolete]
        [AuthorizationFilter("System Admin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ClassSummary(int classId)
        {
            var c = SchoolLocator.ClassService.GetClassDetailsById(classId);
            var currentDateTime = SchoolLocator.Context.NowSchoolYearTime;
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(currentDateTime, true);
            if (mp == null)
                return Json(ClassViewData.Create(c));
            Room curentRoom = null;
            if (c.RoomRef.HasValue)
                curentRoom = SchoolLocator.RoomService.GetRoomById(c.RoomRef.Value);
            var students = SchoolLocator.PersonService.GetClassStudents(classId, mp.Id);
            if (!BaseSecurity.IsAdminViewerOrClassTeacher(c, Context))
                return Json(ClassSummaryViewData.Create(c, curentRoom, students));
            var disciplineTypes = SchoolLocator.InfractionService.GetInfractions();
            var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(new ClassDisciplineQuery
            {
                SchoolYearId = mp.SchoolYearRef,
                ClassId = classId,
                MarkingPeriodId = mp.Id,
            });

            var dates = SchoolLocator.CalendarDateService.GetLastDays(mp.SchoolYearRef, true, currentDateTime.Date, currentDateTime.Date.AddDays(8), 9).Select(x => x.Day).ToList();
            IList<AnnouncementComplex> anns = new List<AnnouncementComplex>();
            if (dates.Count > 0)
                anns = SchoolLocator.AnnouncementService.GetAnnouncements(currentDateTime.Date, dates.Last().Date, false, null, classId);
            var annsByDate = AnnouncementByDateViewData.Create(dates, anns);
            var gradePerMps = SchoolLocator.GradingStatisticService.GetClassGradeAvgPerMP(classId, mp.SchoolYearRef, null, null);
            gradePerMps = gradePerMps.Where(x => x.MarkingPeriod.StartDate <= mp.StartDate).ToList();
            return Json(ClassSummaryViewData.Create(c, curentRoom, students, annsByDate, null, 0, disciplines, disciplineTypes, gradePerMps));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ClassGrading(int classId)
        {
            var classData = SchoolLocator.ClassService.GetClassDetailsById(classId);
            var canCreateItem = SchoolLocator.Context.PersonId == classData.PrimaryTeacherRef;
            var gradingPerMp = ClassLogic.GetGradingSummary(SchoolLocator, classId, GetCurrentSchoolYearId(), null, null, canCreateItem);
            return Json(ClassGradingViewData.Create(classData, gradingPerMp), 8);
        }

        [AuthorizationFilter("System Admin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ClassSchedule(int classId, DateTime? day)
        {
            var clazz = SchoolLocator.ClassService.GetClassDetailsById(classId);
            var schedule = AnnouncementCalendarController.BuildDayAnnCalendar(SchoolLocator, day, classId, null, GetCurrentSchoolYearId());
            return Json(ClassScheduleViewData.Create(clazz, schedule), 13);
        }
        
        [AuthorizationFilter("System Admin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ClassAttendance(int classId)
        {
            var c = SchoolLocator.ClassService.GetClassDetailsById(classId);
            return Json(ClassAttendanceSummaryViewData.Create(c)); //TODO: create ClassAttendanceSummaryViewData 
        }
    }
}