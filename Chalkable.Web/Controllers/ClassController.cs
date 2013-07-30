﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.CalendarsViewData;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class ClassController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_CLASS_LIST, true, CallType.Post, new[] { AppPermissionType.Class })]
        public ActionResult List(Guid? schoolYearId, int? start, int? count)
        {
            var res = SchoolLocator.ClassService.GetClasses(schoolYearId, start ?? 0, count ?? DEFAULT_PAGE_SIZE);
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

        [AuthorizationFilter("System Admin, AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ClassSummary(Guid classId)
        {
            var c = SchoolLocator.ClassService.GetClassById(classId);
            var currentDateTime = SchoolLocator.Context.NowSchoolTime;
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(currentDateTime, true);
            if (mp == null)
                return Json(ClassViewData.Create(c));
            
            var curentClassPeriod = SchoolLocator.ClassPeriodService.GetNearestClassPeriod(classId, currentDateTime);
            Room curentRoom = null;
            if(curentClassPeriod != null)
               curentRoom = SchoolLocator.RoomService.GetRoomById(curentClassPeriod.RoomRef);
            var students = SchoolLocator.PersonService.GetPaginatedPersons(new PersonQuery
                {
                    ClassId = classId, 
                    RoleId = CoreRoles.STUDENT_ROLE.Id
                });
            if (c.TeacherRef != SchoolLocator.Context.UserId && !BaseSecurity.IsAdminViewer(SchoolLocator.Context))
                return Json(ClassSummaryViewData.Create(c, curentRoom, students));

            var possibleAbsents = SchoolLocator.AttendanceService.PossibleAttendanceCount(mp.Id, classId, SchoolLocator.Context.NowSchoolTime.Date);
            var classAttendances = SchoolLocator.AttendanceService.GetClassAttendanceComplex(new ClassAttendanceQuery
                {
                    MarkingPeriodId = mp.Id,
                    ClassId = classId,
                    Type = AttendanceTypeEnum.Absent | AttendanceTypeEnum.Excused | AttendanceTypeEnum.Present | AttendanceTypeEnum.Late
                });
            var disciplineTypes = SchoolLocator.DisciplineTypeService.GetDisciplineTypes(0, int.MaxValue);
            var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(new ClassDisciplineQuery
            {
                SchoolYearId = mp.SchoolYearRef,
                ClassId = classId,
                MarkingPeriodId = mp.Id,
            });
            
            var dates = SchoolLocator.CalendarDateService.GetLastDays(mp.Id, true, currentDateTime.Date, null, 9).Select(x => x.DateTime).ToList();
            IList<AnnouncementComplex> anns = new List<AnnouncementComplex>();
            if (dates.Count > 0)
                anns = SchoolLocator.AnnouncementService.GetAnnouncements(currentDateTime.Date, dates.Last().Date, false, null, classId);
            var annsByDate = AnnouncementByDateViewData.Create(dates, anns);
            var gradePerMps = SchoolLocator.GradingStatisticService.GetClassGradeAvgPerMP(classId, mp.SchoolYearRef, null, null);
            gradePerMps = gradePerMps.Where(x => x.MarkingPeriod.StartDate <= mp.StartDate).ToList();
            return Json(ClassSummaryViewData.Create(c, curentRoom, students, annsByDate, classAttendances, possibleAbsents
                , disciplines, disciplineTypes, gradePerMps));
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ClassGrading(Guid classId)
        {
            var classData = SchoolLocator.ClassService.GetClassById(classId);
            var canCreateItem = SchoolLocator.Context.UserId == classData.TeacherRef;
            var gradingPerMp = ClassLogic.GetGradingSummary(SchoolLocator, classId, GetCurrentSchoolYearId(), null, null, canCreateItem);
            return Json(ClassGradingViewData.Create(classData, gradingPerMp), 8);
        }

        public static List<int> BuildClassUsageMask(IServiceLocatorSchool locator, Guid classId, Guid markingPeriodId, string timeZoneId)
        {
            var res = new List<int>();
            /**
                 * NON = 0
                 * ONE_OF = 1
                 * ALL = 2
                 */
            DateTime now = DateTime.UtcNow.ConvertFromUtc(timeZoneId);
            IList<DateTime> dates = new List<DateTime>();
            for (int i = 0; i < 30; i++)
            {
                dates.Add(now.Date);
                now = now.AddDays(1);
            }
            var days = locator.CalendarDateService.GetDays(markingPeriodId, true);
            var classPeriods = locator.ClassPeriodService.GetClassPeriods(markingPeriodId, classId, null, null, null);
            for (int i = 0; i < dates.Count; i++)
            {
                bool exists = false;
                var d = days.FirstOrDefault(x => x.ScheduleSectionRef.HasValue && x.DateTime == dates[i]);
                if (d != null)
                {
                    exists = classPeriods.Any(x => x.Period.SectionRef == d.ScheduleSectionRef && x.ClassRef == classId);
                }
                res.Add(exists ? 2 : 0);
            }
            return res;
        }

    }
}