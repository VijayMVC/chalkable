using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers.CalendarControllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementCalendarController : CalendarController
    {
         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_LIST, true, CallType.Get, new[] { AppPermissionType.Announcement })]
        public ActionResult List(DateTime? date, Guid? classId, GuidList gradeLevelIds)
         {
             if (!SchoolLocator.Context.SchoolId.HasValue)
                 throw new UnassignedUserException();
             DateTime start, end;
             MonthCalendar(ref date, out start, out end);
             var isAdmin = BaseSecurity.IsAdminViewer(SchoolLocator.Context);
             var announcements = SchoolLocator.AnnouncementService.GetAnnouncements(start, end, false, gradeLevelIds, !isAdmin ? classId : null);
             if (isAdmin)
                 announcements = announcements.Where(x => !x.ClassId.HasValue).ToList();
             var schoolYearId = GetCurrentSchoolYearId();
             var days = SchoolLocator.CalendarDateService.GetLastDays(schoolYearId, true, start, end);
             return Json(PrepareMonthCalendar(start, end, date.Value, (dateTime, isCurrentMonth) => 
                 AnnouncementMonthCalendarViewData.Create(dateTime, isCurrentMonth, announcements, days)));
         }

         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_ADMIN_DAY, true, CallType.Get, new[] { AppPermissionType.Schedule })]
         public ActionResult AdminDay(DateTime? day, GuidList gradeLevelIds)
         {
             var currentDay = day ?? SchoolLocator.Context.NowSchoolTime;
             var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(currentDay);
             if (mp != null)
             {
                 var classPeriods = SchoolLocator.ClassPeriodService.GetClassPeriods(currentDay, null, null, null, null);
                 var calendarDate = SchoolLocator.CalendarDateService.GetCalendarDateByDate(currentDay);
                 var periods = SchoolLocator.PeriodService.GetPeriods(mp.Id, calendarDate.ScheduleSectionRef);
                 var gradeLevels = SchoolLocator.GradeLevelService.GetGradeLevels();
                 if (gradeLevelIds != null && gradeLevelIds.Count > 0)
                     gradeLevels = gradeLevels.Where(x => gradeLevelIds.Contains(x.Id)).ToList();
                 var classes = SchoolLocator.ClassService.GetClasses(mp.SchoolYearRef, mp.Id, null);
                 var rooms = SchoolLocator.RoomService.GetRooms();
                 return Json(AdminDayCalendarViewData.Create(periods, gradeLevels, classPeriods, classes, rooms, currentDay), 5);
             }
             throw new NoMarkingPeriodException();
         }

         //TODO: rewrite this method 
         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_WEEK, true, CallType.Get, new[] { AppPermissionType.Announcement })]
         public ActionResult Week(DateTime? date, Guid? classId, GuidList gradeLevelIds)
         {
             DateTime start, end;
             Guid? teacherId, studentId;
             WeekCalendar(ref date, out start, out end);
             PreperingUsersIdsForCalendar(SchoolLocator, null, out teacherId, out studentId);
             var anns = SchoolLocator.AnnouncementService.GetAnnouncements(start, end, false, gradeLevelIds, classId);
             var rooms = SchoolLocator.RoomService.GetRooms();
             IList<ClassPeriod> classPeriods = new List<ClassPeriod>();
             IList<Period> periods = new List<Period>();
             var res = new List<AnnouncementCalendarWeekViewData>();
             var schoolYearId = GetCurrentSchoolYearId();
             var days = SchoolLocator.CalendarDateService.GetLastDays(schoolYearId, false, start, end);
             Guid? prevMpId = null;
             foreach (var d in days)
             {
                 if (!d.MarkingPeriodRef.HasValue || !d.ScheduleSectionRef.HasValue) continue;
                 if (d.MarkingPeriodRef != prevMpId)
                 {
                     periods = SchoolLocator.PeriodService.GetPeriods(d.MarkingPeriodRef.Value, null);
                     if (!BaseSecurity.IsAdminViewer(SchoolLocator.Context))
                         classPeriods = SchoolLocator.ClassPeriodService.GetClassPeriods(d.MarkingPeriodRef.Value, null, null, null, null, studentId, teacherId);
                     prevMpId = d.MarkingPeriodRef;
                 }
                 var announcements = anns.Where(x => x.Expires.Date == d.DateTime).ToList();
                 var annPeriods = AnnouncementPeriodViewData.Create(periods, classPeriods, d, announcements, rooms);
                 var ann = announcements.Where(x => !x.ClassId.HasValue || !x.GradableType).ToList();
                 res.Add(AnnouncementCalendarWeekViewData.Create(d.DateTime, annPeriods, ann));
             }
             return Json(res, 6);
         }

         private static void PreperingUsersIdsForCalendar(IServiceLocatorSchool locator, Guid? personId, out Guid? teacherId, out Guid? studentId)
         {
             if (!personId.HasValue)
             {
                 teacherId = locator.Context.Role == CoreRoles.TEACHER_ROLE ? locator.Context.UserId : (Guid?)null;
                 studentId = locator.Context.Role == CoreRoles.STUDENT_ROLE ? locator.Context.UserId : (Guid?)null;
             }
             else
             {
                 var person = locator.PersonService.GetPerson(personId.Value);
                 teacherId = person.RoleRef == CoreRoles.TEACHER_ROLE.Id ? person.Id : (Guid?)null;
                 studentId = person.RoleRef == CoreRoles.STUDENT_ROLE.Id ? person.Id : (Guid?)null;
             }
         }
        
    }
}