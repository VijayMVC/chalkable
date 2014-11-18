using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.CalendarsViewData;

namespace Chalkable.Web.Controllers.CalendarControllers
{
    [RequireHttps, TraceControllerFilter]
    public class AnnouncementCalendarController : CalendarController
    {
         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_LIST, true, CallType.Get, new[] { AppPermissionType.Announcement })]
        public ActionResult List(DateTime? date, int? classId, IntList gradeLevelIds, int? schoolPersonId)
         {
             if (!SchoolLocator.Context.PersonId.HasValue)
                 throw new UnassignedUserException();
             DateTime start, end;
             MonthCalendar(ref date, out start, out end);
             var isAdmin = BaseSecurity.IsAdminViewer(SchoolLocator.Context);
             var schoolYearId = GetCurrentSchoolYearId();
             var announcements = SchoolLocator.AnnouncementService.GetAnnouncements(start, end, false, gradeLevelIds, !isAdmin ? classId : null);
             if (schoolPersonId.HasValue)
             {
                 var classes = SchoolLocator.ClassService.GetClasses(schoolYearId, null, schoolPersonId);
                 announcements = announcements.Where(x => classes.Any(y => y.Id == x.ClassRef)).ToList();
             }
             var days = SchoolLocator.CalendarDateService.GetLastDays(schoolYearId, true, start, end);
             return Json(PrepareMonthCalendar(start, end, date.Value, (dateTime, isCurrentMonth) => 
                 AnnouncementMonthCalendarViewData.Create(dateTime, isCurrentMonth, announcements, days)));
         }

         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
         public ActionResult ListForWeek(DateTime? date)
         {
             DateTime start, end;
             var dates = GetWeekDates(ref date, out start, out end);
             var anns = SchoolLocator.AnnouncementService.GetAnnouncements(start, end);
             var listDayAnnouncements = AnnouncementByDateViewData.Create(dates, anns);
             return Json(listDayAnnouncements);
         }

         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
         public ActionResult ListByDateRange(DateTime? startDate, DateTime? endDate, int? classId)
         {
             var query = new AnnouncementsQuery {FromDate = startDate, ToDate = endDate, ClassId = classId};
             var anns = SchoolLocator.AnnouncementService.GetAnnouncementsComplex(query);
             return Json(AnnouncementShortViewData.Create(anns));
         }
        

         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_ADMIN_DAY, true, CallType.Get, new[] { AppPermissionType.Schedule })]
         public ActionResult AdminDay(DateTime? day, IntList gradeLevelIds)
         {
             throw new NotImplementedException();
         }

         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_WEEK, true, CallType.Get, new[] { AppPermissionType.Announcement })]
         public ActionResult Week(DateTime? date, int? classId, int? schoolPersonId)
         {

             var res = BuildDayAnnCalendar(SchoolLocator, date, classId, schoolPersonId, GetCurrentSchoolYearId());
             return Json(res, 8);
         }

         [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_ANNOUNCEMENT_CALENDAR_ANNOUNCEMENT_DAY, true, CallType.Get, new[] { AppPermissionType.Schedule })]
         public ActionResult Day(DateTime? date, int? schoolPersonId)
         {
             var res = BuildDayAnnCalendar(SchoolLocator, date, null, schoolPersonId, GetCurrentSchoolYearId());
             return Json(res, 8);
         }

         public static IList<AnnouncementCalendarWeekViewData> BuildDayAnnCalendar(IServiceLocatorSchool locator, DateTime? date,
             int? classId, int? personId, int schoolYearId)
         {

             DateTime start, end;
             WeekCalendar(ref date, out start, out end, locator.Context);
             int? teacherId, studentId;
             PrepareUsersIdsForCalendar(locator, personId, out teacherId, out studentId);
             var res = new List<AnnouncementCalendarWeekViewData>();
             var announcements = locator.AnnouncementService.GetAnnouncements(start, end, false, null, classId);
             var schedule = locator.ClassPeriodService.GetSchedule(teacherId, studentId, classId, start, end);
                    
             for (var d = start; d <= end; d = d.AddDays(1))
             {
                 var currentDayAnns = announcements.Where(x => x.Expires.Date == d).ToList();
                 var daySchedule = schedule.Where(x => x.Day == d).ToList();
                 var annPeriods = AnnouncementPeriodViewData.Create(daySchedule, currentDayAnns);
                 res.Add(AnnouncementCalendarWeekViewData.Create(d, annPeriods, currentDayAnns));
             }
             return res;
         }


         private static void PrepareUsersIdsForCalendar(IServiceLocatorSchool locator, int? personId, out int? teacherId, out int? studentId)
         {
             if (!personId.HasValue)
             {
                 teacherId = locator.Context.Role == CoreRoles.TEACHER_ROLE ? locator.Context.PersonId : null;
                 studentId = locator.Context.Role == CoreRoles.STUDENT_ROLE ? locator.Context.PersonId : null;
             }
             else
             {
                 var person = locator.PersonService.GetPerson(personId.Value);
                 teacherId = person.RoleRef == CoreRoles.TEACHER_ROLE.Id ? person.Id : (int?)null;
                 studentId = person.RoleRef == CoreRoles.STUDENT_ROLE.Id ? person.Id : (int?)null;
             }
         }
        
    }
}